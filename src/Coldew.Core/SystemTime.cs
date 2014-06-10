using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core
{
    public static class SystemTime
    {
        private static DateTime? _now;
        public static DateTime Now
        {
            set
            {
                _now = value;
            }
            get
            {
                if (_now.HasValue)
                {
                    return _now.Value;
                }
                return DateTime.Now;
            }
        }

    }
}
