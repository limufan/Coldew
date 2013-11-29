using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class MetadataFieldCreateInfo : FieldCreateInfo
    {
        public MetadataFieldCreateInfo(string code, string name, string objectCode)
            :base(code, name)
        {
            this.ObjectCode = objectCode;
        }

        public string ObjectCode{set;get;} 
    }
}
