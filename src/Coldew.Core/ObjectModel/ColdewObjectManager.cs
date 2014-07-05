using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Coldew.Api;
using Coldew.Core.UI;
using Coldew.Core.DataProviders;

namespace Coldew.Core
{
    public class ColdewObjectManager
    {
        private List<ColdewObject> _objects;

        internal ColdewManager ColdewManager { private set; get; }

        internal ObjectDataProvider DataProvider { private set; get; }

        public ColdewObjectManager(ColdewManager coldewManager)
        {
            this.ColdewManager = coldewManager;
            this._objects = new List<ColdewObject>();
            this.DataProvider = new ObjectDataProvider();
        }

        private int MaxIndex()
        {
            if (this._objects.Count == 0)
            {
                return 1;
            }
            return this._objects.Max(x => x.Index) + 1;
        }

        public ColdewObject Create(ColdewObjectCreateInfo createInfo)
        {
            ColdewObject cobject = new ColdewObject(Guid.NewGuid().ToString(), createInfo.Code, createInfo.Name, 
                createInfo.IsSystem, this.MaxIndex(), "", this);
            this.DataProvider.Insert(cobject);
            
            this._objects.Add(cobject);

            return cobject;
        }

        public ColdewObject GetObjectById(string objectId)
        {
            return this._objects.Find(x => x.ID == objectId);
        }

        public ColdewObject GetObjectByCode(string code)
        {
            return this._objects.Find(x => x.Code == code);
        }

        public List<ColdewObject> GetObjects()
        {
            return this._objects.ToList();
        }

        public Field GetFieldById(string fieldId)
        {
            foreach (ColdewObject cobject in this._objects)
            {
                Field field = cobject.GetFieldById(fieldId);
                if (field != null)
                {
                    return field;
                }
            }
            return null;
        }

        public Form GetFormById(string formId)
        {
            foreach (ColdewObject cobject in this._objects)
            {
                Form form = cobject.FormManager.GetFormById(formId);
                if (form != null)
                {
                    return form;
                }
            }
            return null;
        }

        internal void Load()
        {
            IList<ColdewObjectModel> models = this.DataProvider.Select();
            foreach (ColdewObjectModel model in models)
            {
                ColdewObject cobject = new ColdewObject(model.ID, model.Code, model.Name, 
                   model.IsSystem, model.Index, model.NameFieldId, this);
                this._objects.Add(cobject);
            }
            this._objects = this._objects.OrderBy(x => x.Index).ToList();
            foreach (ColdewObject cobject in this._objects)
            {
                cobject.Load();
            }
        }
    }
}
