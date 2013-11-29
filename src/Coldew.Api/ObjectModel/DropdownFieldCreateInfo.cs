using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class DropdownFieldCreateInfo : FieldCreateInfo
    {
        public DropdownFieldCreateInfo(string code, string name, List<string> selectList)
            :base(code, name)
        {
            this.SelectList = selectList;
        }

        public string DefaultValue { set; get; }

        public List<string> SelectList { set; get; }
    }
}
