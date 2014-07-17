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
        internal ObjectDataProvider DataProvider { private set; get; }
        ColdewManager _coldewManager;
        public ObjectDataManager(ColdewManager coldewManager)
        {
            this._metadataDataManagerList = new List<MetadataDataManager>();
            this._coldewManager = coldewManager;
            this.DataProvider = new ObjectDataProvider(this._coldewManager.ObjectManager);
            List<ColdewObject> coldewObjects = this.DataProvider.Select();
            coldewObjects.ForEach(x => this.BindEvent(x));
            this._coldewManager.ObjectManager.AddObjects(coldewObjects);
            this._coldewManager.ObjectManager.Created += ObjectManager_Created;
        }

        void ObjectManager_Created(ColdewObjectManager sender, ColdewObject args)
        {
            this.DataProvider.Insert(args);
            this.BindEvent(args);
        }

        private void BindEvent(ColdewObject cobject)
        {
            cobject.Modfied += Object_Modfied;
            this._metadataDataManagerList.Add( new MetadataDataManager(cobject));
        }

        void Object_Modfied(ColdewObject args)
        {
            this.DataProvider.Update(args);
        }
    }
}
