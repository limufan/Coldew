using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.UI;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
{
    public class FormService : IFormService
    {
        ColdewManager _coldewManager;

        public FormService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public FormWebModel GetForm(string userAccount, string objectId, string code)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cObject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Form form = cObject.FormManager.GetFormByCode(code);
            if (form != null)
            {
                return new FormWebModel(form, user);
            }
            return null;
        }

        public FormWebModel GetFormByCode(string userAccount, string objectCode, string code)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cObject = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            Form form = cObject.FormManager.GetFormByCode(code);
            if (form != null)
            {
                return new FormWebModel(form, user);
            }
            return null;
        }

        public void Modify(FormModifyModel model)
        {
            
        }
    }
}
