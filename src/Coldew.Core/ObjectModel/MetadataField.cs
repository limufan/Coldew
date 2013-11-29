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
        public MetadataField(FieldNewInfo info, ColdewObject relatedObject)
            :base(info)
        {
            this.RelatedObject = relatedObject;
            this.RelatedObject.MetadataManager.MetadataDeleting += MetadataManager_MetadataDeleting;
        }

        void MetadataManager_MetadataDeleting(MetadataManager sender, Metadata args)
        {
            List<Metadata> relatedMetadatas = this.ColdewObject.MetadataManager.GetRelatedList(this.RelatedObject, args.ID, "");
            if (relatedMetadatas != null && relatedMetadatas.Count > 0)
            {
                string metadataNames = string.Join(",", relatedMetadatas.Select(x => x.Name));
                throw new ColdewException(string.Format("该{0}已经被{1}:{2} 关联，无法删除!", this.RelatedObject.Name, this.ColdewObject.Name, metadataNames));
            }
        }

        public ColdewObject RelatedObject { private set;get;}

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
            return new MetadataRelatedValue(value.ToString(), this);
        }

        public override FieldInfo Map(User user)
        {
            MetadataFieldInfo info = new MetadataFieldInfo();
            this.Fill(info, user);
            info.ValueFormId = this.RelatedObject.ID;
            info.ValueFormName = this.RelatedObject.Name;
            return info;
        }
    }
}
