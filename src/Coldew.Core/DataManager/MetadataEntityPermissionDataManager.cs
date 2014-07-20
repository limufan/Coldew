using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Permission;

namespace Coldew.Core.DataManager
{
    public class MetadataEntityPermissionDataManager
    {
        internal MetadataEntityPermissionDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public MetadataEntityPermissionDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new MetadataEntityPermissionDataProvider(cobject);
            cobject.MetadataPermission.EntityManager.Created += EntityManager_Created;
            this.Load();
        }

        void EntityManager_Created(Permission.MetadataEntityPermissionManager sender, Permission.MetadataEntityPermission args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<MetadataEntityPermission> perms = this.DataProvider.Select();
            this._cobject.MetadataPermission.EntityManager.AddPermission(perms);
        }
    }
}
