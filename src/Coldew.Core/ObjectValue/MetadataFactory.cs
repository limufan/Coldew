using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Api;
using System.Threading;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;
using Coldew.Core.DataProviders;

namespace Coldew.Core
{
    public class MetadataFactory
    {
        public MetadataFactory(MetadataManager metadataManager)
        {
            this.MetadataManager = metadataManager;
            this.ColdewObject = metadataManager.ColdewObject;
        }

        public ColdewObject ColdewObject { protected set; get; }

        public MetadataManager MetadataManager { protected set; get; }

        public virtual Metadata Create(MetadataModel model)
        {
            JObject jobject = JsonConvert.DeserializeObject<JObject>(model.PropertysJson);
            List<MetadataValue> valueList = new List<MetadataValue>();
            foreach (JProperty property in jobject.Properties())
            {
                Field field = this.ColdewObject.GetFieldById(property.Name);
                if (field != null && field.Type != FieldType.RelatedField)
                {
                    MetadataValue metadataValue = field.CreateMetadataValue(property.Value);
                    valueList.Add(metadataValue);
                }
            }
            MetadataValueDictionary valueDictionary = new MetadataValueDictionary(valueList);
            Metadata metadata = new Metadata(model.ID, valueDictionary, this.MetadataManager);
            return metadata;
        }

        public virtual Metadata Create(MetadataCreateInfo createInfo)
        {
            Metadata metadata = new Metadata(Guid.NewGuid().ToString(), createInfo.Value, this.MetadataManager);
            return metadata;
        }
    }
}
