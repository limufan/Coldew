using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class StringField : Field
    {
        internal StringField()
        {

        }

        public override string TypeName
        {
            get { return "短文本"; }
        }

        public string DefaultValue { set; get; }

        public List<string> Suggestions { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new StringMetadataValue(value.ToString(), this);
        }

        public void Modify(FieldModifyBaseInfo modifyInfo, string defaultValue)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = modifyInfo.Name, Required = modifyInfo.Required };

            this.OnModifying(args);

            FieldModel model = NHibernateHelper.CurrentSession.Get<FieldModel>(this.ID);
            model.name = modifyInfo.Name;
            model.required = modifyInfo.Required;
            model.Config = defaultValue;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();

            this.Name = modifyInfo.Name;
            this.Required = modifyInfo.Required;
            this.DefaultValue = defaultValue;

            this.OnModifyed(args);
        }
    }
}
