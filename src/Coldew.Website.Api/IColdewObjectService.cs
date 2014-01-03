using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
{
    public interface IColdewObjectService
    {
        ColdewObjectWebModel GetObjectById(string userAccount, string objectId);

        ColdewObjectWebModel GetObjectByCode(string userAccount, string objectCode);
    }
}
