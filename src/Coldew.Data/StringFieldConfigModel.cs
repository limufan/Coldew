using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class StringFieldConfigModel
    {
        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }
    }
}
