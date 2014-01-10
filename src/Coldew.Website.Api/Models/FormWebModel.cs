using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FormWebModel
    {
        public FormWebModel(Form form)
        {
            this.id = form.ID;
            this.code = form.Code;
            this.title = form.Title;
            this.sections = form.Sections.Select(x => new SectionWebModel(x)).ToList();
        }

        public string id;

        public string code;

        public string title;

        public List<SectionWebModel> sections;
    }
}
