using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;
using Coldew.Data;
using Coldew.Core.Organization;


namespace Coldew.Core
{
    public class JsonField : Field
    {
        internal JsonField()
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

        public void Modify(string name, bool required, string defaultValue)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = name, Required = required};

            this.OnModifying(args);

            this.Name = name;
            this.Required = required;
            this.DefaultValue = defaultValue;

            this.OnModifyed(args);
        }

    }
}
