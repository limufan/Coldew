using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class ColdewObjectWebModel
    {
        public ColdewObjectWebModel(ColdewObject cobject, User user)
        {
            this.id = cobject.ID;
            this.name = cobject.Name;
            this.code = cobject.Code;
            this.permissionValue = cobject.GetPermission(user);
            this.fields = cobject.GetFields().Select(x =>{
                dynamic dynField = x;
                FieldWebModel model = FieldWebModel.Map(dynField, user);
                return model;
            }).ToList();
        }

        public string id;

        public string code;

        public string name;

        public List<FieldWebModel> fields;

        public ObjectPermissionValue permissionValue { set; get; }
    }
}
