using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Permission;

namespace Coldew.Core.DataManager
{
    public class ObjectPermissionDataManager
    {
        internal ObjectPermissionDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public ObjectPermissionDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new ObjectPermissionDataProvider(cobject);
            cobject.ObjectPermission.Created += ObjectPermission_Created;
            this.Load();
        }

        void ObjectPermission_Created(ObjectPermissionManager sender, ObjectPermission args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<ObjectPermission> perms = this.DataProvider.Select();
            this._cobject.ObjectPermission.AddPermission(perms);
        }
    }
}
