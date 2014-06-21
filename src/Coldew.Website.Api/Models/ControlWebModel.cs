using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public abstract string type { get; }
    }


    [Serializable]
    public class InputWebModel : ControlWebModel
    {
        public InputWebModel(Input input, User opUser)
        {
            dynamic field = input.Field;
            this.field = FieldWebModel.Map(field, opUser);
            this.required = input.Required;
            this.isReadonly = input.IsReadonly;
        }

        public FieldWebModel field;

        public bool required;

        public bool isReadonly;

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
}
