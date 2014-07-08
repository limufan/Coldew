using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Coldew.Api;
using Coldew.Core.UI;
using Coldew.Data;
using Coldew.Data.UI;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class FormDataProvider
    {
        ColdewObject _coldewObject;
        GridViewColumnMapper _columnMapper;
        public FormDataProvider(ColdewObject coldewObject)
        {
            this._coldewObject = coldewObject;
            this._columnMapper = new GridViewColumnMapper(this._coldewObject.ObjectManager);
        }

        public void Insert(Form form)
        {
            FormModel model = new FormModel
            {
                ID = form.ID,
                Code = form.Code,
                Title = form.Title,
                RelatedsJson = JsonConvert.SerializeObject(form.Relateds.Select(x => x.MapModel()).ToList()),
                ControlsJson = JsonConvert.SerializeObject(this.Map(form.Controls), TypificationJsonSettings.JsonSettings),
                ObjectId = this._coldewObject.ID
            };
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Form form)
        {
            FormModel model = NHibernateHelper.CurrentSession.Get<FormModel>(form.ID);
            model.ControlsJson = JsonConvert.SerializeObject(this.Map(form.Controls), TypificationJsonSettings.JsonSettings);
            model.RelatedsJson = JsonConvert.SerializeObject(form.Relateds.Select(x => x.MapModel()));

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public IList<FormModel> Select()
        {
            List<Form> forms = new List<Form>();
            IList<FormModel> models = NHibernateHelper.CurrentSession.QueryOver<FormModel>().Where(x => x.ObjectId == this._coldewObject.ID).List();
            return models;
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

        private ControlModel Map(Grid grid)
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
    }
}
