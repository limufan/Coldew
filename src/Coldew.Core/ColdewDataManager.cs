using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core
{
    public class ColdewDataManager
    {
        public ColdewDataManager(ColdewManager coldewManager)
        {
            this.PositionDataProvider = new PositionDataProvider(coldewManager.OrgManager);
            this.UserDataProvider = new UserDataProvider(coldewManager.OrgManager);
            this.DepartmentDataProvider = new DepartmentDataProvider(coldewManager.OrgManager);
            this.FunctionDataProvider = new FunctionDataProvider(coldewManager.OrgManager);
            this.GroupDataProvider = new GroupDataProvider(coldewManager.OrgManager);
            this.ObjectDataProvider = new ObjectDataProvider(coldewManager.ObjectManager);
            this.FormDataProvider = new FormDataProvider(coldewManager.ObjectManager);
            this.GridViewDataProvider = new GridViewDataProvider(coldewManager.ObjectManager);
            this.MetadataDataProvider = new MetadataDataProvider(coldewManager.ObjectManager);
            this.MetadataEntityPermissionDataProvider = new MetadataEntityPermissionDataProvider(coldewManager.ObjectManager);
            this.MetadataRelatedPermissionDataProvider = new MetadataRelatedPermissionDataProvider(coldewManager.ObjectManager);
            this.MetadataStrategyPermissionDataProvider = new MetadataStrategyPermissionDataProvider(coldewManager.ObjectManager);
            this.MetadataFavoriteDataProvider = new MetadataFavoriteDataProvider(coldewManager.ObjectManager);
            this.FieldPermissionDataProvider = new FieldPermissionDataProvider(coldewManager.ObjectManager);
            this.Load();
        }
        public UserDataProvider UserDataProvider { set; get; }

        public DepartmentDataProvider DepartmentDataProvider { set; get; }

        public FunctionDataProvider FunctionDataProvider { set; get; }

        public PositionDataProvider PositionDataProvider { set; get; }

        public GroupDataProvider GroupDataProvider { set; get; }

        public ObjectDataProvider ObjectDataProvider { private set; get; }

        public FormDataProvider FormDataProvider { private set; get; }

        public GridViewDataProvider GridViewDataProvider { private set; get; }

        public MetadataDataProvider MetadataDataProvider { private set; get; }

        public MetadataEntityPermissionDataProvider MetadataEntityPermissionDataProvider { private set; get; }

        public MetadataRelatedPermissionDataProvider MetadataRelatedPermissionDataProvider { private set; get; }

        public MetadataStrategyPermissionDataProvider MetadataStrategyPermissionDataProvider { private set; get; }

        public FieldPermissionDataProvider FieldPermissionDataProvider { private set; get; }

        public MetadataFavoriteDataProvider MetadataFavoriteDataProvider { private set; get; }

        public void Load()
        {
            this.PositionDataProvider.Load();
            this.UserDataProvider.Load();
            this.PositionDataProvider.LoadUsers();
            this.DepartmentDataProvider.Load();
            this.FunctionDataProvider.Load();
            this.GroupDataProvider.Load();
            this.ObjectDataProvider.Load();
            this.FormDataProvider.Load();
            this.GridViewDataProvider.Load();
            this.MetadataDataProvider.Load();
            this.MetadataEntityPermissionDataProvider.Load();
            this.MetadataRelatedPermissionDataProvider.Load();
            this.MetadataStrategyPermissionDataProvider.Load();
            this.FieldPermissionDataProvider.Load();
            this.MetadataFavoriteDataProvider.Load();
        }
    }
}
