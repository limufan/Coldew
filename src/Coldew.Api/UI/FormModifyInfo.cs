using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api.UI
{
    [Serializable]
    public class FormModifyInfo
    {
        public string UserAccount{set;get;}

        public string ObjectId { set; get; } 

        public string Code { set; get; }

        public List<SectionModifyInfo> Sections { set; get; }
    }

    [Serializable]
    public class SectionModifyInfo
    {
        public string Name { set; get; }

        public int ColumnCount { set; get; }

        public List<string> Fields { set; get; }
    }
}
