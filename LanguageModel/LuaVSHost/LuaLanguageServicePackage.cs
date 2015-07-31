//------------------------------------------------------------------------------
// <copyright file="Lua.Host.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LanguageServices.Lua
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Guids.PackageString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]

    [ProvideOptionPage(typeof(GeneralPage), Constants.Formatting.Category, Constants.Formatting.Pages.General,
        1000, 1002, false, ProfileMigrationType = ProfileMigrationType.PassThrough)]

    [ProvideOptionPage(typeof(IndentationPage), Constants.Formatting.Category, Constants.Formatting.Pages.Indentation,
        1000, 1003, false, ProfileMigrationType = ProfileMigrationType.PassThrough)]

    [ProvideOptionPage(typeof(SpacingPage), Constants.Formatting.Category, Constants.Formatting.Pages.Spacing,
        1000, 1004, false, ProfileMigrationType = ProfileMigrationType.PassThrough)]

    [ProvideOptionPage(typeof(WrappingAndNewLinePage), Constants.Formatting.Category, Constants.Formatting.Pages.WrappingAndNewLines,
        1000, 1005, false, ProfileMigrationType = ProfileMigrationType.PassThrough)]



    public sealed class LuaLanguageServicePackage : Package
    {
        public LuaLanguageServicePackage()
        {
            // initiaization stuff before package creation
        }

        #region Package Members

        protected override void Initialize()
        {
            var container = (IServiceContainer)this;

            if (container != null)
            {
                container.AddService(typeof(LuaLanguageService),
                    (serviceContainer, t) => new LuaLanguageService(serviceContainer), promote: true);
            }

            base.Initialize();
        }

        #endregion
    }
}
