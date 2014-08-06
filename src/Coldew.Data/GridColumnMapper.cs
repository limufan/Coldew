using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;

namespace Coldew.Data
{
    public class GridColumnMapper
    {
        ColdewObjectManager _objectManager;
        public GridColumnMapper(ColdewObjectManager coldewManger)
        {
            this._objectManager = coldewManger;
        }

        public GridColumn MapColumn(GridViewColumnModel model)
        {
            Field field = this._objectManager.GetFieldById(model.fieldId);
            return new GridColumn(field);
        }

        public GridViewColumnModel MapColumnModel(GridColumn column)
        {
            return new GridViewColumnModel { fieldId = column.Field.ID };
        }
    }
}
