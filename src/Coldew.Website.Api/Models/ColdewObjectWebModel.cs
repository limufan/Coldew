using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class ColdewObjectWebModel
    {
        public string id;

        public string code;

        public string name;

        public ColdewObjectType type;

        public List<FieldWebModel> fields;

        public ObjectPermissionValue permissionValue { set; get; }
    }
}
