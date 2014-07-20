using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Permission;

namespace Coldew.Core.DataManager
{
    public class FieldPermissionDataManager
    {
        internal FieldPermissionDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public FieldPermissionDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new FieldPermissionDataProvider(cobject);
            cobject.FieldPermission.Created += FieldPermission_Created;
            this.Load();
        }

        void FieldPermission_Created(FieldPermissionManager sender, FieldPermission args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<FieldPermission> perms = this.DataProvider.Select();
            this._cobject.FieldPermission.AddPermission(perms);
        }
    }
}
