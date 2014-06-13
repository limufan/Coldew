using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class MetadataGridModel
    {
        public int totalCount;
        public string gridJson;
        public string footersJson;
    }
}
