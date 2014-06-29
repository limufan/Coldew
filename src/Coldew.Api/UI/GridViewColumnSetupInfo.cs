using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    [Serializable]
    public class GridViewColumnSetupInfo
    {
        public GridViewColumnSetupInfo(string fieldId)
        {
            this.FieldId = fieldId;
        }

        public GridViewColumnSetupInfo() { }

        public string FieldId { set; get; }
    }
}
