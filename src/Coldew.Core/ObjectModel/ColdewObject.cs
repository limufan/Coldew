using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Coldew.Api;
using Newtonsoft.Json;
using log4net.Util;
using Coldew.Core.Organization;
using Coldew.Api.Exceptions;
using Coldew.Core.UI;
using Coldew.Core.Permission;
using Coldew.Core.DataProviders;


namespace Coldew.Core
{
    public class ColdewObject
    {
        ReaderWriterLock _lock;
        private List<Field> _fields;
        ObjectDataProvider _dataProvider;
        FieldDataProvider _fieldDataProvider;
        public ColdewObjectManager ObjectManager { set; get; }

        public ColdewObject(string id, string code, string name, bool isSystem, int index, string nameFieldId, ColdewObjectManager objectManager)
        {
            this.ID = id;
            this.Name = name;
            this.Code = code;
            this.IsSystem = isSystem;
            this.Index = index;
            this._nameFieldId = nameFieldId;
            this._fields = new List<Field>();
            this._lock = new ReaderWriterLock();
            this.ObjectManager = objectManager;
            this._dataProvider = objectManager.DataProvider;
            this.ColdewManager = objectManager.ColdewManager;
            this.MetadataManager = this.CreateMetadataManager(this.ColdewManager);
            this.FavoriteManager = new MetadataFavoriteManager(this);
            this.GridViewManager = this.CreateGridViewManager(this.ColdewManager);
            this.FormManager = this.CreateFormManager(this.ColdewManager);
            this.ObjectPermission = new ObjectPermissionManager(this);
            this.MetadataPermission = new MetadataPermissionManager(this);
            this.FieldPermission = new FieldPermissionManager(this);
            this._fieldDataProvider = new FieldDataProvider(this);
        }

        public ColdewManager ColdewManager { private set; get; }

        protected virtual MetadataManager CreateMetadataManager(ColdewManager coldewManager)
        {
            return new MetadataManager(this, coldewManager.OrgManager);
        }

        protected virtual FormManager CreateFormManager(ColdewManager coldewManager)
        {
            return new FormManager(this);
        }

        protected virtual GridViewManager CreateGridViewManager(ColdewManager coldewManager)
        {
            return new GridViewManager(this);
        }

        public virtual ObjectPermissionManager ObjectPermission { private set; get; }

        public virtual MetadataPermissionManager MetadataPermission { private set; get; }

        public virtual FieldPermissionManager FieldPermission { private set; get; }

        public string ID { set; get; }

        public string Code { set; get; }

        public string Name { set; get; }

        public bool IsSystem { set; get; }

        public int Index { set; get; }

        public void SetNameField(Field field)
        {
            ColdewObject cobject = this.MemberwiseClone() as ColdewObject;
            cobject._nameFieldId = field.ID;
            this._dataProvider.Update(cobject);
            this._nameFieldId = field.ID;
        }

        public MetadataManager MetadataManager { private set; get; }

        public MetadataFavoriteManager FavoriteManager { private set; get; }

        public GridViewManager GridViewManager { private set; get; }

        public FormManager FormManager { private set; get; }

        private string _nameFieldId;

        public Field NameField
        {
            get
            {
                return this.GetFieldById(this._nameFieldId);
            }
        }

        public Field CreateStringField(StringFieldCreateInfo createInfo)
        {
            StringField field = new StringField{ DefaultValue = createInfo.DefaultValue, Suggestions = createInfo.Suggestions };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        private void FillFieldInfo(Field field, FieldCreateInfo createInfo)
        {
            field.Code = createInfo.Code;
            field.GridWidth = createInfo.GridWidth;
            field.IsSummary = createInfo.IsSummary;
            field.IsSystem = createInfo.IsSystem;
            field.Name = createInfo.Name;
            field.Required = createInfo.Required;
            field.Tip = createInfo.Tip;
            field.Unique = createInfo.Unique;
        }

        public Field CreateJsonField(JsonFieldCreateInfo createInfo)
        {
            JsonField field = new JsonField();
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateTextField(TextFieldCreateInfo createInfo)
        {
            TextField field = new TextField{ DefaultValue = createInfo.DefaultValue };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateDateField(DateFieldCreateInfo createInfo)
        {
            DateField field = new DateField { DefaultValueIsToday = createInfo.DefaultValueIsToday };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateNumberField(NumberFieldCreateInfo createInfo)
        {
            NumberField field = new NumberField { DefaultValue = createInfo.DefaultValue, Max = createInfo.Max, Min = createInfo.Min, Precision = createInfo.Precision };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateCheckboxListField(CheckboxListFieldCreateInfo createInfo)
        {
            CheckboxListField field = new CheckboxListField { DefaultValue = createInfo.DefaultValues, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateRadioListField(RadioListFieldCreateInfo createInfo)
        {
            RadioListField field = new RadioListField { DefaultValue = createInfo.DefaultValue, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateDropdownField(DropdownFieldCreateInfo createInfo)
        {
            DropdownField field = new DropdownField { DefaultValue = createInfo.DefaultValue, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateUserField(UserFieldCreateInfo createInfo)
        {
            UserField field = new UserField { DefaultValueIsCurrent = createInfo.DefaultValueIsCurrent };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateUserListField(UserListFieldCreateInfo createInfo)
        {
            UserListField field = new UserListField { DefaultValueIsCurrent = createInfo.DefaultValueIsCurrent };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateMetadataField(MetadataFieldCreateInfo createInfo)
        {
            MetadataField field = new MetadataField { ColdewObject = createInfo.ColdewObject };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        private Field CreateRelatedField(RelatedFieldCreateInfo createInfo)
        {
            RelatedField field = new RelatedField { PropertyCode = createInfo.PropertyCode, RelatedFieldCode = createInfo.RelatedFieldCode };
            this.FillFieldInfo(field, createInfo);
            this._fieldDataProvider.Insert(field);
            return field;
        }

        public Field CreateField(CodeFieldCreateInfo createInfo)
        {
            return this.CreateField(createInfo, FieldType.Code, createInfo.Format);
        }

        public event TEventHandler<ColdewObject, Field> FieldCreated;

        protected Field CreateField(FieldCreateInfo baseInfo, string type, string config)
        {
            this._lock.AcquireWriterLock();
            try
            {
                if (string.IsNullOrEmpty(baseInfo.Code))
                {
                    throw new ArgumentNullException("baseInfo.Code");
                }
                if (string.IsNullOrEmpty(baseInfo.Name))
                {
                    throw new ArgumentNullException("baseInfo.Name");
                }

                if (this._fields.Any(x => x.Name == baseInfo.Name))
                {
                    throw new FieldNameRepeatException();
                }

                if (!string.IsNullOrEmpty(baseInfo.Code) && this._fields.Any(x => x.Code == baseInfo.Code))
                {
                    throw new FieldCodeRepeatException();
                }

                FieldModel model = new FieldModel
                {
                    id = Guid.NewGuid().ToString(),
                    objectId = this.ID,
                    code = baseInfo.Code,
                    name = baseInfo.Name,
                    tip = baseInfo.Tip,
                    required = baseInfo.Required,
                    unique = baseInfo.Unique,
                    type = type,
                    isSummary = baseInfo.IsSummary,
                    gridWidth = baseInfo.GridWidth
                };
                NHibernateHelper.CurrentSession.Save(model);
                NHibernateHelper.CurrentSession.Flush();

                Field field = this.CreateField(model);
                this.BindEvent(field);
                this._fields.Add(field);
                if (this.FieldCreated != null)
                {
                    this.FieldCreated(this, field);
                }
                return field;
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
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
            Field field = new UserField { UserManager = this.ColdewManager.OrgManager.UserManager, DefaultValueIsCurrent = model.defaultValueIsCurrent };
            this.FillFieldInfo(field, model);
            return field;
        }

        private Field CreateField(UserListFieldModel model)
        {
            Field field = new UserListField { UserManager = this.ColdewManager.OrgManager.UserManager, DefaultValueIsCurrent = model.defaultValueIsCurrent };
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
            Field field = new DropdownField { DefaultValue = model.defaultValue, SelectList = model.selectList };
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
            Field field = new DateField { DefaultValueIsToday = model.defaultValueIsToday};
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
            Field field = new MetadataField { RelatedObject = this.ColdewManager.ObjectManager.GetObjectById(model.objectId) };
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
            field.ColdewObject = this;
            field.GridWidth = model.gridWidth;
            field.IsSummary = model.isSummary;
            field.IsSystem = model.isSummary;
            field.Name = model.name;
            field.Required = model.required;
            field.Tip = model.tip;
            field.Type = model.type;
            field.Unique = model.unique;
        }

        public virtual Field CreateField(FieldModel model)
        {
            dynamic d_model = model;
            Field field = this.CreateField(d_model);
            return field;
        }

        private void BindEvent(Field field)
        {
            field.Modifying += new TEventHandler<Field, FieldModifyArgs>(Field_Modifying);
            field.Deleted += new TEventHandler<Field, User>(Field_Deleted);
        }

        void Field_Modifying(Field sender, FieldModifyArgs args)
        {
            if (sender.Name != args.Name && this._fields.Any(x => x.Name == args.Name))
            {
                throw new FieldNameRepeatException();
            }
        }

        public event TEventHandler<ColdewObject, Field> FieldDeleted;

        void Field_Deleted(Field field, User args)
        {
            this._lock.AcquireWriterLock();
            try
            {
                this._fields.Remove(field);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
            if (this.FieldDeleted != null)
            {
                this.FieldDeleted(this, field);
            }
        }

        public List<Field> GetFields()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<RelatedField> GetRelatedFields()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Where(x => x.Type == FieldType.RelatedField).Select(x => x as RelatedField).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Field> GetRequiredFields()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Where(x => x.Required).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Field> GetUniqueFields()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Where(x => x.Unique).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Field GetFieldById(string fieldId)
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Find(x => x.ID == fieldId);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Field GetFieldByCode(string fieldCode)
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Find(x => x.Code == fieldCode);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        internal void Load()
        {
            IList<FieldModel> models = NHibernateHelper.CurrentSession.QueryOver<FieldModel>().Where(x => x.objectId == this.ID).List();
            foreach (FieldModel model in models)
            {
                Field field = this.CreateField(model);
                this.BindEvent(field);
                this._fields.Add(field);
            }
            this.MetadataManager.Load();
            this.FavoriteManager.Load();
            this.GridViewManager.Load();
            this.FormManager.Load();
            this.MetadataPermission.EntityManager.Load();
            this.MetadataPermission.StrategyManager.Load();
            this.MetadataPermission.RelatedPermission.Load();
            this.ObjectPermission.Load();
            this.FieldPermission.Load();
        }

    }
}
