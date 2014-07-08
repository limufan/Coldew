﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.UI;

namespace Coldew.Website.Api.Models
{
    [Serializable]
    public abstract class ControlWebModel
    {
        public static List<ControlWebModel> Map(List<Control> controls, User opUser)
        {
            List<ControlWebModel> models = controls.Select(x =>
            {
                dynamic control = x;
                ControlWebModel model = ControlWebModel.Map(control, opUser);
                return model;
            }).ToList();
            return models;
        }
        public static ControlWebModel Map(Fieldset fieldset, User opUser)
        {
            return new FieldsetWebModel(fieldset, opUser);
        }
        public static ControlWebModel Map(Input input, User opUser)
        {
            return new InputWebModel(input, opUser);
        }
        public static ControlWebModel Map(Row row, User opUser)
        {
            return new RowWebModel(row, opUser);
        }
        public static ControlWebModel Map(Grid row, User opUser)
        {
            return new GridWebModel(row, opUser);
        }

        public abstract string type { get; }
    }


    [Serializable]
    public class InputWebModel : ControlWebModel
    {
        public InputWebModel(Input input, User opUser)
        {
            this.field = FieldWebModel.Map(input.Field, opUser);
            this.required = input.Required;
            this.isReadonly = input.IsReadonly;
            this.width = input.Width;
        }

        public FieldWebModel field;

        public bool required;

        public bool isReadonly;

        public int width;

        public override string type
        {
            get { return "input"; }
        }
    }

    [Serializable]
    public class FieldsetWebModel : ControlWebModel
    {
        public FieldsetWebModel(Fieldset fieldset, User opUser)
        {
            this.title = fieldset.Title;
        }

        public string title;

        public override string type
        {
            get { return "fieldset"; }
        }
    }

    [Serializable]
    public class RowWebModel : ControlWebModel
    {
        public RowWebModel(Row row, User opUser)
        {
            this.children = ControlWebModel.Map(row.Children, opUser);
        }

        public List<ControlWebModel> children;

        public override string type
        {
            get { return "row"; }
        }
    }

    [Serializable]
    public class GridWebModel : ControlWebModel
    {
        public GridWebModel(Grid grid, User opUser)
        {
            this.columns = grid.Columns.Select(x => DataGridColumnModel.MapModel(x)).ToList();
            this.width = grid.Width;
            this.required = grid.Required;
            this.isReadonly = grid.IsReadonly;
            if (grid.AddForm != null)
            {
                this.addForm = new FormWebModel(grid.AddForm, opUser);
            }
            if (grid.EditForm != null)
            {
                this.editForm = new FormWebModel(grid.EditForm, opUser);
            }
            this.field = new FieldWebModel(grid.Field, opUser);
            this.editable = grid.Editable;
            if (grid.Footer != null)
            {
                this.footer = grid.Footer.Select(x => new GridViewFooterModel(x)).ToList();
            }
        }

        public FieldWebModel field;

        public FormWebModel addForm { set; get; }

        public FormWebModel editForm { set; get; }

        public List<DataGridColumnModel> columns { set; get; }

        public int width { set; get; }

        public bool required { set; get; }

        public bool isReadonly { set; get; }

        public bool editable { set; get; }

        public List<GridViewFooterModel> footer { set; get; }

        public override string type
        {
            get { return "grid"; }
        }
    }
}
