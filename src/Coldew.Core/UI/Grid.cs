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
            this.Columns = new List<GridColumn>();
        }

        public List<GridColumn> Columns { set; get; }

        public List<GridFooter> Footer { set; get; }
    }
}
