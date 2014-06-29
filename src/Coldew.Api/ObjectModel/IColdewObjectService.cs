using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public interface IColdewObjectService
    {
        ColdewObjectInfo GetObjectById(string userAccount, string objectId);

        ColdewObjectInfo GetObjectByCode(string userAccount, string objectCode);

        List<ColdewObjectInfo> GetObjects(string userAccount);

        FieldInfo GetField(string fieldId);

        void DeleteField(string opUserAccount, string fieldId);

        FieldInfo CreateStringField(string objectId, StringFieldCreateInfo createInfo);

        FieldInfo CreateDateField(string objectId, DateFieldCreateInfo createInfo);

        FieldInfo CreateNumberField(string objectId, NumberFieldCreateInfo createInfo);

        FieldInfo CreateTextField(string objectId, TextFieldCreateInfo createInfo);

        FieldInfo CreateDropdownField(string objectId, DropdownFieldCreateInfo createInfo);

        FieldInfo CreateRadioListField(string objectId, RadioListFieldCreateInfo createInfo);

        FieldInfo CreateCheckboxListField(string objectId, CheckboxListFieldCreateInfo createInfo);

        void ModifyStringField(string fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue);

        void ModifyDateField(string fieldId, FieldModifyBaseInfo modifyInfo, bool defaultValueIsToday);

        void ModifyNumberField(string fieldId, FieldModifyBaseInfo modifyInfo, decimal? defaultValue, decimal? max, decimal? min, int precision);

        void ModifyTextField(string fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue);

        void ModifyDropdownField(string fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList);

        void ModifyRadioListField(string fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList);

        void ModifyCheckboxListField(string fieldId, FieldModifyBaseInfo modifyInfo, List<string> defaultValues, List<string> selectList);
    }
}
