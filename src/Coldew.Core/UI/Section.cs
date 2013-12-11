using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.UI
{
    public class Section
    {
        public Section(string title, int columnCount, List<Field> fields)
        {
            this.Title = title;
            this.ColumnCount = columnCount;
            this.Fields = fields;
        }

        public string Title { private set; get; }

        public int ColumnCount { private set; get; }

        public List<Field> Fields { private set; get; }

        public void ClearFieldData(Field field)
        {
            this.Fields.Remove(field);
        }

        public SectionInfo Map(User user)
        {
            return new SectionInfo
            {
                ColumnCount = this.ColumnCount,
                Fields = this.Fields.Select(x => x.Map(user)).ToList(),
                Title = this.Title
            };
        }

        internal SectionModel MapModel()
        {
            SectionModel model = new SectionModel();
            model.ColumnCount = this.ColumnCount;
            model.Fields = this.Fields.Select(x => x.Code).ToList();
            model.Title = this.Title;
            return model;
        }
    }
}
