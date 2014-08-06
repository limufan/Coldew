using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Api;
using System.Threading;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class MetadataFactory
    {
        public virtual Metadata Create(string id, MetadataValueDictionary values, MetadataManager metadataManager)
        {
            Metadata metadata = new Metadata(Guid.NewGuid().ToString(), values, metadataManager);
            return metadata;
        }
    }
}
