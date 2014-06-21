using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class Fieldset : Control
    {
        public Fieldset(string title)
        {
            this.Title = title;
        }

        public string Title { set; get; }
    }
}
