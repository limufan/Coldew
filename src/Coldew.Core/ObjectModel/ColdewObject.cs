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
        public ColdewObjectManager ObjectManager { set; get; }

        public ColdewObject(string id, string code, string name, bool isSystem, int index, Field nameField, ColdewObjectManager objectManager)
        {
            this.ID = id;
            this.Name = name;
            this.Code = code;
            this.IsSystem = isSystem;
            this.Index = index;
            this.NameField = nameField;
            this._fields = new List<Field>();
            this._lock = new ReaderWriterLock();
            this.ObjectManager = objectManager;
            this.ColdewManager = objectManager.ColdewManager;
            this.MetadataManager = this.CreateMetadataManager(this.ColdewManager);
            this.FavoriteManager = new MetadataFavoriteManager(this);
            this.GridViewManager = this.CreateGridViewManager(this.ColdewManager);
            this.FormManager = this.CreateFormManager(this.ColdewManager);
            this.ObjectPermission = new ObjectPermissionManager(this);
            this.MetadataPermission = new MetadataPermissionManager(this);
            this.FieldPermission = new FieldPermissionManager(this);
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
            this.NameField = field;
            this.UpldateToDb();
        }

        internal void UpldateToDb()
        {
            try
            {
                if (this.Modfied != null)
                {
                    this.Modfied(this);
                }
            }
            catch
            {
                this.ColdewManager.Init();
                throw;
            }
        }

        public MetadataManager MetadataManager { private set; get; }

        public MetadataFavoriteManager FavoriteManager { private set; get; }

        public GridViewManager GridViewManager { private set; get; }

        public FormManager FormManager { private set; get; }

        public Field NameField { internal set; get; }

        public Field CreateStringField(StringFieldCreateInfo createInfo)
        {
            StringField field = new StringField{ DefaultValue = createInfo.DefaultValue, Suggestions = createInfo.Suggestions };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        private void FillFieldInfo(Field field, FieldCreateInfo createInfo)
        {
            field.ID = Guid.NewGuid().ToString();
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
            this.CreateField(field);
            return field;
        }

        public Field CreateTextField(TextFieldCreateInfo createInfo)
        {
            TextField field = new TextField{ DefaultValue = createInfo.DefaultValue };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateDateField(DateFieldCreateInfo createInfo)
        {
            DateField field = new DateField { DefaultValueIsToday = createInfo.DefaultValueIsToday };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateNumberField(NumberFieldCreateInfo createInfo)
        {
            NumberField field = new NumberField { DefaultValue = createInfo.DefaultValue, Max = createInfo.Max, Min = createInfo.Min, Precision = createInfo.Precision };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateCheckboxListField(CheckboxListFieldCreateInfo createInfo)
        {
            CheckboxListField field = new CheckboxListField { DefaultValue = createInfo.DefaultValues, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateRadioListField(RadioListFieldCreateInfo createInfo)
        {
            RadioListField field = new RadioListField { DefaultValue = createInfo.DefaultValue, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateDropdownField(DropdownFieldCreateInfo createInfo)
        {
            DropdownListField field = new DropdownListField { DefaultValue = createInfo.DefaultValue, SelectList = createInfo.SelectList };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateUserField(UserFieldCreateInfo createInfo)
        {
            UserField field = new UserField { OrgManager = this.ColdewManager.OrgManager, DefaultValueIsCurrent = createInfo.DefaultValueIsCurrent };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateUserListField(UserListFieldCreateInfo createInfo)
        {
            UserListField field = new UserListField { OrgManager = this.ColdewManager.OrgManager, DefaultValueIsCurrent = createInfo.DefaultValueIsCurrent };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateMetadataField(MetadataFieldCreateInfo createInfo)
        {
            MetadataField field = new MetadataField { RelatedObject = createInfo.RelatedObject };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateRelatedField(RelatedFieldCreateInfo createInfo)
        {
            RelatedField field = new RelatedField { RelatedField1 = createInfo.RelatedField, ValueField = createInfo.ValueField };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateField(CodeFieldCreateInfo createInfo)
        {
            CodeField field = new CodeField { Format = createInfo.Format };
            this.FillFieldInfo(field, createInfo);
            this.CreateField(field);
            return field;
        }

        public event TEventHandler<ColdewObject> Modfied;

        public event TEventHandler<ColdewObject, Field> FieldCreated;

        protected void CreateField(Field field)
        {
            this._lock.AcquireWriterLock();
            try
            {
                if (string.IsNullOrEmpty(field.Code))
                {
                    throw new ArgumentNullException("baseInfo.Code");
                }
                if (string.IsNullOrEmpty(field.Name))
                {
                    throw new ArgumentNullException("baseInfo.Name");
                }

                if (this._fields.Any(x => x.Name == field.Name))
                {
                    throw new FieldNameRepeatException();
                }

                if (!string.IsNullOrEmpty(field.Code) && this._fields.Any(x => x.Code == field.Code))
                {
                    throw new FieldCodeRepeatException();
                }
                field.ColdewObject = this;
                this._fields.Add(field);
                this.BindEvent(field);
                this.UpldateToDb();
                if (this.FieldCreated != null)
                {
                    this.FieldCreated(this, field);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        private void BindEvent(Field field)
        {
            field.Modifying += new TEventHandler<Field, FieldModifyArgs>(Field_Modifying);
            field.Modified += Field_Modified;
            field.Deleted += new TEventHandler<Field, User>(Field_Deleted);
        }

        void Field_Modifying(Field sender, FieldModifyArgs args)
        {
            if (sender.Name != args.Name && this._fields.Any(x => x.Name == args.Name))
            {
                throw new FieldNameRepeatException();
            }
        }

        void Field_Modified(Field sender, FieldModifyArgs args)
        {
            this.UpldateToDb();
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
            this.UpldateToDb();
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

        internal void SetFields(List<Field> fields)
        {
            fields.ForEach(x => 
            {
                x.ColdewObject = this;
                BindEvent(x);
            });
            this._fields = fields;
            
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
            this.FavoriteManager.Load();
            this.FormManager.Load();
            this.MetadataPermission.EntityManager.Load();
            this.MetadataPermission.StrategyManager.Load();
            this.MetadataPermission.RelatedPermission.Load();
            this.ObjectPermission.Load();
            this.FieldPermission.Load();
        }

    }
}
