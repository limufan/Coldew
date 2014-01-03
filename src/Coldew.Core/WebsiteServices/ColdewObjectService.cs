using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Core.WebsiteServices
{
    public class ColdewObjectService : IColdewObjectService
    {
        ColdewManager _coldewManager;

        public ColdewObjectService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public ColdewObjectWebModel GetObjectById(string userAccount, string objectId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            if (form != null)
            {
                return form.MapWebModel(user);
            }
            return null;
        }

        public ColdewObjectWebModel GetObjectByCode(string userAccount, string objectCode)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            if (form != null)
            {
                return form.MapWebModel(user);
            }
            return null;
        }
    }
}
