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
    public class MetadataStrategyPermissionDataProvider
    {
        ColdewObjectManager _objectManager;
        public MetadataStrategyPermissionDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public void Insert(MetadataPermissionStrategy permission)
        {
            MetadataPermissionStrategyModel model = new MetadataPermissionStrategyModel();
            model.ObjectId = permission.ColdewObject.ID;
            model.Member = permission.Member.Serialize();
            model.Value = (int)permission.Value;
            if (permission.Filter != null)
            {
                model.FilterJson = permission.Filter.ToString();
            }
            model.ID = permission.ID;
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            IList<MetadataPermissionStrategyModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataPermissionStrategyModel>().List();
            foreach (MetadataPermissionStrategyModel model in models)
            {
                MetadataPermissionStrategy permission = this.Create(model);
            }
        }

        private MetadataPermissionStrategy Create(MetadataPermissionStrategyModel model)
        {
            ColdewObject cobject = this._objectManager.GetObjectById(model.ObjectId);
            MetadataMember metadataMember = MetadataMember.Create(model.Member, cobject);
            if (metadataMember != null)
            {
                MetadataFilter filter = null;
                if (!string.IsNullOrEmpty(model.FilterJson))
                {
                    MetadataFilterParser parser = new MetadataFilterParser(model.FilterJson, cobject);
                    filter = parser.Parse();
                }
                MetadataPermissionStrategy permission = new MetadataPermissionStrategy(model.ID, cobject, metadataMember, (MetadataPermissionValue)model.Value, filter);
                cobject.MetadataPermission.StrategyManager.AddPermission(permission);
                return permission;
            }

            return null;
        }
    }
}
