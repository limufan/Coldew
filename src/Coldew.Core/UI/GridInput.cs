using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core.UI
{
    public class GridInput : Input
    {
        public GridInput(Field field, List<GridColumn> columns, Form editForm, Form addForm)
            :base(field)
        {
            this.Columns = columns;
            this.AddForm = addForm;
            this.EditForm = editForm;
            this.Field = field;
        }

        public Form EditForm { set; get; }

        public Form AddForm { set; get; }

        public List<GridColumn> Columns { set; get; }

        public bool Editable { set; get; }

        public List<GridFooter> Footer { set; get; }
    }
}
