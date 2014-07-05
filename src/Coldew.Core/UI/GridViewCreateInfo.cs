using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core.UI
{
    public class GridViewCreateInfo
    {
        public GridViewCreateInfo(string code, string name, bool isShared, bool isSystem,
            MetadataFilter searcher, List<GridViewColumn> columns, Field orderField, User creator)
        {
            this.Creator = creator;
            this.Code = code;
            this.Name = name;
            this.IsShared = isShared;
            this.IsSystem = isSystem;
            this.Searcher = searcher;
            this.Columns = columns;
            this.OrderField = orderField;
        }

        public virtual User Creator { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsShared { set; get; }

        public virtual bool IsSystem { set; get; }

        public virtual List<GridViewColumn> Columns { set; get; }

        public virtual MetadataFilter Searcher { set; get; }

        public virtual Field OrderField { set; get; }

        public List<GridViewFooter> Footer { set; get; }
    }
}
