using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coldew.Api.UI;
using Coldew.Core.DataProviders;
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
        GridColumnMapper _columnMapper;

        public FormManager(ColdewObject coldewObject)
        {
            this._objectManager = coldewObject.ObjectManager;
            this._coldewObject = coldewObject;
            this._forms = new List<Form>();
            this._lock = new ReaderWriterLock();
            this._columnMapper = new GridColumnMapper(coldewObject.ObjectManager);
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

        public event TEventHandler<FormManager, Form> Created;

        public Form Create(FormCreateInfo createInfo)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                Form form = new Form { ID = Guid.NewGuid().ToString(), Code = createInfo.Code, Title = createInfo.Title, Children = createInfo.Controls, ColdewObject = this._coldewObject };
                this._forms.Add(form);
                if (this.Created != null)
                {
                    this.Created(this, form);
                }
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

        public void AddForms(List<Form> forms)
        {
            this._forms.AddRange(forms);
        }

        public void AddForm(Form form)
        {
            this._forms.Add(form);
        }
    }
}
