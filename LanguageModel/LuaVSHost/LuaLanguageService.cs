using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.LuaLanguageService
{
    [Guid("F0911DC5-9D97-4780-BB5F-ED649DD31C66")]
    public sealed class LuaLanguageService : IServiceProvider
    {
        private IServiceProvider serviceProvider;

        public object GetService(Type serviceType)
        {
            if (this.serviceProvider != null)
            {
                return serviceProvider;
            }
            return null;
        }

        public LuaLanguageService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
