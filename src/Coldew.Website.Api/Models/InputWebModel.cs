using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class InputWebModel
    {
        public InputWebModel(Input input, User opUser)
        {
            dynamic field = input.Field;
            this.field = FieldWebModel.Map(field, opUser);
            this.required = input.Required;
            this.isReadonly = input.IsReadonly;
        }

        public FieldWebModel field;

        public bool required;

        public bool isReadonly;
    }
}
