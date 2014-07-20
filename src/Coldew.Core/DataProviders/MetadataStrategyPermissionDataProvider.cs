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
        ColdewObject _cobject;
        public MetadataStrategyPermissionDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(MetadataPermissionStrategy permission)
        {
            MetadataPermissionStrategyModel model = new MetadataPermissionStrategyModel();
            model.ObjectId = this._cobject.ID;
            model.Member = permission.Member.Serialize();
            model.Value = (int)permission.Value;
            model.FilterJson = permission.Filter.ToString();
            model.ID = permission.ID;
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<MetadataPermissionStrategy> Select()
        {
            List<MetadataPermissionStrategy> perms = new List<MetadataPermissionStrategy>();
            IList<MetadataPermissionStrategyModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataPermissionStrategyModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (MetadataPermissionStrategyModel model in models)
            {
                MetadataPermissionStrategy permission = this.Create(model);
                if (permission != null)
                {
                    perms.Add(permission);
                }
            }
            return perms;
        }

        private MetadataPermissionStrategy Create(MetadataPermissionStrategyModel model)
        {
            MetadataMember metadataMember = MetadataMember.Create(model.Member, this._cobject);
            if (metadataMember != null)
            {
                MetadataFilter filter = null;
                if (!string.IsNullOrEmpty(model.FilterJson))
                {
                    MetadataFilterParser parser = new MetadataFilterParser(model.FilterJson, this._cobject);
                    filter = parser.Parse();
                }
                MetadataPermissionStrategy permission = new MetadataPermissionStrategy(model.ID, model.ObjectId, metadataMember, (MetadataPermissionValue)model.Value, filter);
                return permission;
            }

            return null;
        }
    }
}
