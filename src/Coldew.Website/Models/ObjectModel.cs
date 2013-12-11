using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api;

namespace Coldew.Website.Models
{
    public class ObjectModel
    {
        public ObjectModel(ColdewObjectInfo objectInfo)
        {

        }

        public string id;

        public string code { set; get; }

        public string Name { set; get; }

        public ColdewObjectType Type { set; get; }

        public List<FieldInfo> Fields { set; get; }
    }
}