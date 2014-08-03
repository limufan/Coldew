using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class Tab: Control
    {
        
    }

    public class TabPane : Control
    {
        public string Title { set; get; }

        public bool Active { set; get; }
    }
}
