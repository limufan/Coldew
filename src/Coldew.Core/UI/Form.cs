using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coldew.Core.UI
{
    public class Form
    {
        FormDataProvider _dataProvider;
        public Form(string id, string code, string title, List<Control> controls, List<RelatedObject> relateds, FormManager formManager)
        {
            this.ID = id;
            this.Code = code;
            this.Title = title;
            this.Controls = controls;
            this.Relateds = relateds;
            if (this.Relateds == null)
            {
                this.Relateds = new List<RelatedObject>();
            }
            this._dataProvider = formManager.DataProvider;
        }

        public string ID { private set; get; }

        public string Code { private set; get; }

        public string Title {private set; get; }

        public List<Control> Controls {private set; get; }

        public List<RelatedObject> Relateds { private set; get; }

        public void ClearFieldData(Field field)
        {
            this.Relateds.ForEach(x => x.ClearFieldData(field));

            this._dataProvider.Update(this);
        }

        public JObject GetJObject(Metadata metadata, User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            foreach (Control control in this.Controls)
            {
                this.FillJObject(control, metadata, user, jobject);
            }
            return jobject;
        }

        private void FillJObject(Control control, Metadata metadata, User user, JObject jobject)
        {
            if(control.Children != null)
            {
                foreach (Control child in control.Children)
                {
                    this.FillJObject(child, metadata, user, jobject);
                }
            }
            control.FillJObject(metadata, user, jobject);
        }
    }
}
