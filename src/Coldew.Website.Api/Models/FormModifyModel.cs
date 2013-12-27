using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class FormModifyModel
    {
        public string userAccount;

        public string objectId;

        public string code;

        public List<SectionModifyModel> sections;
    }

    [Serializable]
    public class SectionModifyModel
    {
        public string name;

        public int columnCount;

        public List<InputModifyModel> inputs;
    }

    [Serializable]
    public class InputModifyModel
    {
        public string fieldCode;
    }
}
