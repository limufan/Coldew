using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data;

namespace Coldew.Core.DataProviders
{
    public class MetadataFavoriteDataProvider
    {
        ColdewObject _cobject;
        public MetadataFavoriteDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(User user, Metadata metadata)
        {
            MetadataFavoriteModel model = new MetadataFavoriteModel
            {
                ID = Guid.NewGuid().ToString(),
                MetadataId = metadata.ID,
                UserId = user.ID,
                ObjectId = this._cobject.ID
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

        public Dictionary<User, List<Metadata>> Select()
        {
            Dictionary<User, List<Metadata>> userFavoriteDic = new Dictionary<User, List<Metadata>>();
            IList<MetadataFavoriteModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataFavoriteModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (MetadataFavoriteModel model in models)
            {
                Metadata metadata = this._cobject.MetadataManager.GetById(model.MetadataId);
                User user = this._cobject.ColdewManager.OrgManager.UserManager.GetUserById(model.UserId);

                if (!userFavoriteDic.ContainsKey(user))
                {
                    userFavoriteDic.Add(user, new List<Metadata>());
                }

                userFavoriteDic[user].Add(metadata);
            }
            return userFavoriteDic;
        }
    }
}
