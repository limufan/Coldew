﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Api;
using Coldew.Data;
using Coldew.Api.Exceptions;
using Coldew.Core.DataServices;
using Coldew.Core.Permission;

namespace Coldew.Core
{
    public class Metadata
    {
        public Metadata(string id, List<MetadataProperty> propertys, ColdewObject cobject)
        {
            this.ID = id;
            this._propertys = propertys.ToDictionary(x => x.Field.Code);
            this.ColdewObject = cobject;
            this.InitPropertys();
        }

        private void InitPropertys()
        {
            List<MetadataProperty> virtualPropertys = this.GetVirtualPropertys();
            if (virtualPropertys != null)
            {
                foreach (MetadataProperty property in virtualPropertys)
                {
                    if (this._propertys.ContainsKey(property.Field.Code))
                    {
                        this._propertys.Remove(property.Field.Code);
                    }
                    this._propertys.Add(property.Field.Code, property);
                }
            }
        }

        public void RemoveFieldProperty(Field field)
        {
            List<MetadataProperty> propertys = this._propertys.Values.ToList();
            MetadataProperty property = propertys.Find(x => x.Field == field);
            if (property != null)
            {
                propertys.Remove(property);

                this.ColdewObject.DataService.Update(this.ID, propertys);

                this._propertys = propertys.ToDictionary(x => x.Field.Code);
                this.BuildContent();
            }
        }

        protected virtual List<MetadataProperty> GetVirtualPropertys()
        {
            List<MetadataProperty> propertys = new List<MetadataProperty>();
            List<RelatedField> relatedFields = this.ColdewObject.GetRelatedFields();
            foreach(RelatedField relatedField in relatedFields)
            {
                if (this._propertys.ContainsKey(relatedField.RelatedFieldCode))
                {
                    MetadataRelatedValue realtedValue = this._propertys[relatedField.RelatedFieldCode].Value as MetadataRelatedValue;
                    if (realtedValue != null)
                    {
                        MetadataProperty property = realtedValue.Metadata.GetProperty(relatedField.PropertyCode);
                        if (property != null)
                        {
                            propertys.Add(property);
                        }
                    }
                }
            }
            return propertys;
        }

        public string ID { private set; get; }

        public string Name
        {
            get
            {
                StringMetadataValue value = this.GetProperty(this.ColdewObject.NameField.Code).Value as StringMetadataValue;
                return value.String;
            }
        }

        public User Creator
        {
            get
            {
                UserMetadataValue value = this.GetProperty(this.ColdewObject.CreatedUserField.Code).Value as UserMetadataValue;
                return value.User;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                DateMetadataValue value = this.GetProperty(this.ColdewObject.CreatedTimeField.Code).Value as DateMetadataValue;
                return value.Date.Value;
            }
        }

        public ColdewObject ColdewObject { private set; get; }

        string _content;
        public string Content
        {
            private set
            {
                this._content = value;
            }
            get
            {
                if (string.IsNullOrEmpty(this._content))
                {
                    this.BuildContent();
                }
                return this._content;
            }
        }

        protected virtual void BuildContent()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MetadataProperty property in this.GetPropertys())
            {
                if (!string.IsNullOrEmpty(property.Value.ShowValue))
                {
                    sb.Append(property.Value.ShowValue.ToLower());
                }
            }
            this.Content = sb.ToString();
        }

        protected Dictionary<string, MetadataProperty> _propertys;

        public event TEventHandler<Metadata, JObject> PropertyChanging;
        public event TEventHandler<Metadata, JObject> PropertyChanged;

        protected virtual void OnPropertyChanging(JObject dictionary)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, dictionary);
            }
        }

        protected virtual void OnPropertyChanged(JObject dictionary)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, dictionary);
            }
        }

        public virtual void SetPropertys(User opUser, JObject jobject)
        {
            this.OnPropertyChanging(jobject);

            List<MetadataProperty> modifyPropertys = MetadataPropertyListHelper.MapPropertys(jobject, this.ColdewObject);

            foreach (MetadataProperty modifyproperty in modifyPropertys)
            {
                if (this._propertys.ContainsKey(modifyproperty.Field.Code))
                {
                    this._propertys[modifyproperty.Field.Code] = modifyproperty;
                }
                else
                {
                    this._propertys.Add(modifyproperty.Field.Code, modifyproperty);
                }
            }

            this.ColdewObject.DataService.Update(this.ID, this._propertys.Values.ToList());

            this.InitPropertys();
            this.BuildContent();

            this.OnPropertyChanged(jobject);
        }

        public virtual List<MetadataProperty> GetPropertys()
        {
            return this._propertys.Values.ToList();
        }

        public virtual List<MetadataProperty> GetPropertys(User user)
        {
            return this._propertys.Values.Where(x => x.Field.CanView(user)).ToList();
        }

        public MetadataProperty GetProperty(string propertyCode)
        {
            if (this._propertys.ContainsKey(propertyCode))
            {
                return this._propertys[propertyCode];
            }
            return null;
        }

        public MetadataProperty GetPropertyByObject(ColdewObject cObject)
        {
            return this._propertys.Values.Where(x => {
                if (x.Field is MetadataField)
                {
                    MetadataField field = x.Field as MetadataField;
                    return field.RelatedObject == cObject;
                }
                return false;
            }).FirstOrDefault();
        }

        public event TEventHandler<Metadata, User> Deleting;
        public event TEventHandler<Metadata, User> Deleted;

        public virtual void Delete(User opUser)
        {
            if (this.Deleting != null)
            {
                this.Deleting(this, opUser);
            }

            this.ColdewObject.DataService.Delete(this.ID);

            if (this.Deleted != null)
            {
                this.Deleted(this, opUser);
            }
        }

        public virtual bool CanPreview(User user)
        {
            if (this.ColdewObject.MetadataPermission.HasValue(user, MetadataPermissionValue.View, this))
            {
                return true;
            }
            return false;
        }

        public virtual MetadataInfo Map(User user)
        {
            return new MetadataInfo()
            {
                ID = this.ID,
                Name = this.Name,
                Summary = this.GetSummary(),
                PermissionValue = this.ColdewObject.MetadataPermission.GetValue(user, this),
                Favorited = this.ColdewObject.FavoriteManager.IsFavorite(user, this)
            };
        }

        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MetadataProperty property in this.GetPropertys())
            {
                if (property.Field.IsSummary && !string.IsNullOrEmpty(property.Value.ShowValue))
                {
                    sb.Append(property.Field.Name + "：" + property.Value.ShowValue + "；");
                }
            }
            return sb.ToString();
        }

        public JObject MapJObject(User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", this.ID);
            foreach (MetadataProperty property in this.GetPropertys(user))
            {
                jobject.Add(property.Field.Code, property.Value.JTokenValue);
            }
            return jobject;
        }
    }
}
