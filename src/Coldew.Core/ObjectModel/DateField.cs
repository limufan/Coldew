using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Newtonsoft.Json;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class DateField : Field
    {
        public DateField(DateFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.Date; }
        }

        public override string TypeName
        {
            get { return "日期"; }
        }

        public bool DefaultValueIsToday { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new DateMetadataValue(null, this);
            }
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return new DateMetadataValue(date, this);
            }
            else
            {
                throw new ColdewException("创建DateMetadataValue出错,value:" + value);
            }
        }
    }
}
