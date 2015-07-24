//------------------------------------------------------------------------------
// <copyright file="Lua.Host.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LuaLanguageService
{
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(LuaLanguageServicePackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class LuaLanguageServicePackage : Package
    {

        public const string PackageGuidString = "40c3d121-7e37-4d03-a9f8-f10bca9805f3";

        /// <summary>
        /// Initializes a new instance of the <see cref="Lua.Host"/> class.
        /// </summary>
        public LuaLanguageServicePackage()
        {
            // initiaization stuff before package creation
        }

        #region Package Members

        protected override void Initialize()
        {
            var container = this as IServiceContainer;

            if (container != null)
            {
                container.AddService(typeof(LuaLanguageService),
                    (serviceContainer, t) => new LuaLanguageService(serviceContainer), true);
            }

            base.Initialize();
        }

        #endregion
    }
}
