using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class CheckboxListFieldCreateInfo : FieldCreateInfo
    {
        public CheckboxListFieldCreateInfo(string code, string name, List<string> selectList)
            :base(code, name)
        {
            this.SelectList = selectList;
        }

        public List<string> DefaultValues { set; get; }

        public List<string> SelectList { set; get; }
    }
}
