using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    [Serializable]
    public class GridViewColumnSetupInfo
    {
        public GridViewColumnSetupInfo(int fieldId)
        {
            this.FieldId = fieldId;
        }

        public GridViewColumnSetupInfo() { }

        public int FieldId { set; get; }
    }
}
