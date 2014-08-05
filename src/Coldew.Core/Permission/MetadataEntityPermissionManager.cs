using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Api;
using System.Threading;

namespace Coldew.Core.Permission
{
    public class MetadataEntityPermissionManager
    {
        List<MetadataEntityPermission> _permissions;
        protected ReaderWriterLock _lock;
        OrganizationManagement _orgManager;
        ColdewObject _cobject;

        public MetadataEntityPermissionManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this._permissions = new List<MetadataEntityPermission>();
            this._orgManager = cobject.ColdewManager.OrgManager;
            this._lock = new ReaderWriterLock();
        }

        public MetadataPermissionValue GetPermission(User user, Metadata metadata)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                int value = 0;
                foreach (MetadataEntityPermission permission in this._permissions)
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
                foreach (MetadataEntityPermission permission in this._permissions)
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

        public event TEventHandler<MetadataEntityPermissionManager, MetadataEntityPermission> Created;

        public MetadataEntityPermission Create(Metadata metadata, MetadataMember member, MetadataPermissionValue value)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                MetadataEntityPermission permission = new MetadataEntityPermission(Guid.NewGuid().ToString(), metadata, member, value);
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

        internal void AddPermission(List<MetadataEntityPermission> perms)
        {
            this._permissions.AddRange(perms);
        }

        internal void AddPermission(MetadataEntityPermission perm)
        {
            this._permissions.Add(perm);
        }
    }
}
