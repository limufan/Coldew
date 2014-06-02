using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class CodeFieldCreateInfo : FieldCreateInfo
    {
        public CodeFieldCreateInfo(string code, string name, string format)
            :base(code, name)
        {
            this.Format = format;   
        }

        public string Format { set; get; }
    }
}
