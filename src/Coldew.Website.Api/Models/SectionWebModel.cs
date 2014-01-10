using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class SectionWebModel
    {
        public SectionWebModel(Section section)
        {
            this.name = section.Title;
            this.columnCount = section.ColumnCount;
            this.inputs = section.Inputs.Select(x => new InputWebModel(x)).ToList();
        }

        public string name ;

        public int columnCount ;

        public List<InputWebModel> inputs;
    }
}
