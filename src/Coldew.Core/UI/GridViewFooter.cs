using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public enum GridViewFooterValueType
    {
        Fixed,
        Sum
    }

    public class GridViewFooter
    {
        public string FieldCode { set; get; }

        public string Value { set; get; }

        public GridViewFooterValueType ValueType { set; get; }
    }
}
