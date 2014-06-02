using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core
{
    public static class SystemTime
    {
        static SystemTime()
        {
            Now = DateTime.Now;
        }

        public static DateTime Now { set; get; }

    }
}
