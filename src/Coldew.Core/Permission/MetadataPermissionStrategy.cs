using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Search;
using Coldew.Api;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.Permission
{
    public class MetadataPermissionStrategy
    {
        public MetadataPermissionStrategy(string id, string objectId, MetadataMember member, MetadataPermissionValue value, MetadataFilter filter)
        {
            this.ID = id;
            this.ObjectId = objectId; 
            this.Member = member;
            this.Value = value;
            this.Filter = filter;
        }

        public string ID { private set; get; }

        public string ObjectId { private set; get; }

        public MetadataPermissionValue Value { private set; get; }

        public MetadataMember Member { private set; get; }

        public MetadataFilter Filter { private set; get; }

        public virtual bool HasValue(Metadata metadata, User user, MetadataPermissionValue value)
        {
            if (this.Filter == null)
            {
                return this.Value.HasFlag(value) && this.Member.Contains(metadata, user);
            }
            else
            {
                return this.Value.HasFlag(value) && this.Member.Contains(metadata, user) && this.Filter.Accord(user, metadata);
            }
        }
    }
}
