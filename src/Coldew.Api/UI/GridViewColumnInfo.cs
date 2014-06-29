using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    [Serializable]
    public class GridViewColumnInfo
    {
        public string FieldId { set; get; }

        public string Name {  set; get; }

        public string Code {  set; get; }

        public int Width { set; get; }
    }
}
