using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json.Linq;

namespace Coldew.Core.UI
{
    public abstract class Control
    {
        public List<Control> Children { set; get; }

        public virtual void FillJObject(Metadata metadata, User user, JObject jobject)
        {

        }
    }
}
