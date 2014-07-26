using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FormWebModel
    {
        public FormWebModel(Form form, User opUser)
        {
            this.id = form.ID;
            this.code = form.Code;
            this.title = form.Title;
            this.controls = ControlWebModel.Map(form.Children, opUser);
        }

        public string id;

        public string code;

        public string title;

        public List<ControlWebModel> controls;
    }
}
