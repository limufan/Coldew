using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Permission;
using Coldew.Data;

namespace Coldew.Core.DataProviders
{
    public class MetadataEntityPermissionDataProvider
    {
        ColdewObjectManager _objectManager;
        public MetadataEntityPermissionDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public void Insert(MetadataEntityPermission permission)
        {
            MetadataEntityPermissionModel model = new MetadataEntityPermissionModel();
            model.ObjectId = permission.Metadata.ColdewObject.ID;
            model.MetadataId = permission.Metadata.ID;
            model.Member = permission.Member.Serialize();
            model.Value = (int)permission.Value;
            model.ID = permission.ID;

            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            IList<MetadataEntityPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataEntityPermissionModel>().List();
            foreach (MetadataEntityPermissionModel model in models)
            {
                MetadataEntityPermission permission = this.Create(model);
            }
        }

        private MetadataEntityPermission Create(MetadataEntityPermissionModel model)
        {
            ColdewObject cobject = this._objectManager.GetObjectById(model.ObjectId);
            Metadata metadata = cobject.MetadataManager.GetById(model.MetadataId);
            MetadataMember metadataMember = MetadataMember.Create(model.Member, cobject);
            if (metadataMember != null)
            {
                MetadataEntityPermission permission = new MetadataEntityPermission(model.ID, metadata, metadataMember, (MetadataPermissionValue)model.Value);
                cobject.MetadataPermission.EntityManager.AddPermission(permission);
                return permission;
            }
            return null;
        }
    }
}
