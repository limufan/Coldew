using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Data;

namespace Coldew.Data.DataProviders
{
    public class MetadataFavoriteDataProvider
    {
        ColdewObjectManager _objectManager;
        public MetadataFavoriteDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public void Insert(User user, Metadata metadata)
        {
            MetadataFavoriteModel model = new MetadataFavoriteModel
            {
                ID = Guid.NewGuid().ToString(),
                MetadataId = metadata.ID,
                UserId = user.ID,
                ObjectId = metadata.ColdewObject.ID
            };
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(User user, Metadata metadata)
        {
            MetadataFavoriteModel model = NHibernateHelper.CurrentSession.QueryOver<MetadataFavoriteModel>()
                    .Where(x => x.MetadataId == metadata.ID && x.UserId == user.ID)
                    .SingleOrDefault();
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(Metadata metadata)
        {
            IList<MetadataFavoriteModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataFavoriteModel>()
                .Where(x => x.MetadataId == metadata.ID).List();
            foreach (MetadataFavoriteModel model in models)
            {
                NHibernateHelper.CurrentSession.Delete(model);
                NHibernateHelper.CurrentSession.Flush();
            }
        }

        public void Load()
        {
            IList<MetadataFavoriteModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataFavoriteModel>().List();
            foreach (MetadataFavoriteModel model in models)
            {
                ColdewObject cobject = this._objectManager.GetObjectById(model.ObjectId);
                Metadata metadata = cobject.MetadataManager.GetById(model.MetadataId);
                User user = cobject.ColdewManager.OrgManager.UserManager.GetUserById(model.UserId);
                cobject.FavoriteManager.Add(user, metadata);
            }
        }
    }
}
