using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class UserField : Field
    {
        public UserField(UserFieldNewInfo newInfo)
            : base(newInfo)
        {
        
        }

        public override string Type
        {
            get { return FieldType.User; }
        }

        public override string TypeName
        {
            get { return "用户"; }
        }

        public bool DefaultValueIsCurrent { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            User user = null;
            if (value != null)
            {
                string account = "";
                if (value.Type == JTokenType.Object)
                {
                    if (value["account"] == null)
                    {
                        throw new ArgumentException("value 不包含account属性");
                    }
                    account = value["account"].ToString();
                }
                else 
                {
                    account = value.ToString();
                }
                user = this.ColdewObject.ColdewManager.OrgManager.UserManager.GetUserByAccount(account);
            }
            return new UserMetadataValue(user, this);
        }

        public MetadataValue CreateMetadataValue(User value)
        {
            return new UserMetadataValue(value, this);
        }
    }
}
