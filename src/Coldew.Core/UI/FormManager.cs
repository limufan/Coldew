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
        internal FormDataProvider DataProvider { private set; get; }
        GridViewColumnMapper _columnMapper;

        public FormManager(ColdewObject coldewObject)
        {
            this._objectManager = coldewObject.ObjectManager;
            this._coldewObject = coldewObject;
            this._forms = new List<Form>();
            this._lock = new ReaderWriterLock();
            this._columnMapper = new GridViewColumnMapper(coldewObject.ObjectManager);
            this.DataProvider = new FormDataProvider(coldewObject);
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
                Form form = new Form(Guid.NewGuid().ToString(), code, title, controls, relateds, this);
                this.DataProvider.Insert(form);
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
            IList <FormModel> models = this.DataProvider.Select();
            foreach (FormModel model in models)
            {
                this._forms.Add(this.Create(model));
            }
        }

        private Form Create(FormModel model)
        {
            List<RelatedObjectModel> relatedModels = new List<RelatedObjectModel>();
            if (!string.IsNullOrEmpty(model.RelatedsJson))
            {
                relatedModels = JsonConvert.DeserializeObject<List<RelatedObjectModel>>(model.RelatedsJson);
            }
            List<ControlModel> controlModels = JsonConvert.DeserializeObject<List<ControlModel>>(model.ControlsJson, TypificationJsonSettings.JsonSettings);
            List<RelatedObject> relateds = relatedModels.Select(x => new RelatedObject(x.ObjectCode, x.FieldCodes, this._objectManager)).ToList();

            Form form = new Form(model.ID, model.Code, model.Title, this.Map(controlModels), relateds, this);
            return form;
        }

        public List<Control> Map(List<ControlModel> models)
        {
            List<Control> controls = new List<Control>();
            foreach (ControlModel model in models)
            {
                dynamic d = model;
                controls.Add(this.Map(d));
            }
            return controls;
        }

        private Control Map(InputModel model)
        {
            Input input = new Input(this._coldewObject.GetFieldById(model.fieldId));
            input.IsReadonly = model.isReadonly;
            input.Required = model.required;
            input.Width = model.width;
            return input;
        }

        private Control Map(RowModel model)
        {
            Row row = new Row();
            row.Children = this.Map(model.children);
            return row;
        }

        private Control Map(FieldsetModel model)
        {
            Fieldset fieldset = new Fieldset(model.title);
            return fieldset;
        }

        private Control Map(GridModel model)
        {
            Form addForm = this._objectManager.GetFormById(model.addFormId);
            Form editForm = this._objectManager.GetFormById(model.editFormId);
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            List<GridViewColumn> columns = model.columns.Select(x => this._columnMapper.MapColumn(x)).ToList();
            Grid grid = new Grid(field, columns, editForm, addForm);
            grid.Width = model.width;
            grid.Required = model.required;
            grid.IsReadonly = model.isReadonly;
            grid.Editable = model.editable;
            if (model.footer != null)
            {
                grid.Footer = model.footer.Select(x => new GridFooter { FieldCode = x.fieldCode, Value = x.value, ValueType = (GridViewFooterValueType)x.valueType }).ToList();
            }
            return grid;
        }
    }
}
