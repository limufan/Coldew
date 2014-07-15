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
    public class MetadataCreateInfo
    {
        public User Creator { set; get; }

        public JObject JObject { set; get; } 
    }

    public class MetadataFactory
    {
        public MetadataFactory(MetadataManager metadataManager)
        {
            this.MetadataManager = metadataManager;
        }

        public ColdewObject ColdewObject { protected set; get; }

        public MetadataManager MetadataManager { protected set; get; }

        public virtual Metadata Create(MetadataModel model)
        {
            JObject jobject = JsonConvert.DeserializeObject<JObject>(model.PropertysJson);
            Metadata metadata = new Metadata(model.ID, jobject, this.MetadataManager);
            return metadata;
        }

        public virtual Metadata Create(MetadataCreateInfo createInfo)
        {
            Metadata metadata = new Metadata(Guid.NewGuid().ToString(), createInfo.JObject, this.MetadataManager);
            return metadata;
        }
    }
}
