using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FieldWebModel
    {
        public int id;

        public string code;

        public string name;

        public bool required;

        public string type;

        public string typeName;

        public string tip;

        public bool unique;

        public FieldPermissionValue permissionValue;
    }

    [Serializable]
    public class DateFieldWebModel : FieldWebModel
    {
        public bool defaultValueIsToday;

        public string DefaultValue
        {
            get
            {
                if (this.defaultValueIsToday)
                {
                    return DateTime.Now.ToString("yyyy-MM-dd");
                }
                return "";
            }
        }
    }

    [Serializable]
    public class ListFieldWebModel : FieldWebModel
    {
        public string defaultValue;

        public List<string> selectList;
    }

    [Serializable]
    public class MetadataFieldWebModel : FieldWebModel
    {
        public string valueObjectName;

        public string valueObjectId;
    }

    [Serializable]
    public class NumberFieldWebModel : FieldWebModel
    {
        public decimal? defaultValue;

        public decimal? max;

        public decimal? min;

        public int precision;
    }

    [Serializable]
    public class UserFieldWebModel : FieldWebModel
    {
        public bool defaultValueIsCurrent;
    }

    [Serializable]
    public class UserListFieldWebModel : FieldWebModel
    {

        public bool defaultValueIsCurrent;
    }

    [Serializable]
    public class CheckboxFieldWebModel : FieldWebModel
    {
        public List<string> defaultValues;

        public List<string> selectList;
    }

    [Serializable]
    public class StringFieldWebModel : FieldWebModel
    {
        public string defaultValue;

        public List<string> suggestions;
    }
}
