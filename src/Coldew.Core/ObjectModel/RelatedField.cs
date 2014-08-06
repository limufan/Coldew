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
        public RelatedField(RelatedFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public Field RelatedField1 { set; get; }

        public Field ValueField { set; get; }

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
