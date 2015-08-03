//------------------------------------------------------------------------------
// <copyright file="LuaLanguageServicePackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
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
        1000, 1002, true, ProfileMigrationType = ProfileMigrationType.PassThrough)]
    public sealed class LuaLanguageServicePackage : Package
    {
        public LuaLanguageServicePackage()
        {
            // initiaization stuff before package creation
        }

        #region Package Members

        protected override void Initialize()
        {
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
