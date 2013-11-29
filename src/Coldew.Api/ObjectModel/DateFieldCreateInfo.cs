using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class DateFieldCreateInfo : FieldCreateInfo
    {
        public DateFieldCreateInfo(string code, string name)
            :base(code, name)
        {
            
        }

        public bool DefaultValueIsToday { set; get; }
    }
}
