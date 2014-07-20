using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Permission;

namespace Coldew.Core.DataManager
{
    public class MetadataStrategyPermissionDataManager
    {
        internal MetadataStrategyPermissionDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public MetadataStrategyPermissionDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new MetadataStrategyPermissionDataProvider(cobject);
            cobject.MetadataPermission.StrategyManager.Created += StrategyManager_Created;
            this.Load();
        }

        void StrategyManager_Created(MetadataPermissionStrategyManager sender, MetadataPermissionStrategy args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<MetadataPermissionStrategy> perms = this.DataProvider.Select();
            this._cobject.MetadataPermission.StrategyManager.AddPermission(perms);
        }
    }
}
