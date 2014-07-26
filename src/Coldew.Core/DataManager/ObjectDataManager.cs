using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class ObjectDataManager
    {
        List<MetadataDataManager> _metadataDataManagerList;
        List<MetadataFavoriteDataManager> _favoriteDataManagerList;
        List<FormDataManager> _formDataManagerList;
        List<GridViewDataManager> _gridViewDataManagerList;
        List<MetadataEntityPermissionDataManager> _metadataEntityPermissionDataManagerList;
        List<MetadataStrategyPermissionDataManager> _metadataStrategyPermissionDataManagerList;
        List<MetadataRelatedPermissionDataManager> _metadataRelatedPermissionDataManagerList;
        List<ObjectPermissionDataManager> _objectPermissionDataManagerList;
        List<FieldPermissionDataManager> _fieldPermissionDataManagerList;
        internal ObjectDataProvider DataProvider { private set; get; }
        ColdewManager _coldewManager;
        public ObjectDataManager(ColdewManager coldewManager)
        {
            this._coldewManager = coldewManager;
            this._metadataDataManagerList = new List<MetadataDataManager>();
            this._favoriteDataManagerList = new List<MetadataFavoriteDataManager>();
            this._formDataManagerList = new List<FormDataManager>();
            this._gridViewDataManagerList = new List<GridViewDataManager>();
            this._metadataEntityPermissionDataManagerList = new List<MetadataEntityPermissionDataManager>();
            this._metadataStrategyPermissionDataManagerList = new List<MetadataStrategyPermissionDataManager>();
            this._metadataRelatedPermissionDataManagerList = new List<MetadataRelatedPermissionDataManager>();
            this._objectPermissionDataManagerList = new List<ObjectPermissionDataManager>();
            this._fieldPermissionDataManagerList = new List<FieldPermissionDataManager>();
            this._coldewManager.ObjectManager.Created += ObjectManager_Created;
            this.Load();
        }

        void ObjectManager_Created(ColdewObjectManager sender, ColdewObject args)
        {
            this.DataProvider.Insert(args);
            this.BindEvent(args);
        }

        private void BindEvent(ColdewObject cobject)
        {
            cobject.Changed += Object_Changed;
            this._metadataDataManagerList.Add(new MetadataDataManager(cobject));
            this._favoriteDataManagerList.Add(new MetadataFavoriteDataManager(cobject));
            this._formDataManagerList.Add(new FormDataManager(cobject));
            this._gridViewDataManagerList.Add(new GridViewDataManager(cobject));
            this._metadataEntityPermissionDataManagerList.Add(new MetadataEntityPermissionDataManager(cobject));
            this._metadataStrategyPermissionDataManagerList.Add(new MetadataStrategyPermissionDataManager(cobject));
            this._metadataRelatedPermissionDataManagerList.Add(new MetadataRelatedPermissionDataManager(cobject));
            this._objectPermissionDataManagerList.Add(new ObjectPermissionDataManager(cobject));
            this._fieldPermissionDataManagerList.Add(new FieldPermissionDataManager(cobject));
        }

        void Object_Changed(ColdewObject args)
        {
            this.DataProvider.Update(args);
        }

        void Load()
        {
            this.DataProvider = new ObjectDataProvider(this._coldewManager.ObjectManager);
            List<ColdewObject> coldewObjects = this.DataProvider.Select();
            this._coldewManager.ObjectManager.AddObjects(coldewObjects);
            coldewObjects.ForEach(x => this.BindEvent(x));
            this._formDataManagerList.ForEach(manager => manager.LoadFormControls());
        }
    }
}
