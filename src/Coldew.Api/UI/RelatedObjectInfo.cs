using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api.UI
{
    [Serializable]
    public class RelatedObjectInfo
    {
        public List<FieldInfo> ShowFields { set; get; }

        public string ObjectId { set; get; }

        public string ObjectName { set; get; }
    }
}
