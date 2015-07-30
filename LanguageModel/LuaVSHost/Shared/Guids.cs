using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class Guids
    {
        internal const string PackageString = "40c3d121-7e37-4d03-a9f8-f10bca9805f3";
        internal static readonly Guid Package = new Guid(PackageString);

        internal const string ServiceString = "88A1F488-9D00-4896-A255-6F8251208B90";
        internal static readonly Guid Service = new Guid(ServiceString);

        internal const string GeneralPageString = "9CA639C0-CFC8-4583-95CE-D35E97940788";
        internal const string SpacingPageString = "59E82B53-EC56-4EE0-A682-EF331109BA09";
        internal const string IndentationPageString = "687A4168-DE55-4B30-A8E0-5C3A1517642E";
        internal const string WrappingPageString = "45482116-B8FB-46E7-A51D-518FCE83B626";
        internal const string NewLinesPageString = "B459B8FE-A9F3-43D5-97F2-2A99914DD3B1";

    }
}
