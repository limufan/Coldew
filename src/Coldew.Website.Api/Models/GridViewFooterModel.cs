using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class GridViewFooterModel
    {
        public GridViewFooterModel(GridFooter footer)
        {
            this.columnName = footer.FieldCode;
            this.value = footer.Value;
            this.valueType = footer.ValueType.ToString().ToLower();
        }

        public string columnName;

        public string valueType;

        public string value;
    }
}
