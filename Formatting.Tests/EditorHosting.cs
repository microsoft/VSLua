using Microsoft.VisualStudio.Composition;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Formatting.Tests
{
    class EditorHost
    {
        private const string EditorAssemblyNames = @"
            Microsoft.VisualStudio.Platform.VSEditor, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
            Microsoft.VisualStudio.Text.Logic, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
            Microsoft.VisualStudio.Text.UI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
            Microsoft.VisualStudio.Text.UI.Wpf, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
            Microsoft.VisualStudio.Language.StandardClassification, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
            Microsoft.VisualStudio.CoreUtility, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
        ";

        internal static async Task<ExportProvider> CreateExportProviderAsync(ITestOutputHelper logger)
        {
            var assemblies = new List<Assembly>();
            foreach (string line in EditorAssemblyNames.Split('\n'))
            {
                string assemblyName = line.Trim();
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    var assembly = Assembly.Load(assemblyName);
                    assemblies.Add(assembly);
                }
            }

            var discovery = new AttributedPartDiscoveryV1(Resolver.DefaultInstance);
            var parts = await discovery.CreatePartsAsync(assemblies);
            var catalog = ComposableCatalog.Create(Resolver.DefaultInstance)
                .AddParts(parts)
                .WithDesktopSupport()
                .WithCompositionService();
            var configuration = CompositionConfiguration.Create(catalog);
            if (!configuration.CompositionErrors.IsEmpty)
            {
                foreach (var error in configuration.CompositionErrors.First())
                {
                    logger.WriteLine(error.Message);
                }

                configuration.ThrowOnErrors();
            }

            var composition = RuntimeComposition.CreateRuntimeComposition(configuration);
            var expf = composition.CreateExportProviderFactory();
            return expf.CreateExportProvider();
        }
    }
}
