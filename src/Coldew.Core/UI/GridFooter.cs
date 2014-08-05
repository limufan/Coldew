using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public enum GridFooterValueType
    {
        Fixed,
        Sum
    }

    public class GridFooter
    {
        public string FieldCode { set; get; }

        public string Value { set; get; }

        public GridFooterValueType ValueType { set; get; }
    }
}
