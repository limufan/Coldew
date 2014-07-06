using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class GridField : Field
    {
        public GridField()
        {

        }

        public List<Field> Fields { internal set; get; }

        public override string Type
        {
            get { return ""; }
        }

        public override string TypeName
        {
            get { return "Grid"; }
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new JsonMetadataValue(value, this);
        }

        public void Modify(string name, bool required)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = name, Required = required};

            this.OnModifying(args);

            this.Name = name;
            this.Required = required;

            this.OnModifyed(args);
        }
    }
}
