using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.UI;
using Coldew.Data;
using Coldew.Data.UI;
using Newtonsoft.Json;

namespace Coldew.Data.DataProviders
{
    public class FormDataProvider
    {
        ColdewObjectManager _objectManager;
        GridColumnMapper _columnMapper;
        public FormDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
            this._columnMapper = new GridColumnMapper(objectManager);
        }

        public void Insert(Form form)
        {
            FormModel model = new FormModel
            {
                ID = form.ID,
                Code = form.Code,
                Title = form.Title,
                ControlsJson = JsonConvert.SerializeObject(this.Map(form.Children), TypificationJsonSettings.JsonSettings),
                ObjectId = form.ColdewObject.ID
            };
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Form form)
        {
            FormModel model = NHibernateHelper.CurrentSession.Get<FormModel>(form.ID);
            model.ControlsJson = JsonConvert.SerializeObject(this.Map(form.Children), TypificationJsonSettings.JsonSettings);

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            List<Form> forms = new List<Form>();
            List<FormModel> models = NHibernateHelper.CurrentSession.QueryOver<FormModel>().List().ToList();
            foreach (FormModel model in models)
            {
                forms.Add(this.Create(model));
            }
            this.LoadControls(forms);
        }

        private Form Create(FormModel model)
        {
            Form form = new Form
            {
                ID = model.ID,
                Code = model.Code,
                Title = model.Title,
                ColdewObject = this._objectManager.GetObjectById(model.ObjectId)
            };
            form.ColdewObject.FormManager.AddForm(form);
            return form;
        }

        public List<ControlModel> Map(List<Control> controls)
        {
            List<ControlModel> models = new List<ControlModel>();
            foreach (Control control in controls)
            {
                dynamic d = control;
                models.Add(this.Map(d));
            }
            return models;
        }

        private ControlModel Map(Input input)
        {
            return new InputModel { fieldId = input.Field.ID, isReadonly = input.IsReadonly, required = input.Required, width = input.Width };
        }

        private ControlModel Map(Row row)
        {
            List<ControlModel> children = this.Map(row.Children);
            return new RowModel { children = children };
        }

        private ControlModel Map(Fieldset fieldset)
        {
            return new FieldsetModel { title = fieldset.Title };
        }

        private ControlModel Map(GridInput grid)
        {
            List<GridViewColumnModel> columns = grid.Columns.Select(x => this._columnMapper.MapColumnModel(x)).ToList();
            GridModel model = new GridModel { columns = columns, fieldId = grid.Field.ID, 
                                              isReadonly = grid.IsReadonly,
                                              required = grid.Required,
                                              width = grid.Width,
                                              editable = grid.Editable
            };
            if (grid.Footer != null)
            {
                model.footer = grid.Footer.Select(x => new GridViewFooterModel { fieldCode = x.FieldCode, value = x.Value, valueType = (int)(x.ValueType) }).ToList();
            }
            if (grid.AddForm != null)
            {
                model.addFormId = grid.AddForm.ID;
            }
            if (grid.EditForm != null)
            {
                model.editFormId = grid.EditForm.ID;
            }
            return model;
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
            Input input = new Input(this._objectManager.GetFieldById(model.fieldId));
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
            Field field = this._objectManager.GetFieldById(model.fieldId);
            List<GridColumn> columns = model.columns.Select(x => this._columnMapper.MapColumn(x)).ToList();
            GridInput grid = new GridInput(field, columns, editForm, addForm);
            grid.Width = model.width;
            grid.Required = model.required;
            grid.IsReadonly = model.isReadonly;
            grid.Editable = model.editable;
            if (model.footer != null)
            {
                grid.Footer = model.footer.Select(x => new GridFooter { FieldCode = x.fieldCode, Value = x.value, ValueType = (GridFooterValueType)x.valueType }).ToList();
            }
            return grid;
        }

        private void LoadControls(List<Form> forms)
        {
            List<FormModel> models = NHibernateHelper.CurrentSession.QueryOver<FormModel>().List().ToList();
            foreach (Form form in forms)
            {
                FormModel model = models.Find(m => m.ID == form.ID);
                List<ControlModel> controlModels = JsonConvert.DeserializeObject<List<ControlModel>>(model.ControlsJson, TypificationJsonSettings.JsonSettings);
                form.Children = this.Map(controlModels);
            }
        }
    }
}
