using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core.UI
{
    public class Grid : Control
    {
        public Grid(Field field, List<GridViewColumn> columns, Form editForm, Form addForm)
        {
            this.Columns = columns;
            this.AddForm = addForm;
            this.EditForm = editForm;
            this.Field = field;
        }

        public Field Field { set; get; }

        public Form EditForm { set; get; }

        public Form AddForm { set; get; }

        public List<GridViewColumn> Columns { set; get; }

        public int Width { set; get; }

        public bool Required { set; get; }

        public bool IsReadonly { set; get; }

        public List<GridViewFooterInfo> Footer { set; get; }
    }
}
