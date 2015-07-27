//------------------------------------------------------------------------------
// <copyright file="Lua.Host.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.LuaLanguageService.Shared;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LuaLanguageService
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Constants.Package.Guids.String)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class LuaLanguageServicePackage : Package
    {
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
