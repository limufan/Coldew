using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class GridViewChangeInfo
    {
        public GridViewChangeInfo(GridView gridView)
        {
            this.Name = gridView.Name;
            this.IsShared = gridView.IsShared;
            this.Filter = gridView.Filter;
            this.Columns = gridView.Columns;
        }

        public string Name { set; get; }

        public bool IsShared { set; get; }

        public MetadataFilter Filter { set; get; }

        public List<GridColumn> Columns { set; get; }
    }
}
