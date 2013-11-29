using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.Organization;

namespace Coldew.Core.UI
{
    public class FormService : IFormService
    {
        ColdewManager _coldewManager;

        public FormService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public FormInfo GetForm(string userAccount, string objectId, string code)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cObject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Form form = cObject.FormManager.GetFormByCode(code);
            if (form != null)
            {
                return form.Map(user);
            }
            return null;
        }

        public FormInfo GetFormByCode(string userAccount, string objectCode, string code)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cObject = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            Form form = cObject.FormManager.GetFormByCode(code);
            if (form != null)
            {
                return form.Map(user);
            }
            return null;
        }
    }
}
