using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class DropdownListField : ListField
    {
        public DropdownListField(DropdownFieldNewInfo newInfo)
            :base(newInfo)
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
