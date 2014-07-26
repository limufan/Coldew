using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Coldew.Api;
using Coldew.Core.UI;
using Coldew.Core.DataProviders;
using Newtonsoft.Json;

namespace Coldew.Core
{
    public class ColdewObjectManager
    {
        private List<ColdewObject> _objects;

        internal ColdewManager ColdewManager { private set; get; }

        public ColdewObjectManager(ColdewManager coldewManager)
        {
            this.ColdewManager = coldewManager;
            this._objects = new List<ColdewObject>();
        }

        private int MaxIndex()
        {
            if (this._objects.Count == 0)
            {
                return 1;
            }
            return this._objects.Max(x => x.Index) + 1;
        }

        public event TEventHandler<ColdewObjectManager, ColdewObjectCreateInfo> Creating;

        public event TEventHandler<ColdewObjectManager, ColdewObject> Created;

        public ColdewObject Create(ColdewObjectCreateInfo createInfo)
        {
            if (this.Creating != null)
            {
                this.Creating(this, createInfo);
            }
            ColdewObject cobject = new ColdewObject(Guid.NewGuid().ToString(), createInfo.Code, createInfo.Name,
                    createInfo.IsSystem, this.MaxIndex(), this);
            this._objects.Add(cobject);

            if (this.Created != null)
            {
                this.Created(this, cobject);
            }

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

        public event TEventHandler<ColdewObjectManager, List<ColdewObject>> Added;

        public void AddObjects(List<ColdewObject> objects)
        {
            this._objects.AddRange(objects);
            this._objects = this._objects.OrderBy(o => o.Index).ToList();
            if (this.Added != null)
            {
                this.Added(this, objects);
            }
        }
    }
}
