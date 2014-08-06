using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;


namespace Coldew.Core
{
    public abstract class ListField : Field
    {
        public ListField(FieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new StringMetadataValue(value.ToString(), this);
        }

    }
}
