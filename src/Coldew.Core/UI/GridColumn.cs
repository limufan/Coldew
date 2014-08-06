using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class GridColumn
    {
        public GridColumn(Field field)
        {
            this.Field = field;
            this.Width = field.GridWidth;
        }

        public Field Field { set; get; }

        public int Width { set; get; }

        public MetadataValue GetValue(Metadata metadata)
        {
            MetadataValue value = metadata.GetValue(this.Field.Code);
            return value;
        }
    }
}
