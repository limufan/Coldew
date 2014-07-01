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
    }
}
