using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class ColdewDataManager
    {
        public ObjectDataManager ObjectDataManager { set; get; }
        public ColdewDataManager(ColdewManager coldewManager)
        {
            this.ObjectDataManager = new ObjectDataManager(coldewManager);
        }
    }
}
