using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class StringField : Field
    {
        public StringField(StringFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.String; }
        }

        public override string TypeName
        {
            get { return "短文本"; }
        }

        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new StringMetadataValue(value.ToString(), this);
        }
    }
}
