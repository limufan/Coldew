﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.Permission
{
    public class MetadataEntityPermission
    {
        public MetadataEntityPermission(string id, Metadata metadata, MetadataMember member, MetadataPermissionValue value)
        {
            this.ID = id;
            this.Metadata = metadata;
            this.Member = member;
            this.Value = value;
        }

        public string ID { private set; get; }

        public Metadata Metadata { private set; get; }

        public MetadataPermissionValue Value { private set; get; }

        public MetadataMember Member { private set; get; }

        public virtual bool HasValue(Metadata metadata, User user, MetadataPermissionValue value)
        {
            return this.Member.Contains(metadata, user) && this.Value.HasFlag(value);
        }
    }
}
