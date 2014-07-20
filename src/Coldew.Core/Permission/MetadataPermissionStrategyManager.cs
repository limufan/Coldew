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
    public class MetadataPermissionStrategyManager
    {
        List<MetadataPermissionStrategy> _permissions;
        protected ReaderWriterLock _lock;
        OrganizationManagement _orgManager;
        ColdewManager _coldewManager;
        ColdewObject _cobject;

        public MetadataPermissionStrategyManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this._permissions = new List<MetadataPermissionStrategy>();
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
                foreach (MetadataPermissionStrategy permission in this._permissions)
                {
                    if (permission.Member.Contains(metadata, user))
                    {
                        value = value | (int)permission.Value;
                    }
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
                foreach (MetadataPermissionStrategy permission in this._permissions)
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

        public event TEventHandler<MetadataPermissionStrategyManager, MetadataPermissionStrategy> Created;

        public MetadataPermissionStrategy Create(MetadataMember member, MetadataPermissionValue value, MetadataFilter filter)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                MetadataPermissionStrategy permission = new MetadataPermissionStrategy(Guid.NewGuid().ToString(), this._cobject.ID, member, value, filter);
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

        internal void AddPermission(List<MetadataPermissionStrategy> perms)
        {
            this._permissions.AddRange(perms);
        }
    }
}
