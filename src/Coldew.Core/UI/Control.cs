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
        public Control()
        {
            this.Children = new List<Control>();
        }

        public List<Control> Children { set; get; }

        public List<T> GetControls<T>() where T : Control
        {
            List<T> childrenControls = this.Children.SelectMany(c => c.GetControls<T>()).ToList();
            List<T> children = this.Children
                .Where(c => c is T)
                .Select(c => c as T)
                .ToList();
            return children.Concat(childrenControls).ToList();
        }

        public bool Contains(Control control)
        {
            if (this.Children.Contains(control))
            {
                return true;
            }
            if (this.Children.Any(c => c.Contains(control)))
            {
                return true;
            }
            return false;
        }

        public virtual void FillJObject(Metadata metadata, User user, JObject jobject)
        {

        }
    }
}
