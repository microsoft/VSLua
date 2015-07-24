using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Options
{
    public class OptionInfo
    {
        public OptionInfo(uint tabSize)
        {
            this.TabSize = tabSize;
        }

        internal uint TabSize { get; }
    }
}
