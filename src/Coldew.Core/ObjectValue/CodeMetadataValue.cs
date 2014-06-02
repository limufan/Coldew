using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class CodeMetadataValue : MetadataValue
    {
        public CodeMetadataValue(string value, Field field)
            :base(value, field)
        {

        }

        public string Code
        {
            get
            {
                return this.Value;
            }
        }

        public override JToken PersistenceValue
        {
            get { return this.Code; }
        }

        public override string ShowValue
        {
            get { return this.Code; }
        }

        public override dynamic OrderValue
        {
            get { return this.Code; }
        }

        public override JToken JTokenValue
        {
            get
            {
                return this.Code;
            }
        }
    }
}
