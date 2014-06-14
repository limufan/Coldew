using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.Organization;
using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FieldWebModel
    {
        public FieldWebModel(Field field, User user)
        {
            this.id = field.ID;
            this.code = field.Code;
            this.name = field.Name;
            this.required = field.Required;
            this.type = field.Type;
            this.typeName = field.TypeName;
            this.tip = field.Tip;
            this.unique = field.Unique;
            if (user != null)
            {
                this.permissionValue = field.ColdewObject.FieldPermission.GetPermission(user, field);
            }
        }

        public int id;

        public string code;

        public string name;

        public bool required;

        public string type;

        public string typeName;

        public string tip;

        public bool unique;

        public dynamic defaultValue;

        public FieldPermissionValue permissionValue;

        public static FieldWebModel Map(DateField field, User user)
        {
            return new DateFieldWebModel(field, user);
        }

        public static FieldWebModel Map(StringField field, User user)
        {
            return new StringFieldWebModel(field, user);
        }

        public static FieldWebModel Map(ListField field, User user)
        {
            return new ListFieldWebModel(field, user);
        }

        public static FieldWebModel Map(MetadataField field, User user)
        {
            return new MetadataFieldWebModel(field, user);
        }

        public static FieldWebModel Map(RelatedField field, User user)
        {
            return new RelatedFieldWebModel(field, user);
        }

        public static FieldWebModel Map(NumberField field, User user)
        {
            return new NumberFieldWebModel(field, user);
        }

        public static FieldWebModel Map(UserField field, User user)
        {
            return new UserFieldWebModel(field, user);
        }

        public static FieldWebModel Map(UserListField field, User user)
        {
            return new UserListFieldWebModel(field, user);
        }

        public static FieldWebModel Map(CheckboxListField field, User user)
        {
            return new CheckboxListFieldWebModel(field, user);
        }

        public static FieldWebModel Map(JsonField field, User user)
        {
            return new JsonFieldWebModel(field, user);
        }

        public static FieldWebModel Map(CodeField field, User user)
        {
            return new CodeFieldWebModel(field, user);
        }
    }

    [Serializable]
    public class DateFieldWebModel : FieldWebModel
    {
        public DateFieldWebModel(DateField field, User user)
            :base(field, user)
        {
            this.defaultValueIsToday = field.DefaultValueIsToday;
            if (this.defaultValueIsToday)
            {
                this.defaultValue = DateTime.Now.ToString("yyyy-MM-dd");
            }
            
        }

        public bool defaultValueIsToday;
    }

    [Serializable]
    public class ListFieldWebModel : FieldWebModel
    {
        public ListFieldWebModel(ListField field, User user)
            :base(field, user)
        {
            this.defaultValue = field.DefaultValue;
            this.selectList = field.SelectList;
        }

        public List<string> selectList;
    }

    [Serializable]
    public class MetadataFieldWebModel : FieldWebModel
    {
        public MetadataFieldWebModel(MetadataField field, User user)
            :base(field, user)
        {
            this.valueObjectName = field.RelatedObject.Name;
            this.valueObjectId = field.RelatedObject.ID;
        }

        public string valueObjectName;

        public string valueObjectId;
    }

    [Serializable]
    public class RelatedFieldWebModel : FieldWebModel
    {
        public RelatedFieldWebModel(RelatedField field, User user)
            :base(field, user)
        {
            this.relatedFieldCode = field.RelatedFieldCode;
            this.propertyCode = field.PropertyCode;
        }

        public string relatedFieldCode;

        public string propertyCode;
    }

    [Serializable]
    public class NumberFieldWebModel : FieldWebModel
    {
        public NumberFieldWebModel(NumberField field, User user)
            :base(field, user)
        {
            this.defaultValue = field.DefaultValue;
            this.max = field.Max;
            this.min = field.Min;
            this.precision = field.Precision;
        }

        public decimal? max;

        public decimal? min;

        public int precision;
    }

    [Serializable]
    public class UserFieldWebModel : FieldWebModel
    {
        public UserFieldWebModel(UserField field, User user)
            :base(field, user)
        {
            if (field.DefaultValueIsCurrent)
            {
                this.defaultValue = new UserWebModel(user);
            }
        }
    }

    [Serializable]
    public class UserListFieldWebModel : FieldWebModel
    {
        public UserListFieldWebModel(UserListField field, User user)
            :base(field, user)
        {
            this.defaultValueIsCurrent = field.DefaultValueIsCurrent;
            if (field.DefaultValueIsCurrent)
            {
                this.defaultValue = new UserWebModel(user);
            }
        }

        public bool defaultValueIsCurrent;
    }

    [Serializable]
    public class CheckboxListFieldWebModel : FieldWebModel
    {
        public CheckboxListFieldWebModel(CheckboxListField field, User user)
            :base(field, user)
        {
            this.defaultValue = field.DefaultValues;
            this.selectList = field.SelectList;
        }

        public List<string> selectList;
    }

    [Serializable]
    public class StringFieldWebModel : FieldWebModel
    {
        public StringFieldWebModel(StringField field, User user)
            :base(field, user)
        {
            this.defaultValue = field.DefaultValue;
            this.suggestions = field.Suggestions;
        }

        public List<string> suggestions;
    }

    [Serializable]
    public class JsonFieldWebModel : FieldWebModel
    {
        public JsonFieldWebModel(JsonField field, User user)
            :base(field, user)
        {
            
        }

    }

    [Serializable]
    public class CodeFieldWebModel : FieldWebModel
    {
        public CodeFieldWebModel(CodeField field, User user)
            : base(field, user)
        {
            this.defaultValue = field.ColdewObject.MetadataManager.GenerateCode(field.Code);
        }
    }
}
