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
    public class GridViewColumnMapper
    {
        ColdewObjectManager _objectManager;
        public GridViewColumnMapper(ColdewObjectManager coldewManger)
        {
            this._objectManager = coldewManger;
        }

        public GridViewColumn MapColumn(GridViewColumnModel model)
        {
            Field field = this._objectManager.GetFieldById(model.fieldId);
            return new GridViewColumn(field);
        }

        public GridViewColumnModel MapColumnModel(GridViewColumn column)
        {
            return new GridViewColumnModel { fieldId = column.Field.ID };
        }
    }

    public class GridViewColumn
    {
        public GridViewColumn(Field field)
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
