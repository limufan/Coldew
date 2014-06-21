using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;


namespace Coldew.Core.UI
{
    public class Input : Control
    {
        public Input(Field field)
        {
            this.Field = field;
            this.Required = field.Required;
            this.Width = 6;
        }

        public Field Field { set; get; }

        public int Width { set; get; }

        public bool Required { set; get; }

        public bool IsReadonly { set; get; }
    }
}
