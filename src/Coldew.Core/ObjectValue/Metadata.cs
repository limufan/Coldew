using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Api;
using Coldew.Data;
using Coldew.Api.Exceptions;
using Coldew.Core.Permission;
using Coldew.Core.DataProviders;

namespace Coldew.Core
{
    public class Metadata
    {
        internal protected Metadata(string id, MetadataValueDictionary values, MetadataManager metadataManager)
        {
            this.ID = id;
            this.ColdewObject = metadataManager.ColdewObject;
            this._values = values;
        }

        public void RemoveValue(Field field)
        {
            this._values.Remove(field.Code);
            this.BuildContent();
        }

        public string ID { private set; get; }

        public string Name
        {
            get
            {
                if (this.ColdewObject.NameField != null)
                {
                    MetadataValue value = this.GetValue(this.ColdewObject.NameField.Code);
                    return value.ShowValue;
                }
                return "";
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
            foreach (MetadataValue value in this.GetValue())
            {
                if (!string.IsNullOrEmpty(value.ShowValue))
                {
                    sb.Append(value.ShowValue.ToLower());
                }
            }
            this.Content = sb.ToString();
        }

        protected MetadataValueDictionary _values;

        public event TEventHandler<Metadata, MetadataChangeInfo> Changing;
        public event TEventHandler<Metadata, MetadataChangeInfo> Changed;

        protected virtual void OnPropertyChanging(MetadataChangeInfo changeInfo)
        {
            if (this.Changing != null)
            {
                this.Changing(this, changeInfo);
            }
        }

        protected virtual void OnPropertyChanged(MetadataChangeInfo changeInfo)
        {
            if (this.Changed != null)
            {
                this.Changed(this, changeInfo);
            }
        }

        public virtual void SetValue(MetadataChangeInfo changeInfo)
        {
            this.OnPropertyChanging(changeInfo);

            this._values.SetValue(changeInfo.Value);

            this.BuildContent();

            this.OnPropertyChanged(changeInfo);
        }

        public virtual List<MetadataValue> GetValue()
        {
            return this._values.Values;
        }

        public virtual List<MetadataValue> GetValue(User user)
        {
            return this._values.Values.Where(x => x.Field.CanView(user)).ToList();
        }

        public MetadataValue GetValue(string fieldCode)
        {
            return this._values[fieldCode];
        }

        public MetadataValue GetRelatedValue(Field field, Field relatedField)
        {
            MetadataValue value = this.GetValue(field.Code);
            if (value != null && value is MetadataRelatedValue)
            {
                MetadataRelatedValue relatedValue = value as MetadataRelatedValue;
                MetadataValue relatedProperty = relatedValue.Metadata.GetValue(relatedField.Code);
                return relatedProperty;
            }
            return null;
        }

        public MetadataValue GetPropertyByObject(ColdewObject cObject)
        {
            return this._values.Values.Where(x => {
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

        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MetadataValue value in this.GetValue())
            {
                if (value.Field.IsSummary && !string.IsNullOrEmpty(value.ShowValue))
                {
                    sb.Append(value.Field.Name + "：" + value.ShowValue + "；");
                }
            }
            return sb.ToString();
        }

        public JObject GetJObject(User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", this.ID);
            foreach (MetadataValue value in this.GetValue(user))
            {
                jobject.Add(value.Field.Code, value.JTokenValue);
            }
            return jobject;
        }
    }
}
