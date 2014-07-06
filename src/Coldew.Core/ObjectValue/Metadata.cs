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
        private MetadataDataProvider _dataProvider;

        internal Metadata(string id, MetadataManager metadataManager)
        {
            this.ID = id;
            this.ColdewObject = metadataManager.ColdewObject;
            this._propertys = new Dictionary<string, MetadataProperty>();
            this._dataProvider = metadataManager.DataProvider;
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
                this._propertys = propertys.ToDictionary(x => x.Field.Code);
                this._dataProvider.Update(this);
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
                if (this.ColdewObject.NameField != null)
                {
                    StringMetadataValue value = this.GetProperty(this.ColdewObject.NameField.Code).Value as StringMetadataValue;
                    return value.String;
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

        public event TEventHandler<MetadataChangingEventArgs> PropertyChanging;
        public event TEventHandler<MetadataChangingEventArgs> PropertyChanged;

        protected virtual void OnPropertyChanging(MetadataChangingEventArgs args)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(args);
            }
        }

        protected virtual void OnPropertyChanged(MetadataChangingEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(args);
            }
        }

        internal void SetPropertys(List<MetadataProperty> propertys)
        {
            this._propertys = propertys.ToDictionary(x => x.Field.Code);
        }

        internal void SetPropertys(JObject jobject)
        {
            List<MetadataProperty> propertys = new List<MetadataProperty>();
            foreach (JProperty property in jobject.Properties())
            {
                Field field = this.ColdewObject.GetFieldByCode(property.Name);
                if (field != null && field.Type != FieldType.RelatedField)
                {
                    MetadataValue metadataValue = field.CreateMetadataValue(property.Value);
                    propertys.Add(new MetadataProperty(metadataValue));
                }
            }

            foreach (MetadataProperty modifyproperty in propertys)
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
        }

        public virtual void SetPropertys(User opUser, JObject jobject)
        {
            MetadataChangingEventArgs args = new MetadataChangingEventArgs();
            args.ChangeInfo = jobject;
            args.Metadata = this;
            args.Operator = opUser;
            args.ChangingSnapshotInfo = this.MapJObject(opUser);
            this.OnPropertyChanging(args);

            this.SetPropertys(jobject);

            this._dataProvider.Update(this);
            this.InitPropertys();
            this.BuildContent();

            this.OnPropertyChanged(args);
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

            this._dataProvider.Delete(this.ID);

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
