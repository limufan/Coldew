using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Coldew.Data.UI;
using Newtonsoft.Json;

namespace Coldew.Core.UI
{
    public class FormDataService
    {
        protected ColdewObjectManager _objectManager;
        ColdewObject _coldewObject;
        JsonSerializerSettings _jsonSettings;
        public FormDataService(ColdewObjectManager objectManager, ColdewObject coldewObject)
        {
            this._objectManager = objectManager;
            this._coldewObject = coldewObject;
            this._jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple };
        }

        public void Insert(Form form)
        {
            FormModel model = new FormModel
            {
                ID = form.ID,
                Code = form.Code,
                Title = form.Title,
                RelatedsJson = JsonConvert.SerializeObject(form.Relateds.Select(x => x.MapModel()).ToList()),
                ControlsJson = JsonConvert.SerializeObject(this.Map(form.Controls), this._jsonSettings),
                ObjectId = this._coldewObject.ID
            };
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Form form)
        {
            FormModel model = NHibernateHelper.CurrentSession.Get<FormModel>(form.ID);
            model.ControlsJson = JsonConvert.SerializeObject(this.Map(form.Controls), this._jsonSettings);
            model.RelatedsJson = JsonConvert.SerializeObject(form.Relateds.Select(x => x.MapModel()));

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<Form> Load()
        {
            List<Form> forms = new List<Form>();
            IList<FormModel> models = NHibernateHelper.CurrentSession.QueryOver<FormModel>().Where(x => x.ObjectId == this._coldewObject.ID).List();
            foreach (FormModel model in models)
            {
                forms.Add(this.Create(model));
            }
            return forms;
        }

        private Form Create(FormModel model)
        {
            List<RelatedObjectModel> relatedModels = new List<RelatedObjectModel>();
            if (!string.IsNullOrEmpty(model.RelatedsJson))
            {
                relatedModels = JsonConvert.DeserializeObject<List<RelatedObjectModel>>(model.RelatedsJson);
            }
            List<ControlModel> controlModels = JsonConvert.DeserializeObject<List<ControlModel>>(model.ControlsJson, this._jsonSettings);
            List<RelatedObject> relateds = relatedModels.Select(x => new RelatedObject(x.ObjectCode, x.FieldCodes, this._objectManager)).ToList();

            Form form = new Form(model.ID, model.Code, model.Title, this.Map(controlModels), relateds, this);
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
            return new InputModel { fieldCode = input.Field.Code, isReadonly = input.IsReadonly, required = input.Required, width = input.Width };
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

        private ControlModel Map(Grid grid)
        {
            List<GridViewColumnModel> columns = grid.Columns.Select(x => new GridViewColumnModel{ FieldId = x.Field.ID}).ToList();
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
            Input input = new Input(this._coldewObject.GetFieldByCode(model.fieldCode));
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
            Form editForm = null; this._objectManager.GetFormById(model.editFormId);
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            List<GridViewColumn> columns = model.columns.Select(x => new GridViewColumn(this._objectManager.GetFieldById(x.FieldId))).ToList();
            Grid grid = new Grid(field, columns, editForm, addForm);
            grid.Width = model.width;
            grid.Required = model.required;
            grid.IsReadonly = model.isReadonly;
            grid.Editable = model.editable;
            if (model.footer != null)
            {
                grid.Footer = model.footer.Select(x => new GridViewFooter { FieldCode = x.fieldCode, Value = x.value, ValueType = (GridViewFooterValueType)x.valueType }).ToList();
            }
            return grid;
        }
    }
}
