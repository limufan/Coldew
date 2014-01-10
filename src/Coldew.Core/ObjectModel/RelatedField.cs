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
        public RelatedField(FieldNewInfo info, string relatedFieldCode, string propertyCode)
            :base(info)
        {
            this.RelatedFieldCode = relatedFieldCode;
            this.PropertyCode = propertyCode;
        }

        public string RelatedFieldCode { set; get; }

        public string PropertyCode { set; get; }

        public override string TypeName
        {
            get { return ""; }
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo Map(User user)
        {
            MetadataFieldInfo info = new MetadataFieldInfo();
            this.Fill(info, user);
            return info;
        }
    }
}
