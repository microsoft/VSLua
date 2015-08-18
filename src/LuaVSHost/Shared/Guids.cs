using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class Guids
    {
        internal const string PackageString = "40c3d121-7e37-4d03-a9f8-f10bca9805f3";

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        internal static readonly Guid Package = new Guid(PackageString);

        internal const string ServiceString = "88A1F488-9D00-4896-A255-6F8251208B90";

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        internal static readonly Guid Service = new Guid(ServiceString);

        internal const string GeneralPageString = "9CA639C0-CFC8-4583-95CE-D35E97940788";
        internal const string SpacingPageString = "59E82B53-EC56-4EE0-A682-EF331109BA09";
        internal const string WrappingAndNewLinePageString = "45482116-B8FB-46E7-A51D-518FCE83B626";
    }
}
