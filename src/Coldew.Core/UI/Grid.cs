using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class Grid : Control
    {
        public Grid()
        {
            this.Columns = new List<GridViewColumn>();
        }

        public List<GridViewColumn> Columns { set; get; }

        public bool Editable { set; get; }

        public List<GridFooter> Footer { set; get; }
    }
}
