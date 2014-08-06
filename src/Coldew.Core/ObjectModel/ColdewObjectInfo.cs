using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json;
using log4net.Util;
using Coldew.Core.Organization;
using Coldew.Api.Exceptions;
using Coldew.Core.UI;
using Coldew.Core.Permission;

namespace Coldew.Core
{
    public class ColdewObjectBaseInfo
    {
        public string Code { set; get; }

        public string Name { set; get; }

        public bool IsSystem { set; get; }

        public int Index { set; get; }
    }

    public class ColdewObjectNewInfo : ColdewObjectBaseInfo
    {
        public string ID { set; get; }

        public ColdewObjectManager ObjectManager { set; get; }
    }

    public class ColdewObjectCreateInfo : ColdewObjectBaseInfo
    {
        
    } 
}
