using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Chanpin : JObject
    {
        public Chanpin(JObject jobject)
        {
            foreach (JProperty property in jobject.Properties())
            {
                this.Add(property.Name, property.Value);
            }
            this.yaokaipiao = (string)this["shifouKaipiao"] == "是";
        }

        public double xiaoshouDijia
        {
            set
            {
                this["xiaoshouDijia"] = value;
            }
            get
            {
                if (this["xiaoshouDijia"] != null)
                {
                    return (double)this["xiaoshouDijia"];
                }
                return 0;
            }
        }
        public double shijiDanjia
        {
            set
            {
                this["shijiDanjia"] = value;
            }
            get
            {
                if (this["shijiDanjia"] != null)
                {
                    return (double)this["shijiDanjia"];
                }
                return 0;
            }
        }
        public double xiaoshouDanjia
        {
            set
            {
                this["xiaoshouDanjia"] = value;
            }
            get
            {
                if (this["xiaoshouDanjia"] != null)
                {
                    return (double)this["xiaoshouDanjia"];
                }
                return 0;
            }
        }
        public double zongjine
        {
            set
            {
                this["zongjine"] = value;
            }
            get
            {
                if (this["zongjine"] != null)
                {
                    return (double)this["zongjine"];
                }
                return 0;
            }
        }
        public bool yaokaipiao;
        public double ticheng
        {
            set
            {
                this["ticheng"] = value;
            }
            get
            {
                if (this["ticheng"] != null)
                {
                    return (double)this["ticheng"];
                }
                return 0;
            }
        }
        public double shoukuanJine
        {
            set
            {
                this["shoukuanJine"] = value;
            }
            get
            {
                if (this["shoukuanJine"] != null)
                {
                    return (double)this["shoukuanJine"];
                }
                return 0;
            }
        }
    }
}
