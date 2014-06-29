using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class FieldModel
    {
        public virtual string ID { set; get; }

        public virtual string ObjectId { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual string Tip { set; get; }

        public virtual bool Required { set; get; }

        public virtual bool IsSystem { set; get; }

        public virtual bool IsSummary { set; get; }

        public virtual int GridWidth { set; get; }

        public virtual string Type { set; get; }

        public virtual bool Unique { set; get; }

        public virtual string Config { set; get; }
    }
}
