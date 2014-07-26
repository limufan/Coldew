using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.UI
{
    public class Tab: Control
    {
        public string Name { set; get; }

        public string Title { set; get; }

        public override void FillJObject(Metadata metadata, Organization.User user, Newtonsoft.Json.Linq.JObject jobject)
        {
            base.FillJObject(metadata, user, jobject);
        }
    }
}
