using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class FieldModel
    {
        public string id;

        public string objectId;

        public string code;

        public string name;

        public string tip;

        public bool required;

        public bool isSystem;

        public bool isSummary;

        public int gridWidth;

        public string type;

        public bool unique;
    }

    public class StringFieldModel : FieldModel
    {
        public string defaultValue;

        public List<string> suggestions;
    }

    public class TextFieldModel : FieldModel
    {
        public string defaultValue;
    }

    public class UserFieldModel : FieldModel
    {
        public bool defaultValueIsCurrent;
    }

    public class UserListFieldModel : FieldModel
    {
        public bool defaultValueIsCurrent;
    }

    public class NumberFieldModel : FieldModel
    {
        public decimal? defaultValue;

        public decimal? max;

        public decimal? min;

        public int precision;
    }

    public class DropdownListFieldModel : FieldModel
    {
        public string defaultValue;

        public List<string> selectList;
    }

    public class RadioListFieldModel : FieldModel
    {
        public string defaultValue;

        public List<string> selectList;
    }

    public class DateFieldModel : FieldModel
    {
        public bool defaultValueIsToday;
    }

    public class CheckboxListFieldModel : FieldModel
    {
        public List<string> defaultValue;

        public List<string> selectList;
    }

    public class JsonFieldModel : FieldModel
    {
        
    }

    public class CodeFieldModel : FieldModel
    {
        public string format;
    }

    public class MetadataFieldModel : FieldModel
    {
        public string objectId;
    }

    public class RelatedFieldModel : FieldModel
    {
        public string relatedFieldCode;

        public string propertyCode;
    }
}
