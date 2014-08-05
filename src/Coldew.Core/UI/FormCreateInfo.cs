using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class FormCreateInfo
    {
        public string Code {set;get;}

        public string Title { set; get; }

        public List<Control> Controls { set; get; }
    }
}
