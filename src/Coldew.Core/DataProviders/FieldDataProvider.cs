using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class FieldDataProvider
    {
        ColdewObject _coldewObject;
        public FieldDataProvider(ColdewObject coldewObject)
        {
            this._coldewObject = coldewObject;
        }

        public void Insert(Field field)
        {
            dynamic d_field = field;
            FieldModel model = this.MapFieldModel(d_field);
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        private CheckboxListFieldModel MapFieldModel(CheckboxListField field)
        {
            CheckboxListFieldModel model = new CheckboxListFieldModel { defaultValue = field.DefaultValue, selectList = field.SelectList };
            this.FillFieldModel(model, field);
            return model;
        }

        private void FillFieldModel(FieldModel model, Field field)
        {
            model.id = field.ID;
            model.code = field.Code;
            model.gridWidth = field.GridWidth;
            model.isSummary = field.IsSummary;
            model.isSystem = field.IsSystem;
            model.name = field.Name;
            model.required = field.Required;
            model.tip = field.Tip;
            model.type = field.Type;
            model.unique = field.Unique;
        }
    }
}
