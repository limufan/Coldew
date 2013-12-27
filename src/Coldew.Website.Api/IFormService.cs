using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
{
    public interface IFormService
    {
        FormWebModel GetForm(string userAccount, string objectId, string code);

        FormWebModel GetFormByCode(string userAccount, string objectCode, string code);

        void Modify(FormModifyModel model);
    }
}
