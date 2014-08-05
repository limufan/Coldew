﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public class MetadtaGridPageModel
    {
        public string nameField;

        public ObjectPermissionValue permission;

        public List<DataGridColumnModel> columns;

        public string title;

        public List<FieldWebModel> fields;

        public List<LeftMenuModel> menus;

        public string objectId;

        public string viewId;
    }

    [Serializable]
    public class LeftMenuModel
    {
        public LeftMenuModel(GridView view)
        {
            this.name = view.Name;
            this.viewId = view.ID;
        }

        public string name;
        public string viewId;
    }

    [Serializable]
    public class DataGridColumnModel
    {
        public string title;
        public int width;
        public string field;
        public string name;

        public static DataGridColumnModel MapModel(GridColumn column)
        {
            return new DataGridColumnModel { field = column.Field.Code, name = column.Field.Code, title = column.Field.Name, width = column.Width };
        }
    }
}
