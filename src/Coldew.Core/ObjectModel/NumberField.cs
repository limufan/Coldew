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
    public class NumberField : Field
    {
        public NumberField(NumberFieldNewInfo newInfo)
            :base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.Number; }
        }

        public override string TypeName
        {
            get { return "数字"; }
        }

        public decimal? DefaultValue { set; get; }

        public decimal? Max { set; get; }

        public decimal? Min { set; get; }

        public int Precision { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new NumberMetadataValue(null, this);
            }
            decimal number;
            if (decimal.TryParse(value.ToString(), out number))
            {
                return new NumberMetadataValue(number, this);
            }
            else
            {
                throw new ColdewException("创建NumberMetadataValue出错,value:" + value);
            }
        }
    }
}
