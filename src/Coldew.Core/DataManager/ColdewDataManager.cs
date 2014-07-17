using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class ColdewDataManager
    {
        internal ObjectDataProvider ObjectDataProvider { private set; get; }
        ColdewManager _coldewManager;
        ObjectDataManager _objectDataManager;
        public ColdewDataManager(ColdewManager coldewManager)
        {
            _objectDataManager = new ObjectDataManager(coldewManager);
        }
    }
}
