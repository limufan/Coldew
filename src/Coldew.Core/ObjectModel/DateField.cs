using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;
using Coldew.Website.Api.Models;

namespace Coldew.Core
{
    public class DateField : Field
    {
        public DateField(FieldNewInfo info, bool defaultValueIsToday)
            :base(info)
        {
            this.DefaultValueIsToday = defaultValueIsToday;
        }

        public override string TypeName
        {
            get { return "日期"; }
        }

        public bool DefaultValueIsToday { set; get; }

        public void Modify(FieldModifyBaseInfo modifyInfo, bool defaultValueIsToday)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = modifyInfo.Name, Required = modifyInfo.Required };

            this.OnModifying(args);

            FieldModel model = NHibernateHelper.CurrentSession.Get<FieldModel>(this.ID);
            model.Name = modifyInfo.Name;
            model.Required = modifyInfo.Required;
            DateFieldConfigModel configModel = new DateFieldConfigModel { DefaultValueIsToday = defaultValueIsToday };
            model.Config = JsonConvert.SerializeObject(configModel);

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();

            this.Name = modifyInfo.Name;
            this.Required = modifyInfo.Required;
            this.DefaultValueIsToday = defaultValueIsToday;

            this.OnModifyed(args);
        }

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new DateMetadataValue(null, this);
            }
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return new DateMetadataValue(date, this);
            }
            else
            {
                throw new ColdewException("创建DateMetadataValue出错,value:" + value);
            }
        }

        public override FieldInfo Map(User user)
        {
            DateFieldInfo info = new DateFieldInfo();
            info.DefaultValueIsToday = this.DefaultValueIsToday;
            this.Fill(info, user);
            return info;
        }

        public override FieldWebModel MapWebModel(User user)
        {
            DateFieldWebModel info = new DateFieldWebModel();
            info.defaultValueIsToday = this.DefaultValueIsToday;
            this.Fill(info, user);
            return info;
        }
    }
}
