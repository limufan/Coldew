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
            this._values = new Dictionary<string, MetadataValue>();
            this._dataProvider = metadataManager.DataProvider;
            this.InitPropertys();
        }

        private void InitPropertys()
        {
            List<MetadataValue> virtualPropertys = this.GetVirtualPropertys();
            if (virtualPropertys != null)
            {
                foreach (MetadataValue property in virtualPropertys)
                {
                    if (this._values.ContainsKey(property.Field.Code))
                    {
                        this._values.Remove(property.Field.Code);
                    }
                    this._values.Add(property.Field.Code, property);
                }
            }
        }

        public void RemoveValue(Field field)
        {
            List<MetadataValue> values = this._values.Values.ToList();
            MetadataValue value = values.Find(x => x.Field == field);
            if (value != null)
            {
                values.Remove(value);
                this._values = values.ToDictionary(x => x.Field.Code);
                this._dataProvider.Update(this);
                this.BuildContent();
            }
        }

        protected virtual List<MetadataValue> GetVirtualPropertys()
        {
            List<MetadataValue> propertys = new List<MetadataValue>();
            List<RelatedField> relatedFields = this.ColdewObject.GetRelatedFields();
            foreach(RelatedField relatedField in relatedFields)
            {
                if (this._values.ContainsKey(relatedField.RelatedFieldCode))
                {
                    MetadataRelatedValue realtedValue = this._values[relatedField.RelatedFieldCode] as MetadataRelatedValue;
                    if (realtedValue != null)
                    {
                        MetadataValue property = realtedValue.Metadata.GetValue(relatedField.PropertyCode);
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

        protected Dictionary<string, MetadataValue> _values;

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

        internal void SetValue(List<MetadataValue> values)
        {
            this._values = values.ToDictionary(x => x.Field.Code);
        }

        internal void SetValue(JObject jobject)
        {
            List<MetadataValue> values = new List<MetadataValue>();
            foreach (JProperty property in jobject.Properties())
            {
                Field field = this.ColdewObject.GetFieldByCode(property.Name);
                if (field != null && field.Type != FieldType.RelatedField)
                {
                    MetadataValue metadataValue = field.CreateMetadataValue(property.Value);
                    values.Add(metadataValue);
                }
            }

            foreach (MetadataValue modifyValue in values)
            {
                if (this._values.ContainsKey(modifyValue.Field.Code))
                {
                    this._values[modifyValue.Field.Code] = modifyValue;
                }
                else
                {
                    this._values.Add(modifyValue.Field.Code, modifyValue);
                }
            }
        }

        public virtual void SetValue(User opUser, JObject jobject)
        {
            MetadataChangingEventArgs args = new MetadataChangingEventArgs();
            args.ChangeInfo = jobject;
            args.Metadata = this;
            args.Operator = opUser;
            args.ChangingSnapshotInfo = this.GetJObject(opUser);
            this.OnPropertyChanging(args);

            this.SetValue(jobject);

            this._dataProvider.Update(this);
            this.InitPropertys();
            this.BuildContent();

            this.OnPropertyChanged(args);
        }

        public virtual List<MetadataValue> GetValue()
        {
            return this._values.Values.ToList();
        }

        public virtual List<MetadataValue> GetValue(User user)
        {
            return this._values.Values.Where(x => x.Field.CanView(user)).ToList();
        }

        public MetadataValue GetValue(string fieldCode)
        {
            if (this._values.ContainsKey(fieldCode))
            {
                return this._values[fieldCode];
            }
            return null;
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

        public JObject GetJObject(GridView gridView, User opUser)
        {
            JObject jobject = new JObject();
            jobject.Add("id", this.ID);
            foreach (GridViewColumn column in gridView.Columns)
            {
                MetadataValue value = column.GetValue(this);
                if (value != null)
                {
                    jobject.Add(value.Field.Code, value.ShowValue);
                }
            }
            return jobject;
        }
    }
}
