using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class FieldBaseInfo
    {
        public string Code { set; get; }

        public string Name { set; get; }

        public string Tip { set; get; }

        public bool Required { set; get; }

        public bool IsSystem { set; get; }

        public int GridWidth { set; get; }

        public bool IsSummary { set; get; }

        public bool Unique { set; get; }

        public string Type { set; get; }
    }

    public class FieldNewInfo : FieldBaseInfo
    {
        public string ID { set; get; }

        public ColdewObject ColdewObject { set; get; }
    }

    public class FieldCreateInfo : FieldBaseInfo
    {
        public FieldCreateInfo(string code, string name)
        {
            this.Code = code;
            this.Name = name;
            this.GridWidth = 80;
        }
    }

    public class CheckboxListFieldCreateInfo : FieldCreateInfo
    {
        public CheckboxListFieldCreateInfo(string code, string name, List<string> selectList)
            : base(code, name)
        {
            this.SelectList = selectList;
        }

        public List<string> DefaultValues { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class CheckboxListFieldNewInfo : FieldNewInfo
    {
        public List<string> DefaultValues { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class CodeFieldCreateInfo : FieldCreateInfo
    {
        public CodeFieldCreateInfo(string code, string name, string format)
            : base(code, name)
        {
            this.Format = format;
        }

        public string Format { set; get; }
    }

    public class CodeFieldNewInfo : FieldNewInfo
    {
        public string Format { set; get; }
    }

    public class DateFieldCreateInfo : FieldCreateInfo
    {
        public DateFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public bool DefaultValueIsToday { set; get; }
    }

    public class DateFieldNewInfo : FieldNewInfo
    {
        public bool DefaultValueIsToday { set; get; }
    }

    public class DropdownFieldCreateInfo : FieldCreateInfo
    {
        public DropdownFieldCreateInfo(string code, string name, List<string> selectList)
            : base(code, name)
        {
            this.SelectList = selectList;
        }

        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class DropdownFieldNewInfo : FieldNewInfo
    {
        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class NumberFieldCreateInfo : FieldCreateInfo
    {
        public NumberFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public decimal? DefaultValue { set; get; }

        public decimal? Max { set; get; }

        public decimal? Min { set; get; }

        public int Precision { set; get; }
    }

    public class NumberFieldNewInfo : FieldNewInfo
    {
        public decimal? DefaultValue { set; get; }

        public decimal? Max { set; get; }

        public decimal? Min { set; get; }

        public int Precision { set; get; }
    }

    public class RadioListFieldCreateInfo : FieldCreateInfo
    {
        public RadioListFieldCreateInfo(string code, string name, List<string> selectList)
            : base(code, name)
        {
            this.SelectList = selectList;
        }

        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class RadioListFieldNewInfo : FieldNewInfo
    {
        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }
    }

    public class RelatedFieldCreateInfo : FieldCreateInfo
    {
        public RelatedFieldCreateInfo(string code, string name, Field relatedField, Field valueField)
            : base(code, name)
        {
            this.RelatedField = relatedField;
            this.ValueField = valueField;
        }

        public Field RelatedField { set; get; }

        public Field ValueField { set; get; }
    }

    public class RelatedFieldNewInfo : FieldNewInfo
    {
        public Field RelatedField { set; get; }

        public Field ValueField { set; get; }
    }

    public class StringFieldCreateInfo : FieldCreateInfo
    {
        public StringFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }
    }

    public class StringFieldNewInfo : FieldNewInfo
    {
        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }
    }

    public class TextFieldCreateInfo : FieldCreateInfo
    {
        public TextFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public string DefaultValue { set; get; }
    }

    public class TextFieldNewInfo : FieldNewInfo
    {
        public string DefaultValue { set; get; }
    }

    public class UserFieldCreateInfo : FieldCreateInfo
    {
        public UserFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public bool DefaultValueIsCurrent { set; get; }
    }

    public class UserFieldNewInfo : FieldNewInfo
    {
        public bool DefaultValueIsCurrent { set; get; }
    }

    public class UserListFieldCreateInfo : FieldCreateInfo
    {
        public UserListFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

        public bool DefaultValueIsCurrent { set; get; }
    }

    public class UserListFieldNewInfo : FieldNewInfo
    {
        public bool DefaultValueIsCurrent { set; get; }
    }

    public class MetadataFieldCreateInfo : FieldCreateInfo
    {
        public MetadataFieldCreateInfo(string code, string name, ColdewObject relatedObject)
            : base(code, name)
        {
            this.RelatedObject = relatedObject;
        }

        public ColdewObject RelatedObject { set; get; }
    }

    public class MetadataFieldNewInfo : FieldNewInfo
    {
        public ColdewObject RelatedObject { set; get; }
    }

    public class JsonFieldCreateInfo : FieldCreateInfo
    {
        public JsonFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }
    }

    public class JsonFieldNewInfo : FieldNewInfo
    {
        
    }

    public class FieldChangeInfo : FieldBaseInfo
    {
        public FieldChangeInfo(Field field)
        {
            ClassPropertyHelper.ChangeProperty(field, this);
        }
    }
}
