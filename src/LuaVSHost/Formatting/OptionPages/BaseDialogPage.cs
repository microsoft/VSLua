/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages
{
    internal abstract class BaseDialogPage : UIElementDialogPage
    {
        public override object AutomationObject
        {
            get
            {
                return UserSettings.MainInstance;
            }
        }
    }
}
