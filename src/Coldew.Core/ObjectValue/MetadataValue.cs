using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public abstract class MetadataValue 
    {
        public MetadataValue(dynamic value, Field field)
        {
            this.Value = value;
            this.Field = field;
        }

        public virtual dynamic Value { set; get; }

        public virtual Field Field { protected set; get; }

        public abstract dynamic OrderValue { get; }

        public abstract JToken PersistenceValue { get; }

        public abstract string ShowValue { get; }

        public abstract JToken JTokenValue { get; }

        public override bool Equals(object compareObj)
        {
            if (compareObj == null)
            {
                return false;
            }

            MetadataValue compareObjValue = compareObj as MetadataValue;
            if (compareObjValue == null)
            {
                return false;
            }

            if (this.Value == compareObjValue.Value)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return this.ShowValue;
        }
    }
}
