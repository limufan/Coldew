using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core
{
    public class FieldCreateInfo
    {
        public FieldCreateInfo(string code, string name)
        {
            this.Code = code;
            this.Name = name;
            this.GridWidth = 80;
        }

        public FieldCreateInfo(string code, string name, string tip, bool required, bool isSystem)
        {
            this.Code = code;
            this.Name = name;
            this.Tip = tip;
            this.Required = required;
            this.IsSystem = isSystem;
            this.GridWidth = 80;
        }

        public string Code { set; get; }

        public string Name { set; get; }

        public string Tip { set; get; }

        public bool Required { set; get; }

        public bool Unique { set; get; }

        public bool IsSystem { set; get; }

        public bool IsSummary { set; get; }

        public int GridWidth { set; get; }
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

    public class CodeFieldCreateInfo : FieldCreateInfo
    {
        public CodeFieldCreateInfo(string code, string name, string format)
            : base(code, name)
        {
            this.Format = format;
        }

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

    public class RelatedFieldCreateInfo : FieldCreateInfo
    {
        public RelatedFieldCreateInfo(string code, string name, string relatedFieldCode, string propertyCode)
            : base(code, name)
        {
            this.RelatedFieldCode = relatedFieldCode;
            this.PropertyCode = propertyCode;
        }

        public string RelatedFieldCode { set; get; }

        public string PropertyCode { set; get; }
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

    public class TextFieldCreateInfo : FieldCreateInfo
    {
        public TextFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

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

    public class UserListFieldCreateInfo : FieldCreateInfo
    {
        public UserListFieldCreateInfo(string code, string name)
            : base(code, name)
        {

        }

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

    public class JsonFieldCreateInfo : FieldCreateInfo
    {
        public JsonFieldCreateInfo(string code, string name)
            : base(code, name)
        {
            
        }
    }
}
