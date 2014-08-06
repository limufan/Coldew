using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Api;
using System.Threading;

namespace Coldew.Core.Permission
{
    public class FieldPermissionManager
    {
        List<FieldPermission> _permissions;
        protected ReaderWriterLock _lock;
        OrganizationManagement _orgManager;
        ColdewObject _cobject;

        public FieldPermissionManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this._permissions = new List<FieldPermission>();
            this._orgManager = cobject.ColdewManager.OrgManager;
            this._lock = new ReaderWriterLock();
        }

        public FieldPermissionValue GetPermission(User user, Field field)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                List<FieldPermission> permissions = this._permissions.Where(x => x.Field == field).ToList();
                if (permissions.Count == 0)
                {
                    return FieldPermissionValue.All;
                }

                int value = 0;
                foreach (FieldPermission permission in permissions)
                {
                    if (permission.Member.Contains(user))
                    {
                        value = value | (int)permission.Value;
                    }
                }
                return (FieldPermissionValue)value;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public bool HasValue(User user, FieldPermissionValue value, Field field)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                List<FieldPermission> permissions = this._permissions.Where(x => x.Field == field).ToList();
                if (permissions.Count == 0)
                {
                    return true;
                }

                foreach (FieldPermission permission in permissions)
                {
                    if (permission.HasValue(user, value))
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

        public event TEventHandler<FieldPermissionManager, FieldPermission> Created;

        public FieldPermission Create(Field field, Member member, FieldPermissionValue value)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                FieldPermission permission = new FieldPermission(Guid.NewGuid().ToString(), field, member, value);
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

        public void AddPermission(List<FieldPermission> perms)
        {
            this._permissions.AddRange(perms);
        }

        public void AddPermission(FieldPermission perm)
        {
            this._permissions.Add(perm);
        }
    }
}
