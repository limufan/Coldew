using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;


namespace Coldew.Core
{
    public class NumberField : Field
    {
        internal NumberField()
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

        public void Modify(FieldModifyBaseInfo modifyInfo, decimal? defaultValue, decimal? max, decimal? min, int precision)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = modifyInfo.Name, Required = modifyInfo.Required };

            this.OnModifying(args);

            this.Name = modifyInfo.Name;
            this.Required = modifyInfo.Required;
            this.DefaultValue = defaultValue;
            this.Max = max;
            this.Min = min;
            this.Precision = precision;

            this.OnModifyed(args);
        }

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
