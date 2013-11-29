using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class NumberFieldCreateInfo : FieldCreateInfo
    {
        public NumberFieldCreateInfo(string code, string name)
            :base(code, name)
        {
            
        }

        public decimal? DefaultValue{set;get;} 

        public decimal? Max{set;get;} 

        public decimal? Min{set;get;}

        public int Precision { set; get; }
    }
}
