using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.LuaLanguageService.Shared;

namespace Microsoft.VisualStudio.LuaLanguageService
{
    [Guid(Guids.ServiceString)]
    public sealed class LuaLanguageService : IServiceProvider
    {
        private IServiceProvider serviceProvider;

        public object GetService(Type serviceType)
        {
            return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
        }

        public LuaLanguageService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
