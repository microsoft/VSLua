/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LanguageServices.Lua
{

    using Interop = Microsoft.VisualStudio.Shell.Interop;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Guids.PackageString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]

    [ProvideOptionPage(typeof(GeneralPage), Constants.Formatting.Category, Constants.Formatting.Pages.General,
        1000, 1002, true, ProfileMigrationType = ProfileMigrationType.PassThrough)]

    [ProvideOptionPage(typeof(SpacingPage), Constants.Formatting.Category, Constants.Formatting.Pages.Spacing,
        1000, 1004, true, ProfileMigrationType = ProfileMigrationType.PassThrough)]

    /*[ProvideOptionPage(typeof(WrappingAndNewLinePage), Constants.Formatting.Category, Constants.Formatting.Pages.WrappingAndNewLines,
        1000, 1005, true, ProfileMigrationType = ProfileMigrationType.PassThrough)]*/

    public sealed class LuaLanguageServicePackage : Package
    {
        public LuaLanguageServicePackage()
        {
            // initiaization stuff before package creation
        }

        #region Package Members

        protected override void Initialize()
        {
            var uiShell = (Interop.IVsUIShell2)this.GetService(typeof(Interop.IVsUIShell2));
            
            base.Initialize();
        }

        internal UserSettings FormattingUserSettings
        {
            get
            {
                return (UserSettings)this.GetAutomationObject($"{Constants.Formatting.Category}.{Constants.Formatting.Pages.General}");
            }
        }

        #endregion
    }
}
