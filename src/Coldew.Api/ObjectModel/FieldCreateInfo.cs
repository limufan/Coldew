using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class FieldCreateInfo
    {
        public FieldCreateInfo(string code, string name)
        {
            this.Code = code;
            this.Name = name;
            this.GridWidth = 80;
        }

        public FieldCreateInfo(string code, string name, string tip, bool required, bool isSystem)
        {
            this.Code = code;
            this.Name = name;
            this.Tip = tip;
            this.Required = required;
            this.IsSystem = isSystem;
            this.GridWidth = 80;
        }

        public string Code { set; get; }

        public string Name { set; get; }

        public string Tip { set; get; }

        public bool Required { set; get; }

        public bool Unique { set; get; }

        public bool IsSystem { set; get; }

        public bool IsSummary { set; get; }

        public int GridWidth { set; get; }
    }
}
