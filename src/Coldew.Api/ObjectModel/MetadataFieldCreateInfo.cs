using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class MetadataFieldCreateInfo : FieldCreateInfo
    {
        public MetadataFieldCreateInfo(string code, string name, string objectId)
            :base(code, name)
        {
            this.ObjectId = objectId;
        }

        public string ObjectId { set; get; } 
    }
}
