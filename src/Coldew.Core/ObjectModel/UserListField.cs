using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class UserListField : Field
    {
        public UserListField(UserListFieldNewInfo newInfo)
            :base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.UserList; }
        }

        public override string TypeName
        {
            get { return "用户"; }
        }

        public bool DefaultValueIsCurrent { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            List<User> users = new List<User>();
            if (value != null)
            {
                if (value.Type == JTokenType.Array)
                {
                    foreach (JToken token in value)
                    {
                        users.Add(this.CreateUser(token));
                    }
                }
                else
                {
                    users.Add(this.CreateUser(value));
                }
            }
            return new UserListMetadataValue(users, this);
        }

        private User CreateUser(JToken value)
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
            return user;
        }
    }
}
