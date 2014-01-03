using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    [Serializable]
    public class MetadataInfo
    {
        public string ID { get; set; }

        public string Name { set; get; }

        public string Summary { set; get; }

        public bool Favorited { set; get; }

        public MetadataPermissionValue PermissionValue { set; get; }
    }
}
