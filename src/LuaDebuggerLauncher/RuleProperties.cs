// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

namespace Microsoft.VisualStudio.Debugger.Lua.Rules
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.Utilities;
    using Microsoft.VisualStudio.ProjectSystem.Properties;

    /// <summary>
    /// Provides rule-based property access.
    /// </summary>
    [Export]
    [AppliesTo(ProjectCapabilities.VisualC)]
    internal partial class RuleProperties : StronglyTypedPropertyAccess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProperties"/> class.
        /// </summary>
        [ImportingConstructor]
        public RuleProperties(ConfiguredProject configuredProject)
            : base(configuredProject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProperties"/> class.
        /// </summary>
        public RuleProperties(ConfiguredProject configuredProject, string file, string itemType, string itemName)
            : base(configuredProject, file, itemType, itemName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProperties"/> class.
        /// </summary>
        public RuleProperties(ConfiguredProject configuredProject, IProjectPropertiesContext projectPropertiesContext)
            : base(configuredProject, projectPropertiesContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProperties"/> class.
        /// </summary>
        public RuleProperties(ConfiguredProject configuredProject, UnconfiguredProject unconfiguredProject)
            : base(configuredProject, unconfiguredProject)
        {
        }
    }
}