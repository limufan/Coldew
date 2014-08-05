using System;
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
        public ControlWebModel(Control control, User opUser)
        {
            this.children = Map(control.Children, opUser);
        }

        public static List<ControlWebModel> Map(List<Control> controls, User opUser)
        {
            return controls.Select(c => Map(c, opUser)).ToList();
        }
        public static ControlWebModel Map(Control control, User opUser)
        {
            dynamic d_control = control;
            ControlWebModel model = ControlWebModel.Map(d_control, opUser);
            return model;
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
        public static ControlWebModel Map(GridInput row, User opUser)
        {
            return new GridWebModel(row, opUser);
        }
        public static ControlWebModel Map(ColdewObjectGrid grid, User opUser)
        {
            return new RelatedObjectGridModel(grid, opUser);
        }
        public static ControlWebModel Map(Tab tab, User opUser)
        {
            return new TabModel(tab, opUser);
        }
        public static ControlWebModel Map(TabPane tabPane, User opUser)
        {
            return new TabPaneModel(tabPane, opUser);
        }

        public List<ControlWebModel> children;

        public abstract string type { get; }
    }


    [Serializable]
    public class InputWebModel : ControlWebModel
    {
        public InputWebModel(Input input, User opUser)
            :base(input, opUser)
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
            : base(fieldset, opUser)
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
            : base(row, opUser)
        {
            this.children = ControlWebModel.Map(row.Children, opUser);
        }

        public override string type
        {
            get { return "row"; }
        }
    }

    [Serializable]
    public class GridWebModel : ControlWebModel
    {
        public GridWebModel(GridInput grid, User opUser)
            : base(grid, opUser)
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

    [Serializable]
    public class RelatedObjectGridModel : ControlWebModel
    {
        public RelatedObjectGridModel(ColdewObjectGrid grid, User opUser)
            : base(grid, opUser)
        {
            this.columns = grid.Columns.Select(x => DataGridColumnModel.MapModel(x)).ToList();
            if (grid.Footer != null)
            {
                this.footer = grid.Footer.Select(x => new GridViewFooterModel(x)).ToList();
            }
            this.name = grid.ColdewObject.Code;
        }

        public string name;

        public List<DataGridColumnModel> columns;

        public int width;

        public bool required;

        public bool isReadonly;

        public List<GridViewFooterModel> footer;

        public override string type
        {
            get { return "datagrid"; }
        }
    }

    [Serializable]
    public class TabModel : ControlWebModel
    {
        public TabModel(Tab tab, User opUser)
            : base(tab, opUser)
        {
            
        }

        public override string type
        {
            get { return "tab"; }
        }
    }

    [Serializable]
    public class TabPaneModel : ControlWebModel
    {
        public TabPaneModel(TabPane tabPane, User opUser)
            : base(tabPane, opUser)
        {
            this.title = tabPane.Title;
            this.active = tabPane.Active;
        }

        public string title;

        public bool active;

        public override string type
        {
            get { return "tabPane"; }
        }
    }
}
