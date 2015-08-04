//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using IOleConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
using IOleConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;

// Note: This class is coming in from the JavaScriptLanguageService source.
namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{

    /// <summary>
    /// This class manages a connection point to an events interface on a COM classic object,
    /// which will call on a managed code sink that implements that interface.
    /// </summary>
    [ComVisible(false)]
    internal sealed class ConnectionPointCookie
    {
        private IOleConnectionPoint connectionPoint;
        private uint cookie;
#if DEBUG
        private StackTrace cookieStack;
#endif

        /// <summary>
        /// Creates a connection point to of the given interface type.
        /// which will call on a managed code sink that implements that interface.
        /// </summary>
        /// <param name='source'>
        /// The object that exposes the events.  This object must implement IConnectionPointContainer or an InvalidCastException will be thrown.
        /// </param>
        /// <param name='sink'>
        /// The object to sink the events.  This object must implement the interface eventInterface, or an InvalidCastException is thrown.
        /// </param>
        /// <param name='eventInterface'>
        /// The event interface to sink.  The sink object must support this interface and the source object must expose it through it's ConnectionPointContainer.
        /// </param>
        public ConnectionPointCookie(object source, object sink, Type eventInterface)
            : this(source, sink, eventInterface, true)
        {
        }

        /// <summary>
        /// Creates a connection point to of the given interface type.
        /// which will call on a managed code sink that implements that interface.
        /// </summary>
        /// <param name='source'>
        /// The object that exposes the events.  This object must implement IConnectionPointContainer or an InvalidCastException will be thrown.
        /// </param>
        /// <param name='sink'>
        /// The object to sink the events.  This object must implement the interface eventInterface, or an InvalidCastException is thrown.
        /// </param>
        /// <param name='eventInterface'>
        /// The event interface to sink.  The sink object must support this interface and the source object must expose it through it's ConnectionPointContainer.
        /// </param>
        /// <param name='throwException'>
        /// If true, exceptions described will be thrown, otherwise object will silently fail to connect.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly")]
        public ConnectionPointCookie(object source, object sink, Type eventInterface, bool throwException)
        {
            Exception ex = null;
            IOleConnectionPointContainer cpc = source as IOleConnectionPointContainer;
            if (cpc != null)
            {
                try
                {
                    Guid tmp = eventInterface.GUID;
                    cpc.FindConnectionPoint(ref tmp, out this.connectionPoint);
                }
                catch
                {
                    this.connectionPoint = null;
                }

                if (this.connectionPoint == null)
                {
                    ex = new ArgumentException(eventInterface.Name);
                }
                else if (!eventInterface.IsInstanceOfType(sink))
                {
                    ex = new InvalidCastException();
                }
                else
                {
                    try
                    {
                        this.cookie = 0;
                        this.connectionPoint.Advise(sink, out this.cookie);
                    }
                    catch
                    {
                        this.cookie = 0;
                    }
                    if (this.cookie == 0)
                    {
                        this.connectionPoint = null;
                        ex = new InvalidOperationException(eventInterface.Name);
                    }
                }
            }
            else
            {
                ex = new InvalidCastException();
            }

            if (throwException && (this.connectionPoint == null || this.cookie == 0))
            {
                if (ex == null)
                {
                    throw new ArgumentException(eventInterface.Name);
                }
                else
                {
                    throw ex;
                }
            }

#if DEBUG
            if (this.cookie != 0)
            {
                this.cookieStack = new StackTrace();
            }
#endif
        }

        /// <summary>
        /// Disconnect the current connection point.  If the object is not connected,
        /// this method will do nothing.
        /// </summary>
        public void Disconnect()
        {
            this.Disconnect(true);
        }

        /// <summary>
        /// Disconnect the current connection point.  If the object is not connected,
        /// this method will do nothing.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        public void Disconnect(bool release)
        {
            if (this.connectionPoint != null && this.cookie != 0)
            {
                this.connectionPoint.Unadvise(this.cookie);
                this.cookie = 0;

                if (release)
                {
                    Marshal.ReleaseComObject(this.connectionPoint);
                }

                this.connectionPoint = null;
            }

#if DEBUG
            GC.SuppressFinalize(this);
#endif
        }
    }

}

