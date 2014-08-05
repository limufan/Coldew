using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data;
using Coldew.Core;
using System.Threading;

namespace Coldew.Core
{
    public class MetadataFavoriteManager
    {
        ColdewObject _cobject;
        OrganizationManagement _orgManger;
        Dictionary<User, List<Metadata>> _userFavoriteDic;
        Dictionary<Metadata, Metadata> _bindedEventDic;
        ReaderWriterLock _lock;

        public MetadataFavoriteManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this._orgManger = _cobject.ColdewManager.OrgManager;
            this._userFavoriteDic = new Dictionary<User, List<Metadata>>();
            this._bindedEventDic = new Dictionary<Metadata, Metadata>();
            this._lock = new ReaderWriterLock();
        }

        public bool IsFavorite(User user, Metadata metadata)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                if (!this._userFavoriteDic.ContainsKey(user))
                {
                    return false;
                }
                if (this._userFavoriteDic[user].Contains(metadata))
                {
                    return true;
                }
                return false;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public event TEventHandler<User, Metadata> Favorited;

        public void Favorite(User user, Metadata metadata)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                if (!this._userFavoriteDic.ContainsKey(user))
                {
                    this._userFavoriteDic.Add(user, new List<Metadata>());
                }
                if (this._userFavoriteDic[user].Contains(metadata))
                {
                    return;
                }

                this._userFavoriteDic[user].Add(metadata);
                this.BindCustomerEvent(metadata);

                if (this.Favorited != null)
                {
                    this.Favorited(user, metadata);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        private void BindCustomerEvent(Metadata metadata)
        {
            if (!this._bindedEventDic.ContainsKey(metadata))
            {
                this._bindedEventDic.Add(metadata, metadata);
                metadata.Deleted += new TEventHandler<Metadata, User>(Metadata_Deleted);
            }
        }

        void Metadata_Deleted(Metadata metadata, User opUser)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                foreach (KeyValuePair<User, List<Metadata>> pair in _userFavoriteDic)
                {
                    pair.Value.Remove(metadata);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public event TEventHandler<User, Metadata> CancelFavorited;

        public void CancelFavorite(User user, Metadata metadata)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                if (!this._userFavoriteDic.ContainsKey(user))
                {
                    return;
                }
                if (!this._userFavoriteDic[user].Contains(metadata))
                {
                    return;
                }

                this._userFavoriteDic[user].Remove(metadata);

                if (this.CancelFavorited != null)
                {
                    this.CancelFavorited(user, metadata);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public List<Metadata> GetFavorites(User user, int skipCount, int takeCount, string orderBy, out int totalCount)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                totalCount = 0;
                if (!this._userFavoriteDic.ContainsKey(user))
                {
                    return new List<Metadata>();
                }
                totalCount = this._userFavoriteDic[user].Count;
                return this._userFavoriteDic[user].OrderBy(orderBy).Skip(skipCount).Take(takeCount).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Metadata> GetFavorites(User user, string orderBy)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                if (!this._userFavoriteDic.ContainsKey(user))
                {
                    return new List<Metadata>();
                }
                return this._userFavoriteDic[user].OrderBy(orderBy).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public void SetFavoriteDictionary(Dictionary<User, List<Metadata>> dic)
        {
            this._userFavoriteDic = dic;
        }

        public void Add(User user, Metadata metadata)
        {
            if (!this._userFavoriteDic.ContainsKey(user))
            {
                this._userFavoriteDic.Add(user, new List<Metadata>());
            }

            this._userFavoriteDic[user].Add(metadata);
        }
    }
}
