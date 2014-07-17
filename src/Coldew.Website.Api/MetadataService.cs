using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Search;
using Coldew.Core.UI;
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

        public string GetGridJson(string objectId, string account, string filterExpressionJson, int skipCount, int takeCount, string orderBy, out int totalCount)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            MetadataFilter filter = this.ParseMetadataFilter(cobject, filterExpressionJson);
            List<MetadataFilter> searchers = new List<MetadataFilter>();
            if (filter != null)
            {
                searchers.Add(filter);
            }
            List<Metadata> metadatas = cobject.MetadataManager.Search(user, searchers).OrderBy(orderBy).ToList();
            totalCount = metadatas.Count;
            List<JObject> jobjects = metadatas.Skip(skipCount).Take(takeCount)
                .Select(metadata =>
                {
                    JObject jobject = metadata.GetJObject(user);
                    jobject.Add("summary", metadata.GetSummary());
                    return jobject;
                }).ToList();
            return JsonConvert.SerializeObject(jobjects);
        }

        public MetadataGridModel GetMetadataGridModel(string objectId, string gridViewId, string account, string serachExpressionJson,
            int skipCount, int takeCount, string orderBy)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(account);
            MetadataFilter filter = this.ParseMetadataFilter(cobject, serachExpressionJson);
            GridView view = cobject.GridViewManager.GetGridView(gridViewId);
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = view.OrderField.Code;
            }

            List<MetadataFilter> searchers = new List<MetadataFilter>();
            if (filter != null)
            {
                searchers.Add(filter);
            }
            if (view.Filter != null)
            {
                searchers.Add(view.Filter);
            }
            List<Metadata> metadatas = cobject.MetadataManager.Search(user, searchers).OrderBy(orderBy).ToList();
            List<JObject> jobjects = metadatas.Skip(skipCount).Take(takeCount)
                .Select(metadata => {
                    JObject jobject = view.GetJObject(metadata, user);
                    return jobject;
                }).ToList();
            MetadataGridModel model = new MetadataGridModel();
            model.footersJson = JsonConvert.SerializeObject(this.MapFooter(metadatas, view));
            model.totalCount = metadatas.Count;
            model.gridJson = JsonConvert.SerializeObject(jobjects);
            return model;
        }

        public string GetFormJson(string userAccount, string objectId, string meatadataId, string formId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Metadata metadata = cobject.MetadataManager.GetById(meatadataId);
            Form form = cobject.FormManager.GetFormById(formId);
            if (metadata != null)
            {
                return JsonConvert.SerializeObject(form.GetJObject(metadata, user));
            }
            return null;
        }

        private List<JObject> MapFooter(List<Metadata> metadatas, GridView view)
        {
            List<JObject> footerJObject = new List<JObject>();
            if (view.Footer != null)
            {
                foreach (GridFooter footerInfo in view.Footer)
                {
                    JObject footerColumn = new JObject();
                    footerColumn.Add("columnName", footerInfo.FieldCode);
                    footerColumn.Add("valueType", "fixed");
                    footerColumn.Add("value", footerInfo.Value);
                    if (footerInfo.ValueType == GridViewFooterValueType.Sum)
                    {
                        footerColumn["value"] = metadatas.Sum(x =>
                        {
                            decimal value = 0;
                            MetadataValue prop = x.GetValue(footerInfo.FieldCode);
                            if (prop != null)
                            {
                                NumberMetadataValue metadataValue = prop as NumberMetadataValue;
                                if (metadataValue != null && metadataValue.Number.HasValue)
                                {
                                    value = metadataValue.Number.Value;
                                }
                            }
                            return value;
                        });
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
            foreach (MetadataValue value in metadata.GetValue(user))
            {
                jobject.Add(value.Field.Code, value.JTokenValue);
            }
            
            return jobject;
        }

        private MetadataFilter ParseMetadataFilter(ColdewObject cobject, string searchJson)
        {
            if (string.IsNullOrEmpty(searchJson))
            {
                return null;
            }

            List<FilterExpression> expressions = new List<FilterExpression>();
            JObject jObject = JsonConvert.DeserializeObject<JObject>(searchJson);
            foreach (JProperty jProperty in jObject.Properties())
            {
                Field field = cobject.GetFieldByCode(jProperty.Name);
                if (field == null)
                {
                    continue;
                }
                switch (field.Type)
                {
                    case FieldType.Number:
                        if (jProperty.Value.Type != JTokenType.Null)
                        {
                            decimal? max = null;
                            decimal? min = null;
                            decimal decimalOutput;
                            if (decimal.TryParse(jProperty.Value["max"].ToString(), out decimalOutput))
                            {
                                max = decimalOutput;
                            }
                            if (decimal.TryParse(jProperty.Value["min"].ToString(), out decimalOutput))
                            {
                                min = decimalOutput;
                            }
                            expressions.Add(new NumberFilterExpression(field, min, max));
                        }
                        break;
                    case FieldType.Date:
                        if (jProperty.Value.Type != JTokenType.Null)
                        {
                            DateTime? start = (DateTime?)jProperty.Value["start"];
                            DateTime? end = (DateTime?)jProperty.Value["end"];
                            if (start.HasValue || end.HasValue)
                            {
                                expressions.Add(new DateFilterExpression(field, start, end));
                                break;
                            }
                        }
                        break;
                    default:
                        string keywordPropertyValue = jProperty.Value.ToString();
                        expressions.Add(new StringFilterExpression(field, keywordPropertyValue));
                        break;
                }
            }
            string keyword = "";
            if (jObject["keyword"] != null)
            {
                keyword = jObject["keyword"].ToString();
                expressions.Add(new KeywordFilterExpression(keyword));
            }
            MetadataFilter filter = new MetadataFilter(expressions);
            return filter;
        }

        public string Create(string objectId, string opUserAccount, string propertyJson)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
            MetadataCreateInfo createInfo = new MetadataCreateInfo(){ Creator = opUser, JObject = JsonConvert.DeserializeObject<JObject>(propertyJson) };
            Metadata metadata = cobject.MetadataManager.Create(createInfo);
            return JsonConvert.SerializeObject(metadata.GetJObject(opUser));
        }

        public void Modify(string objectId, string opUserAccount, string metadataId, string propertyJson)
        {
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
            Metadata metadata = cobject.MetadataManager.GetById(metadataId);
            metadata.SetValue(opUser, JsonConvert.DeserializeObject<JObject>(propertyJson));
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

        public string Import(string opUserAccount, string objectId, string importModelsJson)
        {
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
            ColdewObject coldewObject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            List<JObject> importModels = JsonConvert.DeserializeObject<List<JObject>>(importModelsJson);
            foreach (JObject model in importModels)
            {
                JObject propertysObject = new JObject();
                List<Field> fields = coldewObject.GetFields();
                foreach (Field field in fields)
                {
                    if (model[field.Code] != null)
                    {
                        if (field.Type == FieldType.Metadata)
                        {
                            Metadata metadata = coldewObject.MetadataManager.GetByName(model[field.Code].ToString());
                            propertysObject.Add(field.Code, metadata.ID);
                        }
                        else
                        {
                            propertysObject.Add(field.Code, model[field.Code]);
                        }
                    }
                }
                try
                {
                    MetadataCreateInfo createInfo = new MetadataCreateInfo() { Creator = opUser, JObject = propertysObject };
                    coldewObject.MetadataManager.Create(createInfo);
                    model["importResult"] = true;
                    model["importMessage"] = "导入成功";
                }
                catch (Exception ex)
                {
                    this._coldewManager.Logger.Error(ex.Message, ex);
                    model["importResult"] = false;
                    model["importMessage"] = "导入失败" + ex.Message;
                }
            }
            return JsonConvert.SerializeObject(importModels);
        }
    }
}
