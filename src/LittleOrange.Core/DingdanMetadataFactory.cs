using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;

namespace LittleOrange.Core
{
    public class DingdanMetadataFactory : MetadataFactory
    {
        public override Metadata Create(string id, MetadataValueDictionary values, MetadataManager metadataManager)
        {
            return base.Create(id, values, metadataManager);
        }
    }
}
