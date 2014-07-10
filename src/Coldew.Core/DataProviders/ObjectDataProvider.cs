using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class ObjectDataProvider
    {
        public void Insert(ColdewObject cobject)
        {
            ColdewObjectModel model = new ColdewObjectModel();
            this.FillInfo(model, cobject);
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(ColdewObject cobject)
        {
            ColdewObjectModel model = NHibernateHelper.CurrentSession.Get<ColdewObjectModel>(cobject.ID);
            this.FillInfo(model, cobject);
            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public IList<ColdewObjectModel> Select()
        {
            List<ColdewObject> objectList = new List<ColdewObject>();
            IList<ColdewObjectModel> models = NHibernateHelper.CurrentSession.QueryOver<ColdewObjectModel>().List();
            return models;
        }

        private void FillInfo(ColdewObjectModel model, ColdewObject cobject)
        {
            List<FieldModel> fieldModels = this.CreateFieldModels(cobject.GetFields());
            model.ID = cobject.ID;
            model.Code = cobject.Code;
            model.Name = cobject.Name;
            model.IsSystem = cobject.IsSystem;
            model.Index = cobject.Index;
            if (cobject.NameField != null)
            {
                model.NameFieldId = cobject.NameField.ID;
            }
            else
            {
                model.NameFieldId = "";
            }
            model.FieldsJson = JsonConvert.SerializeObject(fieldModels, TypificationJsonSettings.JsonSettings);
        }

        private List<FieldModel> CreateFieldModels(List<Field> fields)
        {
            List<FieldModel> models = new List<FieldModel>();
            foreach (dynamic field in fields)
            {
                models.Add(this.CreateFieldModel(field));
            }
            return models;
        }

        private FieldModel CreateFieldModel(CheckboxListField field)
        {
            CheckboxListFieldModel model = new CheckboxListFieldModel { defaultValue = field.DefaultValue, selectList = field.SelectList };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(StringField field)
        {
            FieldModel model = new StringFieldModel { suggestions = field.Suggestions, defaultValue = field.DefaultValue };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(TextField field)
        {
            FieldModel model = new TextFieldModel { defaultValue = field.DefaultValue };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(UserField field)
        {
            FieldModel model = new UserFieldModel { defaultValueIsCurrent = field.DefaultValueIsCurrent };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(UserListField field)
        {
            FieldModel model = new UserListFieldModel { defaultValueIsCurrent = field.DefaultValueIsCurrent };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(NumberField field)
        {
            FieldModel model = new NumberFieldModel { defaultValue = field.DefaultValue, max = field.Max, min = field.Min, precision = field.Precision };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(DropdownListField field)
        {
            FieldModel model = new DropdownListFieldModel { defaultValue = field.DefaultValue, selectList = field.SelectList };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(RadioListField field)
        {
            FieldModel model = new RadioListFieldModel { defaultValue = field.DefaultValue, selectList = field.SelectList };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(DateField field)
        {
            FieldModel model = new DateFieldModel { defaultValueIsToday = field.DefaultValueIsToday };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(MetadataField field)
        {
            FieldModel model = new MetadataFieldModel { relatedObjectId = field.RelatedObject.ID };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(RelatedField field)
        {
            FieldModel model = new RelatedFieldModel { relatedFieldCode = field.RelatedField1.Code, propertyCode = field.ValueField.Code };
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(JsonField field)
        {
            FieldModel model = new JsonFieldModel();
            this.FillInfo(model, field);
            return model;
        }

        private FieldModel CreateFieldModel(CodeField field)
        {
            FieldModel model = new CodeFieldModel { format = field.Format };
            this.FillInfo(model, field);
            return model;
        }

        private void FillInfo(FieldModel model, Field field)
        {
            model.id = field.ID;
            model.code = field.Code;
            model.gridWidth = field.GridWidth;
            model.isSummary = field.IsSummary;
            model.isSystem = field.IsSystem;
            model.name = field.Name;
            model.required = field.Required;
            model.tip = field.Tip;
            model.unique = field.Unique;
        }
    }
}
