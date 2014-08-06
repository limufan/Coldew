using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;

using Coldew.Core.Organization;


namespace Coldew.Core
{
    public class JsonField : Field
    {
        public JsonField(JsonFieldNewInfo newInfo):
            base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.Json; }
        }

        public override string TypeName
        {
            get { return "Json"; }
        }

        public string DefaultValue { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new JsonMetadataValue(value, this);
        }

    }
}
