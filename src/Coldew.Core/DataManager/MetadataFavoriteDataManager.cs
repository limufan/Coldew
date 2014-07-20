using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class MetadataFavoriteDataManager
    {
        internal MetadataFavoriteDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public MetadataFavoriteDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new MetadataFavoriteDataProvider(cobject);
            cobject.FavoriteManager.Favorited += MetadataManager_Favorited;
            cobject.FavoriteManager.CancelFavorited += MetadataManager_CancelFavorited;
            cobject.MetadataManager.MetadataDeleted += MetadataManager_MetadataDeleted;
            this.Load();
        }

        void MetadataManager_MetadataDeleted(MetadataManager sender, Metadata args)
        {
            this.DataProvider.Delete(args);
        }

        void MetadataManager_Favorited(User user, Metadata metadata)
        {
            this.DataProvider.Insert(user, metadata);
        }

        void MetadataManager_CancelFavorited(User user, Metadata metadata)
        {
            this.DataProvider.Delete(user, metadata);
        }

        void Load()
        {
            Dictionary<User, List<Metadata>> dic = this.DataProvider.Select();
            this._cobject.FavoriteManager.SetFavoriteDictionary(dic);
        }
    }
}
