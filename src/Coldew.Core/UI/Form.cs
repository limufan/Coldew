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
    public class Form : Control
    {
        public Form()
        {
            this.Children = new List<Control>();
            this.Relateds = new List<RelatedObject>();
        }

        public string ID { set; get; }

        public string Code { set; get; }

        public string Title {set; get; }

        public List<RelatedObject> Relateds { set; get; }

        public event TEventHandler<Form> Modified;

        public void ClearFieldData(Field field)
        {
            this.Relateds.ForEach(x => x.ClearFieldData(field));

            if (this.Modified != null)
            {
                this.Modified(this);
            }
        }

        public JObject GetJObject(Metadata metadata, User user)
        {
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            foreach (Control control in this.Children)
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
