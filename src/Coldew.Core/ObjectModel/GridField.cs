using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class GridField : Field
    {
        public GridField(FieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public List<Field> Fields { set; get; }

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
    }
}
