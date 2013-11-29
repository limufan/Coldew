using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class ColdewObjectService : IColdewObjectService
    {
        ColdewManager _coldewManager;

        public ColdewObjectService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public ColdewObjectInfo GetObjectById(string userAccount, string objectId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            if (form != null)
            {
                return form.Map(user);
            }
            return null;
        }

        public ColdewObjectInfo GetObjectByCode(string userAccount, string objectCode)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            if (form != null)
            {
                return form.Map(user);
            }
            return null;
        }

        public List<ColdewObjectInfo> GetObjects(string userAccount)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            List<ColdewObject> objects = this._coldewManager.ObjectManager.GetObjects();
            return objects.Where(x => {
                return x.ObjectPermission.HasValue(user, ObjectPermissionValue.View);
            }).Select(x => x.Map(user)).ToList();
        }

        public FieldInfo CreateStringField(string objectId, StringFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            
            Field field = form.CreateStringField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateTextField(string objectId, TextFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateTextField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateDropdownField(string objectId, DropdownFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateDropdownField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateRadioListField(string objectId, RadioListFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateRadioListField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateCheckboxListField(string objectId, CheckboxListFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateCheckboxListField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateDateField(string objectId, DateFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateDateField(createInfo);
            return field.Map();
        }

        public FieldInfo CreateNumberField(string objectId, NumberFieldCreateInfo createInfo)
        {
            ColdewObject form = this._coldewManager.ObjectManager.GetObjectById(objectId);
            Field field = form.CreateNumberField(createInfo);
            return field.Map();
        }

        public void ModifyDateField(int fieldId, FieldModifyBaseInfo modifyInfo, bool defaultValueIsToday)
        {
            DateField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as DateField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValueIsToday);
            }
        }

        public void ModifyNumberField(int fieldId, FieldModifyBaseInfo modifyInfo, decimal? defaultValue, decimal? max, decimal? min, int precision)
        {
            NumberField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as NumberField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValue, max, min, precision);
            }
        }

        public void ModifyStringField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue)
        {
            StringField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as StringField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValue);
            }
        }

        public void ModifyTextField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue)
        {
            TextField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as TextField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValue);
            }
        }

        public void ModifyDropdownField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList)
        {
            DropdownField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as DropdownField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValue, selectList);
            }
        }

        public void ModifyRadioListField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList)
        {
            RadioListField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as RadioListField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValue, selectList);
            }
        }

        public void ModifyCheckboxListField(int fieldId, FieldModifyBaseInfo modifyInfo, List<string> defaultValues, List<string> selectList)
        {
            CheckboxListField field = this._coldewManager.ObjectManager.GetFieldById(fieldId) as CheckboxListField;
            if (field != null)
            {
                field.Modify(modifyInfo, defaultValues, selectList);
            }
        }

        public FieldInfo GetField(int fieldId)
        {
            Field field = this._coldewManager.ObjectManager.GetFieldById(fieldId);
            if (field != null)
            {
                return field.Map();
            }
            return null;
        }

        public void DeleteField(string opUserAccount, int fieldId)
        {
            User opUser = this._coldewManager.OrgManager.UserManager.GetUserByAccount(opUserAccount);
            Field field = this._coldewManager.ObjectManager.GetFieldById(fieldId);
            if (field != null)
            {
                field.Delete(opUser);
            }
        }
    }
}
