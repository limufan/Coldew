using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.Permission;

namespace Coldew.Core.DataManager
{
    public class MetadataRelatedPermissionDataManager
    {
        internal MetadataRelatedPermissionDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public MetadataRelatedPermissionDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new MetadataRelatedPermissionDataProvider(cobject);
            cobject.MetadataPermission.RelatedPermission.Created += RelatedPermission_Created;
            this.Load();
        }

        void RelatedPermission_Created(MetadataRelatedPermissionManager sender, MetadataRelatedPermission args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<MetadataRelatedPermission> perms = this.DataProvider.Select();
            this._cobject.MetadataPermission.RelatedPermission.AddPermission(perms);
        }
    }
}
