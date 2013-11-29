using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class TextFieldCreateInfo : FieldCreateInfo
    {
        public TextFieldCreateInfo(string code, string name)
            :base(code, name)
        {
            
        }

        public string DefaultValue { set; get; }
    }
}
