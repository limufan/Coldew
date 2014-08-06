using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Coldew.Api;
using Newtonsoft.Json;
using log4net.Util;
using Coldew.Core.Organization;
using Coldew.Api.Exceptions;
using Coldew.Core.UI;
using Coldew.Core.Permission;



namespace Coldew.Core
{
    public class ColdewObject
    {
        ReaderWriterLock _lock;
        private List<Field> _fields;
        public ColdewObjectManager ObjectManager { set; get; }

        public ColdewObject(ColdewObjectNewInfo newInfo)
        {
            ClassPropertyHelper.ChangeProperty(newInfo, this);
            this._fields = new List<Field>();
            this._permissions = new List<ObjectPermission>();
            this._lock = new ReaderWriterLock();
            this.ColdewManager = newInfo.ObjectManager.ColdewManager;
            this.MetadataManager = this.CreateMetadataManager(this.ColdewManager);
            this.FavoriteManager = new MetadataFavoriteManager(this);
            this.GridViewManager = this.CreateGridViewManager(this.ColdewManager);
            this.FormManager = this.CreateFormManager(this.ColdewManager);
            this.MetadataPermission = new MetadataPermissionManager(this);
            this.FieldPermission = new FieldPermissionManager(this);
        }

        public ColdewManager ColdewManager { set; get; }

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

        public virtual MetadataPermissionManager MetadataPermission { set; get; }

        public virtual FieldPermissionManager FieldPermission { set; get; }

        public string ID { set; get; }

        public string Code { set; get; }

        public string Name { set; get; }

        public bool IsSystem { set; get; }

        public int Index { set; get; }

        public void SetNameField(Field field)
        {
            this.NameField = field;
            this.OnChanged();
        }

        private void OnChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this);
            }
        }

        public MetadataManager MetadataManager { set; get; }

        public MetadataFavoriteManager FavoriteManager { set; get; }

        public GridViewManager GridViewManager { set; get; }

        public FormManager FormManager { set; get; }

        public Field NameField { set; get; }

        public Field CreateStringField(StringFieldCreateInfo createInfo)
        {
            StringFieldNewInfo newInfo = new StringFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            StringField field = new StringField(newInfo);
            this.CreateField(field);
            return field;
        }

        private void FillFieldNewInfo(FieldCreateInfo createInfo, FieldNewInfo newInfo)
        {
            ClassPropertyHelper.ChangeProperty(createInfo, newInfo);
            newInfo.ID = Guid.NewGuid().ToString();
            newInfo.ColdewObject = this;
        }

        public Field CreateJsonField(JsonFieldCreateInfo createInfo)
        {
            JsonFieldNewInfo newInfo = new JsonFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            JsonField field = new JsonField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateTextField(TextFieldCreateInfo createInfo)
        {
            TextFieldNewInfo newInfo = new TextFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            TextField field = new TextField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateDateField(DateFieldCreateInfo createInfo)
        {
            DateFieldNewInfo newInfo = new DateFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            DateField field = new DateField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateNumberField(NumberFieldCreateInfo createInfo)
        {
            NumberFieldNewInfo newInfo = new NumberFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            NumberField field = new NumberField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateCheckboxListField(CheckboxListFieldCreateInfo createInfo)
        {
            CheckboxListFieldNewInfo newInfo = new CheckboxListFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            CheckboxListField field = new CheckboxListField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateRadioListField(RadioListFieldCreateInfo createInfo)
        {
            RadioListFieldNewInfo newInfo = new RadioListFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            RadioListField field = new RadioListField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateDropdownField(DropdownFieldCreateInfo createInfo)
        {
            DropdownFieldNewInfo newInfo = new DropdownFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            DropdownListField field = new DropdownListField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateUserField(UserFieldCreateInfo createInfo)
        {
            UserFieldNewInfo newInfo = new UserFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            UserField field = new UserField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateUserListField(UserListFieldCreateInfo createInfo)
        {
            UserListFieldNewInfo newInfo = new UserListFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            UserListField field = new UserListField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateMetadataField(MetadataFieldCreateInfo createInfo)
        {
            MetadataFieldNewInfo newInfo = new MetadataFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            MetadataField field = new MetadataField(newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateRelatedField(RelatedFieldCreateInfo createInfo)
        {
            RelatedFieldNewInfo newInfo = new RelatedFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            RelatedField field = new RelatedField (newInfo);
            this.CreateField(field);
            return field;
        }

        public Field CreateField(CodeFieldCreateInfo createInfo)
        {
            CodeFieldNewInfo newInfo = new CodeFieldNewInfo();
            this.FillFieldNewInfo(createInfo, newInfo);
            CodeField field = new CodeField(newInfo);
            this.CreateField(field);
            return field;
        }

        public event TEventHandler<ColdewObject> Changed;

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
                this._fields.Add(field);
                this.BindEvent(field);
                this.OnChanged();
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
            field.Changing += new TEventHandler<Field, FieldChangeInfo>(Field_Modifying);
            field.Changed += Field_Modified;
            field.Deleted += new TEventHandler<Field, User>(Field_Deleted);
        }

        void Field_Modifying(Field sender, FieldChangeInfo args)
        {
            if (sender.Name != args.Name && this._fields.Any(x => x.Name == args.Name))
            {
                throw new FieldNameRepeatException();
            }
        }

        void Field_Modified(Field sender, FieldChangeInfo args)
        {
            this.OnChanged();
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
            this.OnChanged();
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

        public void SetFields(List<Field> fields)
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

        public List<CodeField> GetCodeFields()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._fields.Where(x => x is CodeField).Select(x => x as CodeField).ToList();
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

        List<ObjectPermission> _permissions;

        public List<ObjectPermission> GetPermissions()
        {
            return this._permissions.ToList();
        }

        public bool HasPerm(User user, ObjectPermissionValue value)
        {
            foreach (ObjectPermission permission in this._permissions)
            {
                if (permission.Member.Contains(user) && permission.Value.HasFlag(value))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddPermission(ObjectPermission permission)
        {
            this._permissions.Add(permission);
        }

        public void AddPermission(List<ObjectPermission> permissions)
        {
            this._permissions.AddRange(permissions);
        }

        public ObjectPermissionValue GetPermission(User user)
        {
            int value = 0;
            foreach (ObjectPermission permission in this._permissions)
            {
                if (permission.Member.Contains(user))
                {
                    value = value | (int)permission.Value;
                }
            }
            return (ObjectPermissionValue)value;
        }
    }
}
