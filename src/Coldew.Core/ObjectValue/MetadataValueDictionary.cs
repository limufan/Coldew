using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class MetadataValueDictionary
    {
        public MetadataValueDictionary(List<MetadataValue> values)
        {
            this._values = values.ToDictionary(x => x.Field.Code);
        }

        Dictionary<string, MetadataValue> _values;
        ColdewObject _coldewObject;
        public MetadataValueDictionary(ColdewObject coldewObject, JObject jobject)
        {
            this._coldewObject = coldewObject;
            List<MetadataValue> valueList = this.GetValueList(jobject);
            this._values = valueList.ToDictionary(x => x.Field.Code);
        }

        private List<MetadataValue> GetValueList(JObject jobject)
        {
            List<MetadataValue> valueList = new List<MetadataValue>();
            foreach (JProperty property in jobject.Properties())
            {
                Field field = this._coldewObject.GetFieldByCode(property.Name);
                if (field != null)
                {
                    MetadataValue metadataValue = field.CreateMetadataValue(property.Value);
                    valueList.Add(metadataValue);
                }
            }
            return valueList;
        }

        public MetadataValue this[string fieldCode]
        {
            get
            {
                if (this._values.ContainsKey(fieldCode))
                {
                    return this._values[fieldCode];
                }
                return null;
            }
            set
            {
                if (this._values.ContainsKey(fieldCode))
                {
                    this._values[fieldCode] = value;
                }
                else
                {
                    this._values.Add(fieldCode, value);
                }
            }
        }

        public List<MetadataValue> Values
        {
            get
            {
                return this._values.Values.ToList();
            }
        }

        public void Remove(string fieldCode)
        {
            this._values.Remove(fieldCode);
        }

        public void SetValue(MetadataValueDictionary value)
        {
            this.SetValue(value.Values);
        }

        public void SetValue(JObject value)
        {
            List<MetadataValue> valueList = this.GetValueList(value);
            this.SetValue(valueList);
        }

        public void SetValue(List<MetadataValue> values)
        {
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

        public JObject ToJObject()
        {
            JObject jobject = new JObject();
            foreach (MetadataValue value in this.Values)
            {
                jobject.Add(value.Field.Code, value.JTokenValue);
            }
            return jobject;
        }
    }
}
