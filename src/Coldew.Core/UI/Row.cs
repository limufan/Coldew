using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class Row: Control
    {
        public Row()
        {
            this.Children = new List<Control>();
        }

        public List<Control> Children { set; get; }
    }
}
