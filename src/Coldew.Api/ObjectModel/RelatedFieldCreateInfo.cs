using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class RelatedFieldCreateInfo : FieldCreateInfo
    {
        public RelatedFieldCreateInfo(string code, string name, string relatedFieldCode, string propertyCode)
            :base(code, name)
        {
            this.RelatedFieldCode = relatedFieldCode;
            this.PropertyCode = propertyCode;
        }

        public string RelatedFieldCode { set; get; }

        public string PropertyCode { set; get; }
    }
}
