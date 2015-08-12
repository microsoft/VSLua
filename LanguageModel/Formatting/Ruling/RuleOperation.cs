/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using Validation;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleOperation
    {
        internal RuleOperationContext Context { get; }
        internal RuleAction Action { get; }

        internal RuleOperation(RuleOperationContext context, RuleAction action)
        {
            Requires.NotNull(context, nameof(context));

            this.Action = action;
            this.Context = context;
        }
    }
}
