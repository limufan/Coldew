using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class LiuchengMobanModel
    {
        public virtual string Id { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual string ColdewObjectCode { set; get; }

        public virtual string TransferUrl { set; get; }

        public virtual string Remark { set; get; }
    }
}
