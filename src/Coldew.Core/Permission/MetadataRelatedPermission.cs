using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.Permission
{
    public class MetadataRelatedPermission
    {
        MetadataPermissionManager _metadataPermission;
        public MetadataRelatedPermission(string id, Field field, MetadataPermissionManager metadataPermission)
        {
            this.ID = id;
            this.Field = field;
            this._metadataPermission = metadataPermission;
        }

        public string ID { private set; get; }

        public Field Field { private set; get; }

        public virtual bool HasValue(Metadata metadata, User user, MetadataPermissionValue value)
        {
            MetadataValue relatedValue = metadata.GetValue(this.Field.Code);
            if (relatedValue != null && relatedValue is MetadataRelatedValue)
            {
                MetadataRelatedValue metadataValue = relatedValue as MetadataRelatedValue;
                return metadataValue.Metadata.ColdewObject.MetadataPermission.HasValue(user, value, metadataValue.Metadata);
            }
            return false;
        }

        public virtual MetadataPermissionValue GetPermission(User user, Metadata metadata)
        {
            MetadataValue relatedValue = metadata.GetValue(this.Field.Code);
            if (relatedValue != null && relatedValue is MetadataRelatedValue)
            {
                MetadataRelatedValue metadataValue = relatedValue as MetadataRelatedValue;
                return metadataValue.Metadata.ColdewObject.MetadataPermission.GetValue(user, metadataValue.Metadata);
            }
            return MetadataPermissionValue.None;
        }
    }
}
