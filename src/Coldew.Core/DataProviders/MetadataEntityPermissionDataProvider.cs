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
        ColdewObject _cobject;
        public MetadataEntityPermissionDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(MetadataEntityPermission permission)
        {
            MetadataEntityPermissionModel model = new MetadataEntityPermissionModel();
            model.ObjectId = this._cobject.ID;
            model.MetadataId = permission.MetadataId;
            model.Member = permission.Member.Serialize();
            model.Value = (int)permission.Value;
            model.ID = permission.ID;

            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<MetadataEntityPermission> Select()
        {
            List<MetadataEntityPermission> perms = new List<MetadataEntityPermission>();
            IList<MetadataEntityPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataEntityPermissionModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (MetadataEntityPermissionModel model in models)
            {
                MetadataEntityPermission permission = this.Create(model);
                if (permission != null)
                {
                    perms.Add(permission);
                }
            }
            return perms;
        }

        private MetadataEntityPermission Create(MetadataEntityPermissionModel model)
        {
            MetadataMember metadataMember = MetadataMember.Create(model.Member, this._cobject);
            if (metadataMember != null)
            {
                MetadataEntityPermission permission = new MetadataEntityPermission(model.ID, model.MetadataId, metadataMember, (MetadataPermissionValue)model.Value);
                return permission;

            }
            return null;
        }
    }
}
