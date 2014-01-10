using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class InputWebModel
    {
        public InputWebModel(Input input)
        {
            dynamic field = input.Field;
            this.field = FieldWebModel.Map(field, null);
            this.required = input.Required;
        }

        public FieldWebModel field;

        public bool required;
    }
}
