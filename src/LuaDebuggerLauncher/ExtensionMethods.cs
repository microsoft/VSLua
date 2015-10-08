// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

namespace Microsoft.VisualStudio.Debugger.Lua
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders;
    using Microsoft.VisualStudio.ProjectSystem.VS.Debuggers;

    internal static class ExtensionMethods
    {
        /// <summary>
        /// Clones an <see cref="IDebugLaunchSettings"/> instance so that it can be changed.
        /// </summary>
        /// <param name="value">The instance to clone.</param>
        /// <returns>The new instance.</returns>
        internal static DebugLaunchSettings Clone(this IDebugLaunchSettings value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var result = new DebugLaunchSettings(value.LaunchOptions)
            {
                Arguments = value.Arguments,
                CurrentDirectory = value.CurrentDirectory,
                Executable = value.Executable,
                LaunchDebugEngineGuid = value.LaunchDebugEngineGuid,
                LaunchOperation = value.LaunchOperation,
                Options = value.Options,
                PortName = value.PortName,
                RemoteMachine = value.RemoteMachine,
                PortSupplierGuid = value.PortSupplierGuid,
                StandardErrorHandle = value.StandardErrorHandle,
                StandardOutputHandle = value.StandardOutputHandle,
                StandardInputHandle = value.StandardInputHandle,
                SendToOutputWindow = value.SendToOutputWindow,
                Unknown = value.Unknown,
                ProcessId = value.ProcessId,
                ProcessLanguageGuid = value.ProcessLanguageGuid,
            };
            foreach (var engine in value.AdditionalDebugEngines)
            {
                result.AdditionalDebugEngines.Add(engine);
            }
            foreach (var env in value.Environment)
            {
                result.Environment[env.Key] = env.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets the first export from a list of exports that matches some metadata name=value pair,
        /// or null if no match was found.
        /// </summary>
        /// <typeparam name="T">The interface exported by the MEF part.</typeparam>
        internal static T FindByMetadata<T>(this IEnumerable<Lazy<T, IDictionary<string, object>>> exports, string metadataName, string metadataValue)
        {
            if (exports == null)
            {
                throw new ArgumentNullException("exports");
            }
            if (string.IsNullOrEmpty(metadataName))
            {
                throw new ArgumentException("metadataName");
            }
            if (metadataValue == null)
            {
                throw new ArgumentNullException("metadataValue");
            }

            // Loop through each component in the composition and find the one that matches the given metadata.
            T result = (from export in exports
                        where export.Metadata.ContainsKey(metadataName)
                        let actualValue = export.Metadata[metadataName] as string
                        where String.Equals(actualValue, metadataValue, StringComparison.OrdinalIgnoreCase)
                        select export.Value).FirstOrDefault();

            return result;
        }
    }
}
