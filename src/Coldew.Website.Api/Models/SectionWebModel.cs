using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class SectionWebModel
    {
        public string name ;

        public int columnCount ;

        public List<InputWebModel> inputs;
    }
}
