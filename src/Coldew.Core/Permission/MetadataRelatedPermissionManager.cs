using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using System.Threading;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Api;
using Coldew.Core.Search;

namespace Coldew.Core.Permission
{
    public class MetadataRelatedPermissionManager
    {
        List<MetadataRelatedPermission> _permissions;
        protected ReaderWriterLock _lock;
        OrganizationManagement _orgManager;
        ColdewManager _coldewManager;
        ColdewObject _cobject;

        public MetadataRelatedPermissionManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this._permissions = new List<MetadataRelatedPermission>();
            this._orgManager = cobject.ColdewManager.OrgManager;
            this._coldewManager = cobject.ColdewManager;
            this._lock = new ReaderWriterLock();
        }

        public MetadataPermissionValue GetPermission(User user, Metadata metadata)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                int value = 0;
                foreach (MetadataRelatedPermission permission in this._permissions)
                {
                    value = value | (int)permission.GetPermission(user, metadata);
                }
                return (MetadataPermissionValue)value;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public bool HasValue(User user, MetadataPermissionValue value, Metadata metadata)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                foreach (MetadataRelatedPermission permission in this._permissions)
                {
                    if (permission.HasValue(metadata, user, value))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public event TEventHandler<MetadataRelatedPermissionManager, MetadataRelatedPermission> Created;

        public MetadataRelatedPermission Create(Field field)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                MetadataRelatedPermission permission = new MetadataRelatedPermission(Guid.NewGuid().ToString(), field);
                this._permissions.Add(permission);
                if (this.Created != null)
                {
                    this.Created(this, permission);
                }
                return permission;
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        internal void AddPermission(List<MetadataRelatedPermission> pemrs)
        {
            this._permissions.AddRange(pemrs);
        }

        internal void AddPermission(MetadataRelatedPermission pemr)
        {
            this._permissions.Add(pemr);
        }
    }
}
