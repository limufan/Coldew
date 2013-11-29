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
        public MetadataRelatedPermission(string id, string fieldCode, MetadataPermissionManager metadataPermission)
        {
            this.ID = id;
            this.FieldCode = fieldCode;
            this._metadataPermission = metadataPermission;
        }

        public string ID { private set; get; }

        public string FieldCode { private set; get; }

        public virtual bool HasValue(Metadata metadata, User user, MetadataPermissionValue value)
        {
            MetadataProperty relatedProperty = metadata.GetProperty(this.FieldCode);
            if (relatedProperty != null && relatedProperty.Value is MetadataRelatedValue)
            {
                MetadataRelatedValue metadataValue = relatedProperty.Value as MetadataRelatedValue;
                return metadataValue.Metadata.ColdewObject.MetadataPermission.HasValue(user, value, metadataValue.Metadata);
            }
            return false;
        }

        public virtual MetadataPermissionValue GetPermission(User user, Metadata metadata)
        {
            MetadataProperty relatedProperty = metadata.GetProperty(this.FieldCode);
            if (relatedProperty != null && relatedProperty.Value is MetadataRelatedValue)
            {
                MetadataRelatedValue metadataValue = relatedProperty.Value as MetadataRelatedValue;
                return metadataValue.Metadata.ColdewObject.MetadataPermission.GetValue(user, metadataValue.Metadata);
            }
            return MetadataPermissionValue.None;
        }
    }
}
