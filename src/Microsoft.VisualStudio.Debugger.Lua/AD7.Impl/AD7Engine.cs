using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using Microsoft.VisualStudio.Threading;

using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Debugger.Lua
{
    // AD7Engine is the primary entrypoint object for the sample engine. 
    //
    // It implements:
    //
    // IDebugEngine2: This interface represents a debug engine (DE). It is used to manage various aspects of a debugging session, 
    // from creating breakpoints to setting and clearing exceptions.
    //
    // IDebugEngineLaunch2: Used by a debug engine (DE) to launch and terminate programs.
    //
    // IDebugProgram3: This interface represents a program that is running in a process. Since this engine only debugs one process at a time and each 
    // process only contains one program, it is implemented on the engine.
    //
    // IDebugEngineProgram2: This interface provides simultanious debugging of multiple threads in a debuggee.
    [ComVisible(true)]
    [Guid("438738F7-C363-47ED-87DA-42B682E38096")]
    public class AD7Engine : IDebugEngine2, IDebugEngineLaunch2, IDebugProgram3, IDebugEngineProgram2
    {
        private IntPtr threadHandle;

        private IDebugEventCallback2 events;

        // A unique identifier for the program being debugged.
        Guid m_ad7ProgramId;
        uint pID;

        private AsyncQueue<Command> writeCommandQueue = new AsyncQueue<Command>();
        bool keepReadPipeOpen = true;
        bool keepWritePipeOpen = true;

        PROCESS_INFORMATION pi;

        BreakpointManager breakpointManager;

        IDebugProcess2 debugProcess;

        // IDebugThread2 debugThread;
        AD7Thread debugThread;

        ManualResetEvent _programCreateContinued = new ManualResetEvent(false);

        #region IDebugEngine2 Members

        public AD7Engine()
        {
            breakpointManager = new BreakpointManager(this);
        }

        // Attach the debug engine to a program. 
        int IDebugEngine2.Attach(IDebugProgram2[] rgpPrograms, IDebugProgramNode2[] rgpProgramNodes, uint celtPrograms, IDebugEventCallback2 ad7Callback, enum_ATTACH_REASON dwReason)
        {
            int processId = EngineUtils.GetProcessId(rgpPrograms[0]);
            if (processId == 0)
            {
                return VSConstants.E_NOTIMPL;
            }

            pID = (uint)processId;

            events = ad7Callback;

            EngineUtils.RequireOk(rgpPrograms[0].GetProgramId(out m_ad7ProgramId));

            AD7EngineCreateEvent.Send(this);

            AD7ProgramCreateEvent.Send(this);

            debugThread = new AD7Thread(this);

            AD7ThreadCreateEvent.Send(this);

            // This event is optional
            AD7LoadCompleteEvent.Send(this);
           

            return VSConstants.S_OK;
        }

        // Requests that all programs being debugged by this DE stop execution the next time one of their threads attempts to run.
        // This is normally called in response to the user clicking on the pause button in the debugger.
        // When the break is complete, an AsyncBreakComplete event will be sent back to the debugger.
        int IDebugEngine2.CauseBreak()
        {
            return ((IDebugProgram2)this).CauseBreak();
        }

        // Called by the SDM to indicate that a synchronous debug event, previously sent by the DE to the SDM,
        // was received and processed. The only event the sample engine sends in this fashion is Program Destroy.
        // It responds to that event by shutting down the engine.
        int IDebugEngine2.ContinueFromSynchronousEvent(IDebugEvent2 eventObject)
        {
            AD7ProgramCreateEvent programCreate = eventObject as AD7ProgramCreateEvent;
            if (programCreate != null)
            {
                _programCreateContinued.Set();
            }
            
            return VSConstants.S_OK;
        }

        // Creates a pending breakpoint in the engine. A pending breakpoint is contains all the information needed to bind a breakpoint to 
        // a location in the debuggee.
        // Called when new bp set by user
        int IDebugEngine2.CreatePendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, out IDebugPendingBreakpoint2 ppPendingBP)
        {
            breakpointManager.CreatePendingBreakpoint(pBPRequest, out ppPendingBP);

            return VSConstants.S_OK;
        }

        // Informs a DE that the program specified has been atypically terminated and that the DE should 
        // clean up all references to the program and send a program destroy event.
        int IDebugEngine2.DestroyProgram(IDebugProgram2 pProgram)
        {
            return pProgram.Terminate();
            //return (AD7_HRESULT.E_PROGRAM_DESTROY_PENDING);
        }

        // Gets the GUID of the DE.
        int IDebugEngine2.GetEngineId(out Guid guidEngine)
        {
            guidEngine = new Guid(EngineConstants.EngineId);
            return VSConstants.S_OK;
        }

        // Removes the list of exceptions the IDE has set for a particular run-time architecture or language.
        // The sample engine does not support exceptions in the debuggee so this method is not actually implemented.
        int IDebugEngine2.RemoveAllSetExceptions(ref Guid guidType)
        {
            return VSConstants.S_OK;
        }

        // Removes the specified exception so it is no longer handled by the debug engine.
        // The sample engine does not support exceptions in the debuggee so this method is not actually implemented.       
        int IDebugEngine2.RemoveSetException(EXCEPTION_INFO[] pException)
        {
            // The sample engine will always stop on all exceptions.

            return VSConstants.S_OK;
        }

        // Specifies how the DE should handle a given exception.
        // The sample engine does not support exceptions in the debuggee so this method is not actually implemented.
        int IDebugEngine2.SetException(EXCEPTION_INFO[] pException)
        {
            return VSConstants.S_OK;
        }

        // Sets the locale of the DE.
        // This method is called by the session debug manager (SDM) to propagate the locale settings of the IDE so that
        // strings returned by the DE are properly localized. The sample engine is not localized so this is not implemented.
        int IDebugEngine2.SetLocale(ushort wLangID)
        {
            return VSConstants.S_OK;
        }

        // A metric is a registry value used to change a debug engine's behavior or to advertise supported functionality. 
        // This method can forward the call to the appropriate form of the Debugging SDK Helpers function, SetMetric.
        int IDebugEngine2.SetMetric(string pszMetric, object varValue)
        {
            // The sample engine does not need to understand any metric settings.
            return VSConstants.S_OK;
        }

        // Sets the registry root currently in use by the DE. Different installations of Visual Studio can change where their registry information is stored
        // This allows the debugger to tell the engine where that location is.
        int IDebugEngine2.SetRegistryRoot(string pszRegistryRoot)
        {
            // The sample engine does not read settings from the registry.
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugEngineLaunch2 Members

        // Determines if a process can be terminated.
        int IDebugEngineLaunch2.CanTerminateProcess(IDebugProcess2 process)
        {
            if (EngineUtils.GetProcessId(process) == EngineUtils.GetProcessId(debugProcess))
            {
                return VSConstants.S_OK;
            }

            return VSConstants.S_FALSE;
        }

        // Launches a process by means of the debug engine.
        // Normally, Visual Studio launches a program using the IDebugPortEx2::LaunchSuspended method and then attaches the debugger 
        // to the suspended program. However, there are circumstances in which the debug engine may need to launch a program 
        // (for example, if the debug engine is part of an interpreter and the program being debugged is an interpreted language), 
        // in which case Visual Studio uses the IDebugEngineLaunch2::LaunchSuspended method
        // The IDebugEngineLaunch2::ResumeProcess method is called to start the process after the process has been successfully launched in a suspended state.
        int IDebugEngineLaunch2.LaunchSuspended(string pszServer, IDebugPort2 port, string exe, string args, string dir, string env, string options, enum_LAUNCH_FLAGS launchFlags, uint hStdInput, uint hStdOutput, uint hStdError, IDebugEventCallback2 ad7Callback, out IDebugProcess2 process)
        {            
            Debug.Assert(m_ad7ProgramId == Guid.Empty);
            
            m_ad7ProgramId = Guid.NewGuid();

            STARTUPINFO si = new STARTUPINFO();
            pi = new PROCESS_INFORMATION();

            // try/finally free
            bool procOK = NativeMethods.CreateProcess(exe, 
                                                      args,
                                                      IntPtr.Zero,
                                                      IntPtr.Zero,
                                                      false,
                                                      ProcessCreationFlags.CREATE_SUSPENDED,
                                                      IntPtr.Zero,
                                                      null,
                                                      ref si,
                                                      out pi);

            pID = pi.dwProcessId;
            Task writepipeOK = WriteNamedPipeAsync();
            Task readpipeOK = ReadNamedPipeAsync();

            // Marshal.FreeHGlobal(lpEnvironment);
            threadHandle = pi.hThread;
            IntPtr processHandle = pi.hProcess;

            // Inject LuaDebug into host
            IntPtr loadLibAddr = DLLInjector.GetProcAddress(DLLInjector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            string VS140ExtensionPath = Path.Combine(Path.GetDirectoryName(typeof(EngineConstants).Assembly.Location), "LuaDetour");
            string luaDetoursDllName = Path.Combine(VS140ExtensionPath, "LuaDetours.dll");
            if(!File.Exists(luaDetoursDllName))
            {
                process = null;
                return VSConstants.E_FAIL;
            }
            IntPtr allocMemAddress1 = DLLInjector.VirtualAllocEx(processHandle, IntPtr.Zero,
                (uint)((luaDetoursDllName.Length + 1) * Marshal.SizeOf(typeof(char))),
                DLLInjector.MEM_COMMIT | DLLInjector.MEM_RESERVE, DLLInjector.PAGE_READWRITE);

            UIntPtr bytesWritten1;
            DLLInjector.WriteProcessMemory(processHandle, allocMemAddress1,
                Encoding.Default.GetBytes(luaDetoursDllName),
                (uint)((luaDetoursDllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten1);
            IntPtr hRemoteThread1 = DLLInjector.CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibAddr, allocMemAddress1, 0, IntPtr.Zero);

            IntPtr[] handles1 = new IntPtr[] { hRemoteThread1 };
            uint index1;
            CoWaitForMultipleHandles(0, -1, handles1.Length, handles1, out index1);

            string debugDllName = Path.Combine(VS140ExtensionPath, "LuaDebug32.dll");

            IntPtr allocMemAddress2 = DLLInjector.VirtualAllocEx(processHandle, IntPtr.Zero,
                (uint)((debugDllName.Length + 1) * Marshal.SizeOf(typeof(char))),
                DLLInjector.MEM_COMMIT | DLLInjector.MEM_RESERVE, DLLInjector.PAGE_READWRITE);

            UIntPtr bytesWritten2;
            DLLInjector.WriteProcessMemory(processHandle, allocMemAddress2,
                Encoding.Default.GetBytes(debugDllName), (uint)((debugDllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten2);

            IntPtr hRemoteThread2 = DLLInjector.CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibAddr, allocMemAddress2, 0, IntPtr.Zero);
            IntPtr[] handles = new IntPtr[] { hRemoteThread2 };
            uint index2;
            CoWaitForMultipleHandles(0, -1, handles.Length, handles, out index2);


            AD_PROCESS_ID adProcessId = new AD_PROCESS_ID();
            adProcessId.ProcessIdType = (uint)enum_AD_PROCESS_ID.AD_PROCESS_ID_SYSTEM;
            adProcessId.dwProcessId = pi.dwProcessId;

            EngineUtils.RequireOk(port.GetProcess(adProcessId, out process));
            debugProcess = process;

            return VSConstants.S_OK;
        }

        [DllImport("ole32.dll")]
        static extern int CoWaitForMultipleHandles(uint dwFlags, int dwTimeout, int cHandles, IntPtr[] pHandles, out uint lpdwindex);

        private async Task WriteNamedPipeAsync()
        {
            string pipeID = pID.ToString();

            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("luaPipeW" + pipeID, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                await Task.Factory.FromAsync((cb, state) => pipeServer.BeginWaitForConnection(cb, state), ar => pipeServer.EndWaitForConnection(ar), null);

                using (StreamWriter pipeWriter = new StreamWriter(pipeServer))
                {
                    pipeWriter.AutoFlush = true;
                    // keepWritePipeOpen = true;

                    // Transition to a background thread
                    await TaskScheduler.Default;

                    while (keepWritePipeOpen)
                    {
                        Command command = await this.writeCommandQueue.DequeueAsync();

                        switch (command.Kind)
                        {
                            case CommandKind.Breakpoint:
                                BreakpointCommand bpCommand = command as BreakpointCommand;
                                await pipeWriter.WriteLineAsync("Breakpoint\0");
                                await pipeWriter.WriteLineAsync(bpCommand.File + "\0");
                                await pipeWriter.WriteLineAsync(bpCommand.LineNumber.ToString() + "\0");
                                break;
                            case CommandKind.Step:
                                await pipeWriter.WriteLineAsync("Step\0");
                                break;
                            case CommandKind.Continue:
                                await pipeWriter.WriteLineAsync("Continue\0");
                                break;
                            case CommandKind.Detach:
                                keepWritePipeOpen = false;
                                break;
                            case CommandKind.DebuggerEnvironmentReady:
                                await pipeWriter.WriteLineAsync("DebuggerEnvironmentReady\0");
                                break;
                        }
                    }
                }
            }
        }

        private async Task ReadNamedPipeAsync()
        {
            string pipeID = pID.ToString();

            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("luaPipeR" + pipeID, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                await Task.Factory.FromAsync((cb, state) => pipeServer.BeginWaitForConnection(cb, state), ar => pipeServer.EndWaitForConnection(ar), null);

                using (StreamReader pipeReader = new StreamReader(pipeServer))
                {
                    await TaskScheduler.Default;

                    while (this.keepReadPipeOpen)
                    {
                        string command = await pipeReader.ReadLineAsync();

                        switch (command)
                        {
                            case "BreakpointHit":
                            {
                                // this.writeCommandQueue.Enqueue(new Command(CommandKind.Locals));
                                // this.writeCommandQueue.Enqueue(new Command(CommandKind.CallStack));
                                debugThread.SourceFile = await pipeReader.ReadLineAsync();
                                debugThread.Line = uint.Parse(await pipeReader.ReadLineAsync());
                                debugThread.FuncName = await pipeReader.ReadLineAsync();

                                // Receive Callstack
                                debugThread.FrameCount = int.Parse(await pipeReader.ReadLineAsync());

                                List<Frame> frames = new List<Frame>(debugThread.FrameCount);
                                // List<string> frames = new List<string>(debugThread.FrameCount);

                                for (int stackLineIndex = 0; stackLineIndex < debugThread.FrameCount; stackLineIndex++)
                                {
                                    string func = await pipeReader.ReadLineAsync();
                                    string source = await pipeReader.ReadLineAsync();
                                    string line = await pipeReader.ReadLineAsync();
                                    frames.Add(new Frame(func, source, line));
                                    // frames.Add(await pipeReader.ReadLineAsync());
                                }

                                debugThread.StackFrames = frames;

                                    // TODO get locals
                                int numberToRead = int.Parse(await pipeReader.ReadLineAsync());

                                List<Variable> variables = new List<Variable>(numberToRead);

                                for (int localIndex = 0; localIndex < numberToRead; localIndex++)
                                {
                                    variables.Add(new Variable(await pipeReader.ReadLineAsync(), await pipeReader.ReadLineAsync(), await pipeReader.ReadLineAsync()));
                                }

                                debugThread.NumberOfLocals = numberToRead;
                                debugThread.Locals = variables;
                                AD7BreakpointEvent.Send(this, breakpointManager.GetBoundBreakpoint(debugThread.SourceFile + debugThread.Line));
                                break;
                            }
                            case "BreakpointBound":
                                string fileandline = await pipeReader.ReadLineAsync();
                                AD7BoundBreakpoint boundbp = breakpointManager.GetBoundBreakpoint(fileandline);

                                AD7BreakpointBoundEvent boundBreakpointEvent = new AD7BreakpointBoundEvent(boundbp);
                                Send(boundBreakpointEvent, AD7BreakpointBoundEvent.IID, this);
                                break;
                            case "StepComplete":
                            {
                                debugThread.FrameCount = 1;
                                debugThread.SourceFile = await pipeReader.ReadLineAsync();
                                debugThread.Line = uint.Parse(await pipeReader.ReadLineAsync());
                                debugThread.FuncName = await pipeReader.ReadLineAsync();

                                // Receive Callstack
                                debugThread.FrameCount = int.Parse(await pipeReader.ReadLineAsync());

                                List<Frame> frames = new List<Frame>(debugThread.FrameCount);

                                for (int stackLineIndex = 0; stackLineIndex < debugThread.FrameCount; stackLineIndex++)
                                {
                                    string func = await pipeReader.ReadLineAsync();
                                    string source = await pipeReader.ReadLineAsync();
                                    string line = await pipeReader.ReadLineAsync();
                                    frames.Add(new Frame(func, source, line));
                                }

                                debugThread.StackFrames = frames;
                                    
                                int numberToRead = int.Parse(await pipeReader.ReadLineAsync());

                                List<Variable> variables = new List<Variable>(numberToRead);

                                for (int localIndex = 0; localIndex < numberToRead; localIndex++)
                                {
                                    variables.Add(new Variable(await pipeReader.ReadLineAsync(), await pipeReader.ReadLineAsync(), await pipeReader.ReadLineAsync()));
                                }

                                debugThread.NumberOfLocals = numberToRead;
                                debugThread.Locals = variables;


                                Send(new AD7StepCompleteEvent(), AD7StepCompleteEvent.IID, this);
                                break;
                            }
                        }
                    }
                }

            }
        }

        // Resume a process launched by IDebugEngineLaunch2.LaunchSuspended
        int IDebugEngineLaunch2.ResumeProcess(IDebugProcess2 process)
        {
            IDebugPort2 port;
            EngineUtils.RequireOk(process.GetPort(out port));

            IDebugDefaultPort2 defaultPort = (IDebugDefaultPort2)port;

            IDebugPortNotify2 portNotify;
            EngineUtils.RequireOk(defaultPort.GetPortNotify(out portNotify));

            EngineUtils.RequireOk(portNotify.AddProgramNode(new AD7ProgramNode((int)pi.dwProcessId)));

            if (this.m_ad7ProgramId == Guid.Empty)
            {
                Debug.Fail("Attaching failed");
                return VSConstants.E_FAIL;
            }

            this._programCreateContinued.WaitOne();
            this.writeCommandQueue.Enqueue(new Command(CommandKind.DebuggerEnvironmentReady));

            NativeMethods.ResumeThread(threadHandle);
            return 0;
        }

        // This function is used to terminate a process that the SampleEngine launched
        // The debugger will call IDebugEngineLaunch2::CanTerminateProcess before calling this method.
        int IDebugEngineLaunch2.TerminateProcess(IDebugProcess2 process)
        {
            if (EngineUtils.GetProcessId(process) != EngineUtils.GetProcessId(debugProcess))
            {
                return VSConstants.S_FALSE;
            }
            
            EnqueueCommand(new Command(CommandKind.Detach));

            return debugProcess.Terminate();
        }

        #endregion

        #region IDebugProgram2 Members

        // Determines if a debug engine (DE) can detach from the program.
        public int CanDetach()
        {
            // The sample engine always supports detach
            return VSConstants.S_OK;
        }

        // The debugger calls CauseBreak when the user clicks on the pause button in VS. The debugger should respond by entering
        // breakmode. 
        public int CauseBreak()
        {
            // TODO: pause, async break P2

            return VSConstants.S_OK;
        }

        // Continue is called from the SDM when it wants execution to continue in the debugee
        // but have stepping state remain. An example is when a tracepoint is executed, 
        // and the debugger does not want to actually enter break mode.
        public int Continue(IDebugThread2 pThread)
        {
            return VSConstants.S_OK;
        }

        // Detach is called when debugging is stopped and the process was attached to (as opposed to launched)
        // or when one of the Detach commands are executed in the UI.
        public int Detach()
        {
            return VSConstants.E_NOTIMPL;
        }

        // Enumerates the code contexts for a given position in a source file.
        public int EnumCodeContexts(IDebugDocumentPosition2 pDocPos, out IEnumDebugCodeContexts2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        // EnumCodePaths is used for the step-into specific feature -- right click on the current statment and decide which
        // function to step into. This is not something that the SampleEngine supports.
        public int EnumCodePaths(string hint, IDebugCodeContext2 start, IDebugStackFrame2 frame, int fSource, out IEnumCodePaths2 pathEnum, out IDebugCodeContext2 safetyContext)
        {
            pathEnum = null;
            safetyContext = null;
            return VSConstants.E_NOTIMPL;
        }

        // EnumModules is called by the debugger when it needs to enumerate the modules in the program.
        public int EnumModules(out IEnumDebugModules2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        // EnumThreads is called by the debugger when it needs to enumerate the threads in the program.
        public int EnumThreads(out IEnumDebugThreads2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        // The properties returned by this method are specific to the program. If the program needs to return more than one property, 
        // then the IDebugProperty2 object returned by this method is a container of additional properties and calling the 
        // IDebugProperty2::EnumChildren method returns a list of all properties.
        // A program may expose any number and type of additional properties that can be described through the IDebugProperty2 interface. 
        // An IDE might display the additional program properties through a generic property browser user interface.
        // The sample engine does not support this
        public int GetDebugProperty(out IDebugProperty2 ppProperty)
        {
            ppProperty = null;
            return VSConstants.E_NOTIMPL;
        }

        // The debugger calls this when it needs to obtain the IDebugDisassemblyStream2 for a particular code-context.
        // The sample engine does not support dissassembly so it returns E_NOTIMPL
        public int GetDisassemblyStream(enum_DISASSEMBLY_STREAM_SCOPE dwScope, IDebugCodeContext2 codeContext, out IDebugDisassemblyStream2 disassemblyStream)
        {
            disassemblyStream = null;
            return VSConstants.E_NOTIMPL;
        }

        // This method gets the Edit and Continue (ENC) update for this program. A custom debug engine always returns E_NOTIMPL
        public int GetENCUpdate(out object update)
        {
            // The sample engine does not participate in managed edit & continue.
            update = null;
            return VSConstants.S_OK;
        }

        // Gets the name and identifier of the debug engine (DE) running this program.
        public int GetEngineInfo(out string engineName, out Guid engineGuid)
        {
            engineName = ResourceStrings.EngineName;
            engineGuid = new Guid(EngineConstants.EngineId);
            return VSConstants.S_OK;
        }

        // The memory bytes as represented by the IDebugMemoryBytes2 object is for the program's image in memory and not any memory 
        // that was allocated when the program was executed.
        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            ppMemoryBytes = null;
            return VSConstants.E_NOTIMPL;
        }

        // Gets the name of the program.
        // The name returned by this method is always a friendly, user-displayable name that describes the program.
        public int GetName(out string programName)
        {
            // The Sample engine uses default transport and doesn't need to customize the name of the program,
            // so return NULL.
            programName = null;
            return VSConstants.S_OK;
        }

        // Gets a GUID for this program. A debug engine (DE) must return the program identifier originally passed to the IDebugProgramNodeAttach2::OnAttach
        // or IDebugEngine2::Attach methods. This allows identification of the program across debugger components.
        public int GetProgramId(out Guid guidProgramId)
        {
            Debug.Assert(m_ad7ProgramId != Guid.Empty);

            guidProgramId = m_ad7ProgramId;
            return VSConstants.S_OK;
        }

        // This method is deprecated. Use the IDebugProcess3::Step method instead.
        public int Step(IDebugThread2 pThread, enum_STEPKIND sk, enum_STEPUNIT Step)
        {
            // TODO stepping,
            EnqueueCommand(new Command(CommandKind.Step));
            return VSConstants.S_OK;
        }

        // Terminates the program.
        public int Terminate()
        {
            // Because the sample engine is a native debugger, it implements IDebugEngineLaunch2, and will terminate
            // the process in IDebugEngineLaunch2.TerminateProcess
            return VSConstants.S_OK;
        }

        // Writes a dump to a file.
        public int WriteDump(enum_DUMPTYPE DUMPTYPE, string pszDumpUrl)
        {
            // The sample debugger does not support creating or reading mini-dumps.
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IDebugProgram3 Members

        // ExecuteOnThread is called when the SDM wants execution to continue and have 
        // stepping state cleared.
        public int ExecuteOnThread(IDebugThread2 pThread)
        {
            EnqueueCommand(new Command(CommandKind.Continue));

            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugEngineProgram2 Members

        // Stops all threads running in this program.
        // This method is called when this program is being debugged in a multi-program environment. When a stopping event from some other program 
        // is received, this method is called on this program. The implementation of this method should be asynchronous; 
        // that is, not all threads should be required to be stopped before this method returns. The implementation of this method may be 
        // as simple as calling the IDebugProgram2::CauseBreak method on this program.
        //
        // The sample engine only supports debugging native applications and therefore only has one program per-process
        public int Stop()
        {
            return VSConstants.E_NOTIMPL;
        }

        // WatchForExpressionEvaluationOnThread is used to cooperate between two different engines debugging 
        // the same process. The sample engine doesn't cooperate with other engines, so it has nothing
        // to do here.
        public int WatchForExpressionEvaluationOnThread(IDebugProgram2 pOriginatingProgram, uint dwTid, uint dwEvalFlags, IDebugEventCallback2 pExprCallback, int fWatch)
        {
            return VSConstants.S_OK;
        }

        // WatchForThreadStep is used to cooperate between two different engines debugging the same process.
        // The sample engine doesn't cooperate with other engines, so it has nothing to do here.
        public int WatchForThreadStep(IDebugProgram2 pOriginatingProgram, uint dwTid, int fWatch, uint dwFrame)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region Deprecated interface methods
        // These methods are not called by the Visual Studio debugger, so they don't need to be implemented

        int IDebugEngine2.EnumPrograms(out IEnumDebugPrograms2 programs)
        {
            Debug.Fail("This function is not called by the debugger");

            programs = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Attach(IDebugEventCallback2 pCallback)
        {
            Debug.Fail("This function is not called by the debugger");

            return VSConstants.E_NOTIMPL;
        }

        public int GetProcess(out IDebugProcess2 process)
        {
            Debug.Fail("This function is not called by the debugger");
            process = null;
            return VSConstants.S_OK;
        }

        public int Execute()
        {
            Debug.Fail("This function is not called by the debugger.");
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region Events

        internal void Send(IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program)
        {
            uint attributes;
            Guid riidEvent = new Guid(iidEvent);

            EngineUtils.RequireOk(eventObject.GetAttributes(out attributes));

            Debug.WriteLine(string.Format("Sending Event: {0} {1}", eventObject.GetType(), iidEvent));
            try
            {
                EngineUtils.RequireOk(events.Event(this, debugProcess, this, debugThread, eventObject, ref riidEvent, attributes));
            }
            catch (InvalidCastException)
            {
                // COM object has gone away
            }
        }

        internal void Send(IDebugEvent2 eventObject, string iidEvent)
        {
            Send(eventObject, iidEvent, this);
        }

        #endregion

        internal void EnqueueCommand(Command command)
        {
            writeCommandQueue.Enqueue(command);
        }
    }
}
