﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api;

namespace Coldew.Website.Models
{
    public class ListFieldEditModel
    {
        public ListFieldEditModel(ListFieldInfo field)
        {
            this.id = field.ID;
            this.name = field.Name;
            this.required = field.Required;
            this.defaultValue = field.DefaultValue;
            
            this.selectList = string.Join(",", field.SelectList);
        }

        public ListFieldEditModel(CheckboxFieldInfo field)
        {
            this.id = field.ID;
            this.name = field.Name;
            this.required = field.Required;
            this.defaultValue = string.Join(",", field.DefaultValues);
            
            this.selectList = string.Join(",", field.SelectList);
        }

        public int id;

        public string name;

        public bool required;

        public string selectList;

        public string defaultValue;

        public int index;
    }
}