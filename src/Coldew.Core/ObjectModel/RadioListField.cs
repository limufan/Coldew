using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class RadioListField : ListField
    {
        internal RadioListField()
        {

        }

        public override string Type
        {
            get { return FieldType.RadioList; }
        }

        public override string TypeName
        {
            get { return "单选框"; }
        }
    }
}
