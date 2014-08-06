using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Permission;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Data.DataProviders
{
    public class ObjectDataProvider
    {
        ColdewObjectManager _objectManager;

        public ObjectDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

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

        public void Load()
        {
            List<ColdewObject> objectList = new List<ColdewObject>();

            IList<ColdewObjectModel> models = NHibernateHelper.CurrentSession.QueryOver<ColdewObjectModel>().List();
            foreach (ColdewObjectModel model in models)
            {
                ColdewObjectNewInfo newInfo = new ColdewObjectNewInfo
                {
                    ID = model.ID,
                    Code = model.Code,
                    Name = model.Name,
                    IsSystem = model.IsSystem,
                    Index = model.Index,
                    ObjectManager = this._objectManager
                };
                ColdewObject cobject = new ColdewObject(newInfo);
                if(!string.IsNullOrEmpty(model.PermissionJson))
                {
                    List<ObjectPermissionModel> permissionModels = JsonConvert.DeserializeObject<List<ObjectPermissionModel>>(model.PermissionJson);
                    cobject.AddPermission(this.GetObjectPermissions(permissionModels, cobject));
                }
                objectList.Add(cobject);
            }
            this._objectManager.AddObjects(objectList);

            //load fields
            foreach (ColdewObjectModel model in models)
            {
                ColdewObject cobject = objectList.Find(x => x.ID == model.ID);
                List<FieldModel> fieldModels = JsonConvert.DeserializeObject<List<FieldModel>>(model.FieldsJson, TypificationJsonSettings.JsonSettings);
                List<Field> fields = fieldModels.Select(x => this.CreateField(x)).ToList();
                Field nameField = fields.Find(x => x.ID == model.NameFieldId);
                cobject.NameField = nameField;
                cobject.SetFields(fields);
            }
        }

        private List<ObjectPermission> GetObjectPermissions(List<ObjectPermissionModel> models, ColdewObject cobject)
        {
            List<ObjectPermission> permissions = new List<ObjectPermission>();
            foreach (ObjectPermissionModel model in models)
            {
                Member member = cobject.ColdewManager.OrgManager.GetMember(model.memberId);
                if (member != null)
                {
                    ObjectPermission permission = new ObjectPermission(member, (ObjectPermissionValue)model.value);
                    permissions.Add(permission);
                }
            }
            return permissions;
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
            List<ObjectPermissionModel> permissionModels = cobject.GetPermissions().Select(p => new ObjectPermissionModel{ memberId = p.Member.ID, value = (int)p.Value }).ToList();
            model.PermissionJson = JsonConvert.SerializeObject(permissionModels);
        }

        #region  Create Field Model

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
        #endregion

        #region Create Field
        public Field CreateField(FieldModel model)
        {
            dynamic d_model = model;
            Field field = this.CreateField(d_model);
            return field;
        }

        private Field CreateField(StringFieldModel model)
        {
            StringFieldNewInfo newInfo = new StringFieldNewInfo { Suggestions = model.suggestions, DefaultValue = model.defaultValue };
            this.FillFieldInfo(newInfo, model);
            Field field = new StringField(newInfo);
            return field;
        }

        private Field CreateField(TextFieldModel model)
        {
            TextFieldNewInfo newInfo = new TextFieldNewInfo { DefaultValue = model.defaultValue };
            this.FillFieldInfo(newInfo, model);
            Field field = new TextField(newInfo);
            return field;
        }

        private Field CreateField(UserFieldModel model)
        {
            UserFieldNewInfo newInfo = new UserFieldNewInfo { DefaultValueIsCurrent = model.defaultValueIsCurrent };
            this.FillFieldInfo(newInfo, model);
            Field field = new UserField(newInfo);
            return field;
        }

        private Field CreateField(UserListFieldModel model)
        {
            UserListFieldNewInfo newInfo = new UserListFieldNewInfo { DefaultValueIsCurrent = model.defaultValueIsCurrent };
            this.FillFieldInfo(newInfo, model);
            Field field = new UserListField(newInfo);
            return field;
        }

        private Field CreateField(NumberFieldModel model)
        {
            NumberFieldNewInfo newInfo = new NumberFieldNewInfo { DefaultValue = model.defaultValue, Max = model.max, Min = model.min, Precision = model.precision };
            this.FillFieldInfo(newInfo, model);
            Field field = new NumberField(newInfo);
            return field;
        }

        private Field CreateField(DropdownListFieldModel model)
        {
            DropdownFieldNewInfo newInfo = new DropdownFieldNewInfo { DefaultValue = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(newInfo, model);
            Field field = new DropdownListField(newInfo);
            return field;
        }

        private Field CreateField(RadioListFieldModel model)
        {
            RadioListFieldNewInfo newInfo = new RadioListFieldNewInfo { DefaultValue = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(newInfo, model);
            Field field = new RadioListField(newInfo);
            return field;
        }

        private Field CreateField(DateFieldModel model)
        {
            DateFieldNewInfo newInfo = new DateFieldNewInfo { DefaultValueIsToday = model.defaultValueIsToday };
            this.FillFieldInfo(newInfo, model);
            Field field = new DateField(newInfo);
            return field;
        }

        private Field CreateField(CheckboxListFieldModel model)
        {
            CheckboxListFieldNewInfo newInfo = new CheckboxListFieldNewInfo { DefaultValues = model.defaultValue, SelectList = model.selectList };
            this.FillFieldInfo(newInfo, model);
            Field field = new CheckboxListField(newInfo);
            return field;
        }

        private Field CreateField(MetadataFieldModel model)
        {
            MetadataFieldNewInfo newInfo = new MetadataFieldNewInfo { RelatedObject = this._objectManager.GetObjectById(model.relatedObjectId) };
            this.FillFieldInfo(newInfo, model);
            Field field = new MetadataField(newInfo);
            return field;
        }

        private Field CreateField(RelatedFieldModel model)
        {
            //Field field = new RelatedField { RelatedFieldCode = model.relatedFieldCode, PropertyCode = model.propertyCode };
            //this.FillFieldInfo(field, model);
            //return field;
            return null;
        }

        private Field CreateField(JsonFieldModel model)
        {
            JsonFieldNewInfo newInfo = new JsonFieldNewInfo { };
            this.FillFieldInfo(newInfo, model);
            Field field = new JsonField(newInfo);
            return field;
        }

        private Field CreateField(CodeFieldModel model)
        {
            CodeFieldNewInfo newInfo = new CodeFieldNewInfo { Format = model.format };
            this.FillFieldInfo(newInfo, model);
            Field field = new CodeField(newInfo);
            return field;
        }

        private void FillFieldInfo(FieldNewInfo newInfo, FieldModel model)
        {
            newInfo.ID = model.id;
            newInfo.Code = model.code;
            newInfo.GridWidth = model.gridWidth;
            newInfo.IsSummary = model.isSummary;
            newInfo.IsSystem = model.isSummary;
            newInfo.Name = model.name;
            newInfo.Required = model.required;
            newInfo.Tip = model.tip;
            newInfo.Unique = model.unique;
        }
        #endregion
    }
}
