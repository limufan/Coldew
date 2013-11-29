using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class MetadataRelatedPermissionModel
    {
        public virtual string ID { set; get; }

        public virtual string ObjectId { set; get; }

        public virtual string FieldCode { set; get; }
    }
}
