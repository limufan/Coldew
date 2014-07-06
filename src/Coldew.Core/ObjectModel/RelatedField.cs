using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;


namespace Coldew.Core
{
    public class RelatedField: Field
    {
        internal RelatedField()
        {

        }

        public string RelatedFieldCode { set; get; }

        public string PropertyCode { set; get; }

        public override string Type
        {
            get { return FieldType.RelatedField; }
        }

        public override string TypeName
        {
            get { return ""; }
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            throw new NotImplementedException();
        }
    }
}
