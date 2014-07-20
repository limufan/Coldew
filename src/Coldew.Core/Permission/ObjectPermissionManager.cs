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
    public class ObjectPermissionManager
    {
        List<ObjectPermission> _permissions;
        protected ReaderWriterLock _lock;
        ColdewObject _coldewObject;

        public ObjectPermissionManager(ColdewObject coldewObject)
        {
            this._coldewObject = coldewObject;
            this._permissions = new List<ObjectPermission>();
            this._lock = new ReaderWriterLock();
        }

        public ObjectPermissionValue GetPermission(User user)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                int value = 0;
                foreach (ObjectPermission permission in this._permissions)
                {
                    if (permission.Member.Contains(user))
                    {
                        value = value | (int)permission.Value;
                    }
                }
                return (ObjectPermissionValue)value;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public bool HasValue(User user, ObjectPermissionValue value)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                foreach (ObjectPermission permission in this._permissions)
                {
                    if (permission.Member.Contains(user) && permission.Value.HasFlag(value))
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

        public event TEventHandler<ObjectPermissionManager, ObjectPermission> Created;

        public ObjectPermission Create(Member member, ObjectPermissionValue value)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                ObjectPermission permission = new ObjectPermission(Guid.NewGuid().ToString(), member, this._coldewObject.ID, value);
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

        internal void AddPermission(List<ObjectPermission> perms)
        {
            this._permissions.AddRange(perms);
        }
    }
}
