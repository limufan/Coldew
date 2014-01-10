using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;


namespace Coldew.Core.UI
{
    public class Input
    {
        public Input(Field field)
        {
            this.Field = field;
            this.Required = field.Required;
        }

        public Input(Field field, bool required)
        {
            this.Field = field;
            this.Required = required;
        }

        public Field Field { private set; get; }

        public bool Required { private set; get; }
    }
}
