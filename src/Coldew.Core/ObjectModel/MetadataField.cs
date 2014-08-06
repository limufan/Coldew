using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;
using Coldew.Api.Exceptions;

namespace Coldew.Core
{
    public class MetadataField: Field
    {
        public MetadataField(MetadataFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        void MetadataManager_MetadataDeleting(MetadataManager sender, Metadata args)
        {
            List<Metadata> relatedMetadatas = this.ColdewObject.MetadataManager.GetRelatedList(args, "");
            if (relatedMetadatas != null && relatedMetadatas.Count > 0)
            {
                string metadataNames = string.Join(",", relatedMetadatas.Select(x => x.Name));
                throw new ColdewException(string.Format("该{0}已经被{1}:{2} 关联，无法删除!", this.RelatedObject.Name, this.ColdewObject.Name, metadataNames));
            }
        }

        public ColdewObject RelatedObject { set; get; }

        public override string Type
        {
            get { return FieldType.Metadata; }
        }

        public override string TypeName
        {
            get { return this.RelatedObject.Name; }
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string metadataId = "";
            if (value.Type == JTokenType.Object)
            {
                if (value["id"] == null)
                {
                    throw new ArgumentException("value 不包含id属性");
                }
                metadataId = value["id"].ToString();
            }
            else
            {
                metadataId = value.ToString();
            }
            return new MetadataRelatedValue(metadataId, this);
        }
    }
}
