using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public abstract class GridViewColumn
    {
        public int Width { set; get; }

        public abstract MetadataValue GetValue(Metadata metadata);
    }

    public class GridViewFieldColumn : GridViewColumn
    {
        public GridViewFieldColumn(Field field)
        {
            this.Field = field;
            this.Width = field.GridWidth;
        }

        public Field Field { set; get; }

        public override MetadataValue GetValue(Metadata metadata)
        {
            MetadataValue value = metadata.GetValue(this.Field.Code);
            return value;
        }
    }

    public class GridViewRelatedColumn : GridViewColumn
    {
        public GridViewRelatedColumn(Field field, Field relatedField)
        {
            this.Field = field as MetadataField;
            this.RelatedField = relatedField;
            this.Width = 80;
        }

        public MetadataField Field { set; get; }

        public Field RelatedField { set; get; }

        public override MetadataValue GetValue(Metadata metadata)
        {
            return metadata.GetRelatedValue(this.Field, this.RelatedField);
        }
    }
}
