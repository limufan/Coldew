using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Website.Api.Models;

namespace Coldew.Core.UI
{
    public class Input
    {
        public Input(Field field)
        {
            this.Field = field;
        }

        public Field Field { private set; get; }

        public InputWebModel Map(User user)
        {
            return new InputWebModel
            {
                field = this.Field.MapWebModel(user)
            };
        }


    }
}
