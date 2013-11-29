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

        public MetadataRelatedPermission Create(string fieldCode)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                MetadataRelatedPermissionModel model = new MetadataRelatedPermissionModel();
                model.ObjectId = this._cobject.ID;
                model.FieldCode = fieldCode;

                model.ID = NHibernateHelper.CurrentSession.Save(model).ToString();
                NHibernateHelper.CurrentSession.Flush();

                return this.Create(model);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        private MetadataRelatedPermission Create(MetadataRelatedPermissionModel model)
        {
            MetadataRelatedPermission permission = new MetadataRelatedPermission(model.ID, model.FieldCode, this._cobject.MetadataPermission);
            this._permissions.Add(permission);
            return permission;
        }

        internal void Load()
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                IList<MetadataRelatedPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataRelatedPermissionModel>().Where(x => x.ObjectId == this._cobject.ID).List();
                foreach (MetadataRelatedPermissionModel model in models)
                {
                    this.Create(model);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }
    }
}
