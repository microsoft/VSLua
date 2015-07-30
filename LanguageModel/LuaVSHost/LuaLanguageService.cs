using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LuaLanguageService.Shared;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.LuaLanguageService
{
    [Guid(Guids.ServiceString)]
    public sealed class LuaLanguageService : IServiceProvider
    {
        private IServiceProvider serviceProvider;
        private IComponentModel componentModel;
        private ICore core;

        public object GetService(Type serviceType)
        {
            return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
        }

        public LuaLanguageService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        internal IComponentModel ComponentModel
        {
            get
            {
                if (this.componentModel == null && this.serviceProvider != null)
                {
                    this.componentModel = this.serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
                }
                return this.componentModel;
            }
        }

        internal ICore Core
        {
            get
            {
                if (this.core == null && this.serviceProvider != null)
                {
                    if (this.ComponentModel != null)
                    {
                        this.core = this.ComponentModel.GetService<ICore>();
                    }
                }
                return this.core;
            }
        }

    }
}
