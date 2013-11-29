﻿using System;
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
        UserManagement _userManager;
        public UserListField(FieldNewInfo info, bool defaultValueIsCurrent, UserManagement userManager)
            :base(info)
        {
            this.DefaultValueIsCurrent = defaultValueIsCurrent;
            this._userManager = userManager;
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
                    foreach (JToken account in value)
                    {
                        users.Add(this._userManager.GetUserByAccount(account.ToString()));
                    }
                }
                else
                {
                    users.Add(this._userManager.GetUserByAccount(value.ToString()));
                }
            }
            return new UserListMetadataValue(users, this);
        }

        public override FieldInfo Map(User user)
        {
            UserListFieldInfo info = new UserListFieldInfo();
            this.Fill(info, user);
            info.DefaultValueIsCurrent = this.DefaultValueIsCurrent;
            return info;
        }
    }
}
