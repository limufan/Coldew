using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.Organization;

namespace Coldew.Core.UI
{
    public class Input
    {
        public Input(Field field)
        {
            this.Field = field;
        }

        public Field Field { private set; get; }

        public InputInfo Map(User user)
        {
            return new InputInfo
            {
                Field = this.Field.Map(user)
            };
        }
    }
}
