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
        public ColdewDataManager(ColdewManager coldewManager)
        {
            this._coldewManager = coldewManager;
            this.ObjectDataProvider = new ObjectDataProvider(this._coldewManager.ObjectManager);
            List<ColdewObject> coldewObjects = this.ObjectDataProvider.Select();
            this._coldewManager.ObjectManager.AddObjects(coldewObjects);
            this._coldewManager.ObjectManager.Created += ObjectManager_Created;
        }

        void ObjectManager_Created(ColdewObjectManager sender, ColdewObject args)
        {
            this.ObjectDataProvider.Insert(args);
        }

        private void BindColdewObjectEvent(ColdewObject cobject)
        {

        }
    }
}
