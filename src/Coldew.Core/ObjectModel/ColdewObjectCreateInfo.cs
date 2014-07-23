using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class ColdewObjectCreateInfo
    {
        public ColdewObjectCreateInfo()
        {

        }

        public ColdewObjectCreateInfo(string name, string code, bool isSystem)
        {
            this.Code = code;
            this.Name = name;
            this.IsSystem = isSystem;
        }

        public string Code { set; get; }

        public string Name { set; get; }

        public bool IsSystem { set; get; }
    }
}
