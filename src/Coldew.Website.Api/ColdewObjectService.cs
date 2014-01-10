using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
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
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            if (cobject != null)
            {
                return new ColdewObjectWebModel(cobject, user);
            }
            return null;
        }

        public ColdewObjectWebModel GetObjectByCode(string userAccount, string objectCode)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            if (cobject != null)
            {
                return new ColdewObjectWebModel(cobject, user);
            }
            return null;
        }

        public List<ColdewObjectWebModel> GetObjects(string userAccount)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            List<ColdewObject> objects = this._coldewManager.ObjectManager.GetObjects();
            return objects.Where(x =>
            {
                return x.ObjectPermission.HasValue(user, ObjectPermissionValue.View);
            }).Select(x => new ColdewObjectWebModel(x, user)).ToList();
        }
    }
}
