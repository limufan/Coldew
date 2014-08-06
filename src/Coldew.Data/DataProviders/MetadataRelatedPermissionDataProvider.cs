using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Permission;
using Coldew.Core.Search;
using Coldew.Data;

namespace Coldew.Data.DataProviders
{
    public class MetadataRelatedPermissionDataProvider
    {
        ColdewObjectManager _objectManager;
        public MetadataRelatedPermissionDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public void Insert(MetadataRelatedPermission permission)
        {
            MetadataRelatedPermissionModel model = new MetadataRelatedPermissionModel();
            model.FieldId = permission.Field.ID;
            model.ID = permission.ID;
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            IList<MetadataRelatedPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataRelatedPermissionModel>().List();
            foreach (MetadataRelatedPermissionModel model in models)
            {
                MetadataRelatedPermission permission = this.Create(model);
            }
        }

        private MetadataRelatedPermission Create(MetadataRelatedPermissionModel model)
        {
            Field field = this._objectManager.GetFieldById(model.FieldId);
            MetadataRelatedPermission permission = new MetadataRelatedPermission(model.ID, field);
            field.ColdewObject.MetadataPermission.RelatedPermission.AddPermission(permission);
            return permission;
        }
    }
}
