using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class GridField : Field
    {
        public GridField(FieldNewInfo info, List<Field> fields)
            :base(info)
        {
            this.Fields = fields;   
        }

        public List<Field> Fields { private set; get; }

        public override string TypeName
        {
            get { return "Grid"; }
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new JsonMetadataValue(value, this);
        }

        public void Modify(string name, bool required)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = name, Required = required};

            this.OnModifying(args);

            FieldModel model = NHibernateHelper.CurrentSession.Get<FieldModel>(this.ID);
            model.Name = name;
            model.Required = required;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();

            this.Name = name;
            this.Required = required;

            this.OnModifyed(args);
        }
    }
}
