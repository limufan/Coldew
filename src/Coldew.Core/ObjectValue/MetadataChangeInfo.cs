using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class MetadataChangeInfo
    {
        public User Operator { set; get; }

        public MetadataValueDictionary Value { set; get; }
    }
}
