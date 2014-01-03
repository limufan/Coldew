using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class MetadataProperty
    {
        public MetadataProperty(MetadataValue value)
        {
            this.Field = value.Field;
            this.Value = value;
        }

        public Field Field { private set; get; }

        public MetadataValue Value { private set; get; }

        public PropertyInfo Map(User user)
        {
            bool canview = this.Field.CanView(user);
            dynamic showValue = "";
            dynamic editValue = "";
            if (canview)
            {
                showValue = this.Value.ShowValue;
                editValue = this.Value.JTokenValue;
            }
            else
            {
                showValue = "*****";
                editValue = "*****";
            }

            return new PropertyInfo
            {
                Code = this.Field.Code,
                FieldType = this.Field.Type,
                ShowValue = showValue,
                EditValue = editValue
            };
        }
    }
}
