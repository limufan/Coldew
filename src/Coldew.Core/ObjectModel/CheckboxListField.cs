using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class CheckboxListField : Field
    {
        public CheckboxListField(CheckboxListFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.CheckboxList; }
        }

        public override string TypeName
        {
            get { return "复选框"; }
        }

        public List<string> DefaultValue { set; get; }

        public List<string> SelectList { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            List<string> stringList = new List<string>();
            if (value != null)
            {
                foreach (JToken str in value)
                {
                    stringList.Add(str.ToString());
                }
            }
            return new StringListMetadataValue(stringList, this);
        }
    }
}
