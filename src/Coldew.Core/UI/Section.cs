﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.Organization;
using Coldew.Data;
using Coldew.Website.Api.Models;
using Newtonsoft.Json;

namespace Coldew.Core.UI
{
    public class Section
    {
        public Section(string title, int columnCount, List<Input> inputs)
        {
            this.Title = title;
            this.ColumnCount = columnCount;
            this.Inputs = inputs;
        }

        public string Title { private set; get; }

        public int ColumnCount { private set; get; }

        public List<Input> Inputs { private set; get; }

        public void ClearFieldData(Field field)
        {
            Input input = this.Inputs.Find(x => x.Field == field);
            if (input != null)
            {
                this.Inputs.Remove(input);
            }
        }

        public SectionInfo Map(User user)
        {
            return new SectionInfo
            {
                ColumnCount = this.ColumnCount,
                Title = this.Title
            };
        }

        public SectionWebModel MapWebModel(User user)
        {
            return new SectionWebModel
            {
                columnCount = this.ColumnCount,
                inputs = this.Inputs.Select(x => x.Map(user)).ToList(),
                name = this.Title
            };
        }

        internal SectionModel MapModel()
        {
            SectionModel model = new SectionModel();
            model.ColumnCount = this.ColumnCount;
            model.Inputs = this.Inputs.Select(x => new InputModel { fieldCode = x.Field.Code }).ToList();
            model.Title = this.Title;
            return model;
        }
    }
}
