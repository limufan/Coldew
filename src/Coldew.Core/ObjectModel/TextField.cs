using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class TextField : Field
    {
        public TextField(TextFieldNewInfo newInfo)
            :base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.Text; }
        }

        public override string TypeName
        {
            get { return "长文本"; }
        }

        public string DefaultValue { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new StringMetadataValue(value.ToString(), this);
        }
    }
}
