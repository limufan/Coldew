using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Permission;
using Coldew.Core.Search;
using Coldew.Data;

namespace Coldew.Core.DataProviders
{
    public class MetadataRelatedPermissionDataProvider
    {
        ColdewObject _cobject;
        public MetadataRelatedPermissionDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(MetadataRelatedPermission permission)
        {
            MetadataRelatedPermissionModel model = new MetadataRelatedPermissionModel();
            model.ObjectId = this._cobject.ID;
            model.FieldId = permission.Field.ID;
            model.ID = permission.ID;
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<MetadataRelatedPermission> Select()
        {
            List<MetadataRelatedPermission> perms = new List<MetadataRelatedPermission>();
            IList<MetadataRelatedPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataRelatedPermissionModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (MetadataRelatedPermissionModel model in models)
            {
                MetadataRelatedPermission permission = this.Create(model);
                if (permission != null)
                {
                    perms.Add(permission);
                }
            }
            return perms;
        }

        private MetadataRelatedPermission Create(MetadataRelatedPermissionModel model)
        {
            Field field = this._cobject.GetFieldById(model.FieldId);
            MetadataRelatedPermission permission = new MetadataRelatedPermission(model.ID, field, this._cobject.MetadataPermission);
            return permission;
        }
    }
}
