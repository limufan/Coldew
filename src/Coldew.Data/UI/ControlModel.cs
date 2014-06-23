using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data.UI
{
    public class ControlModel
    {

    }

    public class InputModel : ControlModel
    {
        public string fieldCode;

        public int width;

        public bool required;

        public bool isReadonly;
    }

    public class RowModel : ControlModel
    {
        public List<ControlModel> children;
    }

    public class FieldsetModel : ControlModel
    {
        public string title;
    }

    public class GridModel : ControlModel
    {
        public string addFormId;

        public string editFormId;

        public int fieldId;

        public List<GridViewColumnModel> columns;

        public int width;

        public bool required;

        public bool isReadonly;

        public List<GridViewFooterModel> footer;
    }

    public class GridViewFooterModel
    {
        public string fieldCode;

        public int valueType;

        public string value;
    }
}
