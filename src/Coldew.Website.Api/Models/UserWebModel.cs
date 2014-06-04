using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;

namespace Coldew.Website.Api.Models
{

    [Serializable]
    public class UserWebModel
    {
        public UserWebModel(User user)
        {
            this.name = user.Name;
            this.account = user.Account;
        }

        public string name;

        public string account;
    }
}
