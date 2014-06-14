using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class NumberMetadataValue : MetadataValue
    {
        NumberField _field;
        public NumberMetadataValue(decimal? value, Field field)
            : base(value, field)
        {
            this._field = field as NumberField;
        }

        public decimal? Number
        {
            get
            {
                return this.Value;
            }
        }

        public override JToken PersistenceValue
        {
            get 
            {
                return this.ShowValue; 
            }
        }

        public override string ShowValue
        {
            get 
            {
                if (this.Number.HasValue)
                {
                    return Math.Round(this.Number.Value, this._field.Precision).ToString(); 
                }
                return "";
            }
        }

        public override dynamic OrderValue
        {
            get { return this.Number; }
        }

        public override JToken JTokenValue
        {
            get
            {
                return this.Number;
            }
        }
    }
}
