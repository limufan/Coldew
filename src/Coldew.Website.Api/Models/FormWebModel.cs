using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FormWebModel
    {
        public string id;

        public string code;

        public string title;

        public List<SectionWebModel> sections;
    }
}
