using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api.UI
{
    public interface IFormService
    {
        FormInfo GetForm(string userAccount, string objectId, string code);

        FormInfo GetFormByCode(string userAccount, string objectCode, string code);
    }
}
