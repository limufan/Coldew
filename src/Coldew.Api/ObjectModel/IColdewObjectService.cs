﻿using System;
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

        FieldInfo GetField(int fieldId);

        void DeleteField(string opUserAccount, int fieldId);

        FieldInfo CreateStringField(string objectId, StringFieldCreateInfo createInfo);

        FieldInfo CreateDateField(string objectId, DateFieldCreateInfo createInfo);

        FieldInfo CreateNumberField(string objectId, NumberFieldCreateInfo createInfo);

        FieldInfo CreateTextField(string objectId, TextFieldCreateInfo createInfo);

        FieldInfo CreateDropdownField(string objectId, DropdownFieldCreateInfo createInfo);

        FieldInfo CreateRadioListField(string objectId, RadioListFieldCreateInfo createInfo);

        FieldInfo CreateCheckboxListField(string objectId, CheckboxListFieldCreateInfo createInfo);

        void ModifyStringField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue);

        void ModifyDateField(int fieldId, FieldModifyBaseInfo modifyInfo, bool defaultValueIsToday);

        void ModifyNumberField(int fieldId, FieldModifyBaseInfo modifyInfo, decimal? defaultValue, decimal? max, decimal? min, int precision);

        void ModifyTextField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue);

        void ModifyDropdownField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList);

        void ModifyRadioListField(int fieldId, FieldModifyBaseInfo modifyInfo, string defaultValue, List<string> selectList);

        void ModifyCheckboxListField(int fieldId, FieldModifyBaseInfo modifyInfo, List<string> defaultValues, List<string> selectList);
    }
}
