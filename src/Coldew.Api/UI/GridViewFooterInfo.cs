using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public enum GridViewFooterValueType
    {
        Fixed,
        Sum
    }

    public class GridViewFooterInfo
    {
        public string FieldCode { set; get; }

        public string Value { set; get; }

        public GridViewFooterValueType ValueType { set; get; }
    }
}
