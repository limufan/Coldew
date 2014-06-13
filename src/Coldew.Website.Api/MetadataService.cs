using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Search;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coldew.Website.Api
{
    public class MetadataService : Coldew.Website.Api.IMetadataService
    {
        ColdewManager _coldewManager;

        public MetadataService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public string GetEditJson(string userAccount, string objectId, string meatadataId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Metadata metadata = cobject.MetadataManager.GetById(meatadataId);
            if (metadata != null)
            {
                return JsonConvert.SerializeObject(this.MapEditJObject(metadata, user));
            }
            return null;
        }

        public string GetDetailsJson(string userAccount, string objectId, string meatadataId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Metadata metadata = cobject.MetadataManager.GetById(meatadataId);
            if (metadata != null)
            {
                return JsonConvert.SerializeObject(this.MapDetailsJObject(metadata, user));
            }
            return null;
        }

        public string GetGridJson(string objectId, string account, int skipCount, int takeCount, string orderBy, out int totalCount)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            List<JObject> jobjects = cobject.MetadataManager
                .GetList(user, skipCount, takeCount, orderBy, out totalCount)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects);
        }

        public string GetGridJson(string objectId, string account, string orderBy)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            List<JObject> jobjects = cobject.MetadataManager
                .GetList(user, orderBy)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects);
        }

        public string GetGridJson(string objectId, string gridViewId, string account, string orderBy)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            GridView view = cobject.GridViewManager.GetGridView(gridViewId);
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = view.OrderFieldCode;
            }

            List<MetadataSearcher> searchers = new List<MetadataSearcher>();
            if (view.Searcher != null)
            {
                searchers.Add(view.Searcher);
            }
            List<JObject> jobjects = cobject.MetadataManager
                .Search(user, searchers, orderBy)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects);
        }

        private JObject MapGridJObject(Metadata metadata, User user)
        {
            MetadataPermissionValue permission = metadata.ColdewObject.MetadataPermission.GetValue(user, metadata);
            bool favorited = metadata.ColdewObject.FavoriteManager.IsFavorite(user, metadata);
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            foreach (MetadataProperty property in metadata.GetPropertys(user))
            {
                jobject.Add(property.Field.Code, property.Value.ShowValue);
            }
            jobject.Add("summary", metadata.GetSummary());

            jobject.Add("favorited", favorited);
            jobject.Add("canModify", permission.HasFlag(MetadataPermissionValue.Modify));
            jobject.Add("canDelete", permission.HasFlag(MetadataPermissionValue.Delete));
            return jobject;
        }

        private List<JObject> MapFooter(List<Metadata> metadatas, GridView view)
        {
            List<JObject> footerJObject = new List<JObject>();
            if (view.Footer != null)
            {
                foreach (GridViewFooterInfo footerInfo in view.Footer)
                {
                    JObject footerColumn = new JObject();
                    footerColumn.Add("columnName", footerInfo.FieldCode);
                    footerColumn.Add("valueType", "fixed");
                    footerColumn.Add("value", footerInfo.Value);
                    if (footerInfo.ValueType == GridViewFooterValueType.Sum)
                    {
                        footerColumn.Add("value", metadatas.Sum(x =>
                        {
                            decimal value = 0;
                            MetadataProperty prop = x.GetProperty(footerInfo.FieldCode);
                            if (prop != null)
                            {
                                NumberMetadataValue metadataValue = prop.Value as NumberMetadataValue;
                                if (metadataValue != null && metadataValue.Number.HasValue)
                                {
                                    value = metadataValue.Number.Value;
                                }
                            }
                            return value;
                        }));
                    }
                    footerJObject.Add(footerColumn);
                }
            }
            return footerJObject;
        }

        private JObject MapEditJObject(Metadata metadata, User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            foreach (MetadataProperty property in metadata.GetPropertys(user))
            {
                jobject.Add(property.Field.Code, property.Value.JTokenValue);
            }
            
            return jobject;
        }

        private JObject MapDetailsJObject(Metadata metadata, User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            foreach (MetadataProperty property in metadata.GetPropertys(user))
            {
                jobject.Add(property.Field.Code, property.Value.ShowValue);
            }

            return jobject;
        }

        public string GetGridJsonBySerach(string objectId, string account, string serachExpressionJson, int skipCount, int takeCount, string orderBy, out int totalCount)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            MetadataExpressionSearcher searcher = MetadataExpressionSearcher.Parse(serachExpressionJson, cobject);

            List<MetadataSearcher> searchers = new List<MetadataSearcher>();
            searchers.Add(searcher);
            List<Metadata> metadatas = cobject.MetadataManager.Search(user, searchers).OrderBy(orderBy).ToList();
            totalCount = metadatas.Count;
            List<JObject> jobjects = metadatas
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects); 
        }

        public MetadataGridModel GetMetadataGridModel(string objectId, string gridViewId, string account, string serachExpressionJson, int skipCount, int takeCount, string orderBy)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            MetadataExpressionSearcher searcher = MetadataExpressionSearcher.Parse(serachExpressionJson, cobject);
            GridView view = cobject.GridViewManager.GetGridView(gridViewId);
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = view.OrderFieldCode;
            }

            List<MetadataSearcher> searchers = new List<MetadataSearcher>();
            if(searcher != null)
            {
                searchers.Add(searcher);
            }
            if (view.Searcher != null)
            {
                searchers.Add(view.Searcher);
            }
            List<Metadata> metadatas = cobject.MetadataManager.Search(user, searchers).OrderBy(orderBy).ToList();
            List<JObject> jobjects = metadatas.Skip(skipCount).Take(takeCount)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            MetadataGridModel model = new MetadataGridModel();
            model.footersJson = JsonConvert.SerializeObject(this.MapFooter(metadatas, view));
            model.totalCount = metadatas.Count;
            model.gridJson = JsonConvert.SerializeObject(jobjects);
            return model;
        }

        public string GetGridJsonBySerach(string objectId, string gridViewId, string account, string serachExpressionJson, string orderBy)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            MetadataExpressionSearcher searcher = MetadataExpressionSearcher.Parse(serachExpressionJson, cobject);
            GridView view = cobject.GridViewManager.GetGridView(gridViewId);
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = view.OrderFieldCode;
            }

            List<MetadataSearcher> searchers = new List<MetadataSearcher>();
            searchers.Add(searcher);
            if (view.Searcher != null)
            {
                searchers.Add(view.Searcher);
            }
            List<JObject> jobjects = cobject.MetadataManager.Search(user, searchers, orderBy)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects);
        }


        public string GetGridJsonBySerach(string objectId, string account, string serachExpressionJson, string orderBy)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            MetadataExpressionSearcher searcher = MetadataExpressionSearcher.Parse(serachExpressionJson, cobject);

            List<MetadataSearcher> searchers = new List<MetadataSearcher>();
            searchers.Add(searcher);
            List<JObject> jobjects = cobject.MetadataManager.Search(user, searchers, orderBy)
                .Select(metadata => this.MapGridJObject(metadata, user)).ToList();
            return JsonConvert.SerializeObject(jobjects); 
        }



        public string Create(string objectId, string opUserAccount, string propertyJson)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);

            Metadata metadata = cobject.MetadataManager.Create(opUser, JsonConvert.DeserializeObject<JObject>(propertyJson));
            return JsonConvert.SerializeObject(this.MapEditJObject(metadata, opUser));
        }

        public void Modify(string objectId, string opUserAccount, string metadataId, string propertyJson)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
            Metadata metadata = cobject.MetadataManager.GetById(metadataId);
            metadata.SetPropertys(opUser, JsonConvert.DeserializeObject<JObject>(propertyJson));
        }

        public void Delete(string objectId, string opUserAccount, List<string> metadataIds)
        {
            if (metadataIds == null)
            {
                return;
            }
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            foreach (string metadataId in metadataIds)
            {
                User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
                Metadata metadata = cobject.MetadataManager.GetById(metadataId);
                metadata.Delete(opUser);
            }
        }

        public void ToggleFavorite(string objectId, string opUserAccount, List<string> metadataIds)
        {
            if (metadataIds == null)
            {
                return;
            }
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            foreach (string metadataId in metadataIds)
            {
                User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
                Metadata metadata = cobject.MetadataManager.GetById(metadataId);

                if (cobject.FavoriteManager.IsFavorite(user, metadata))
                {
                    cobject.FavoriteManager.CancelFavorite(user, metadata);
                }
                else
                {
                    cobject.FavoriteManager.Favorite(user, metadata);
                }
            }
        }

        public MetadataInfo GetMetadataById(string userAccount, string objectId, string id)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Metadata metadata = cobject.MetadataManager.GetById(id);
            if (metadata != null)
            {
                return metadata.Map(user);
            }
            return null;
        }

        public MetadataInfo GetMetadataByName(string userAccount, string objectId, string name)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Metadata metadata = cobject.MetadataManager.GetByName(name);
            if (metadata != null)
            {
                return metadata.Map(user);
            }
            return null;
        }

        public List<MetadataInfo> GetRelatedMetadatas(string userAccount, string relatedObjectId, string objectId, string metadataId, string orderBy)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject relatedObject = this._coldewManager.ObjectManager.GetObjectById(relatedObjectId);
            ColdewObject cObject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            return relatedObject.MetadataManager.GetRelatedList(cObject, metadataId, orderBy).Select(x => x.Map(user)).ToList();
        }
    }
}
