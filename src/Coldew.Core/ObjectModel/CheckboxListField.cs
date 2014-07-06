using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class CheckboxListField : Field
    {
        internal CheckboxListField()
        {

        }

        public override string Type
        {
            get { return FieldType.CheckboxList; }
        }

        public override string TypeName
        {
            get { return "复选框"; }
        }

        public List<string> DefaultValue { set; get; }

        public List<string> SelectList { set; get; }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            List<string> stringList = new List<string>();
            if (value != null)
            {
                foreach (JToken str in value)
                {
                    stringList.Add(str.ToString());
                }
            }
            return new StringListMetadataValue(stringList, this);
        }

        public void Modify(FieldModifyBaseInfo modifyInfo, List<string> defaultValues, List<string> selectList)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = modifyInfo.Name, Required = modifyInfo.Required };
            this.OnModifying(args);

            this.Name = modifyInfo.Name;
            this.Required = modifyInfo.Required;
            this.DefaultValue = defaultValues;
            this.SelectList = selectList;

            this.OnModifyed(args);
        }
    }
}
