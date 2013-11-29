using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class StringFieldCreateInfo : FieldCreateInfo
    {
        public StringFieldCreateInfo(string code, string name)
            :base(code, name)
        {
            
        }

        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }
    }
}
