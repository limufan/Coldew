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
                createInfo.IsSystem, this.MaxIndex(), null, this);
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

        private Field CreateField(StringFieldModel model)
        {
            Field field = new StringField { Suggestions = model.suggestions, DefaultValue = model.defaultValue };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(TextFieldModel model)
        {
            Field field = new TextField { DefaultValue = model.defaultValue };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(UserFieldModel model)
        {
            Field field = new UserField { OrgManager = this.ColdewManager.OrgManager, DefaultValueIsCurrent = model.defaultValueIsCurrent };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(UserListFieldModel model)
        {
            Field field = new UserListField { OrgManager = this.ColdewManager.OrgManager, DefaultValueIsCurrent = model.defaultValueIsCurrent };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(NumberFieldModel model)
        {
            Field field = new NumberField { DefaultValue = model.defaultValue, Max = model.max, Min = model.min, Precision = model.precision };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(DropdownListFieldModel model)
        {
            Field field = new DropdownListField { DefaultValue = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(RadioListFieldModel model)
        {
            Field field = new RadioListField { DefaultValue = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(DateFieldModel model)
        {
            Field field = new DateField { DefaultValueIsToday = model.defaultValueIsToday };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(CheckboxListFieldModel model)
        {
            Field field = new CheckboxListField { DefaultValue = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(MetadataFieldModel model)
        {
            Field field = new MetadataField { RelatedObject = this.ColdewManager.ObjectManager.GetObjectById(model.relatedObjectId) };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(RelatedFieldModel model)
        {
            Field field = new RelatedField { RelatedFieldCode = model.relatedFieldCode, PropertyCode = model.propertyCode };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(JsonFieldModel model)
        {
            Field field = new JsonField();
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(CodeFieldModel model)
        {
            Field field = new CodeField { Format = model.format };
            this.FillFieldInfo(field, model);
            return field;
        }

        private void FillFieldInfo(Field field, FieldModel model)
        {
            field.ID = model.id;
            field.Code = model.code;
            field.GridWidth = model.gridWidth;
            field.IsSummary = model.isSummary;
            field.IsSystem = model.isSummary;
            field.Name = model.name;
            field.Required = model.required;
            field.Tip = model.tip;
            field.Unique = model.unique;
        }

        public virtual Field CreateField(FieldModel model)
        {
            dynamic d_model = model;
            Field field = this.CreateField(d_model);
            return field;
        }

        internal void Load()
        {
            IList<ColdewObjectModel> models = this.DataProvider.Select();
            foreach (ColdewObjectModel model in models)
            {
                ColdewObject cobject = new ColdewObject(model.ID, model.Code, model.Name,
                   model.IsSystem, model.Index, null, this);
                this._objects.Add(cobject);
            }
            this._objects = this._objects.OrderBy(x => x.Index).ToList();

            foreach (ColdewObjectModel model in models)
            {
                ColdewObject cobject = this.GetObjectById(model.ID);
                List<FieldModel> fieldModels = JsonConvert.DeserializeObject<List<FieldModel>>(model.FieldsJson, TypificationJsonSettings.JsonSettings);
                List<Field> fields = fieldModels.Select(x => this.CreateField(x)).ToList();
                Field nameField = fields.Find(x => x.ID == model.NameFieldId);
                cobject.NameField = nameField;
                cobject.SetFields(fields);
                cobject.Load();
            }
        }
    }
}
