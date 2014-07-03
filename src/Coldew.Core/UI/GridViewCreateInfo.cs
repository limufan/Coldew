using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core.UI
{
    public class GridViewCreateInfo
    {
        public GridViewCreateInfo(GridViewType type, string code, string name, bool isShared, bool isSystem,
            string searchExpressionJson, List<GridViewColumn> columns, string orderFieldId, string createdUserAccount)
        {
            this.CreatedUserAccount = createdUserAccount;
            this.Code = code;
            this.Name = name;
            this.Type = type;
            this.IsShared = isShared;
            this.IsSystem = isSystem;
            this.SearchExpression = searchExpressionJson;
            this.Columns = columns;
            this.OrderFieldId = orderFieldId;
        }

        public virtual string CreatedUserAccount { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual GridViewType Type { set; get; }

        public virtual bool IsShared { set; get; }

        public virtual bool IsSystem { set; get; }

        public virtual int Index { set; get; }

        public virtual List<GridViewColumn> Columns { set; get; }

        public virtual string SearchExpression { set; get; }

        public virtual string OrderFieldId { set; get; }

        public List<GridViewFooter> Footer { set; get; }
    }
}
