using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coldew.Api.UI;
using Coldew.Data;
using Coldew.Data.UI;
using Newtonsoft.Json;

namespace Coldew.Core.UI
{
    public class FormManager
    {
        protected ColdewObjectManager _objectManager;
        protected ColdewObject _coldewObject;
        List<Form> _forms;
        protected ReaderWriterLock _lock;
        FormDataService _dataService;

        public FormManager(ColdewObjectManager objectManager, ColdewObject coldewObject)
        {
            this._objectManager = objectManager;
            this._coldewObject = coldewObject;
            this._forms = new List<Form>();
            this._lock = new ReaderWriterLock();
            this._dataService = new FormDataService(objectManager, coldewObject);
            this._coldewObject.FieldDeleted += new TEventHandler<ColdewObject, Field>(ColdewObject_FieldDeleted);
        }

        void ColdewObject_FieldDeleted(ColdewObject sender, Field field)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                foreach (Form form in this._forms)
                {
                    form.ClearFieldData(field);
                }
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Form Create(string code, string title, List<Control> controls, List<RelatedObject> relateds)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                Form form = new Form(Guid.NewGuid().ToString(), code, title, controls, relateds, this._dataService);
                this._dataService.Insert(form);
                this._forms.Add(form);
                return form;
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public Form GetFormById(string formId)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                return this._forms.Find(x => x.ID == formId);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Form GetFormByCode(string code)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                return this._forms.Find(x => x.Code == code);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Form> GetForms()
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                return this._forms.ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        internal void Load()
        {
            this._forms = this._dataService.Load();
        }
    }
}
