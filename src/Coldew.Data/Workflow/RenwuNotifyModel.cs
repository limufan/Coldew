using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class RenwuNotifyModel
    {
        public virtual string Id { set; get; }

        public virtual string RenwuId { get; set; }

        public virtual string UserAccount { get; set; }

        public virtual DateTime NotifyTime { get; set; }

        public virtual string Subject { get; set; }

        public virtual string Body { get; set; }
    }
}
