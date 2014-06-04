﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class ColdewObjectCreateInfo
    {
        public ColdewObjectCreateInfo(string name, string code, ColdewObjectType type, bool isSystem)
        {
            this.Code = code;
            this.Name = name;
            this.Type = type;
            this.IsSystem = isSystem;
        }

        public string Code { set; get; }

        public string Name { set; get; }

        public ColdewObjectType Type { set; get; }

        public bool IsSystem { set; get; }
    }
}
