using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class DropdownListField : ListField
    {
        internal DropdownListField()
        {
        }


        public override string Type
        {
            get { return FieldType.DropdownList; }
        }

        public override string TypeName
        {
            get { return "下拉选项"; }
        }
    }
}
