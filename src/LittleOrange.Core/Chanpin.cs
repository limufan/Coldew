using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Chanpin
    {
        public Chanpin(JObject chanpin)
        {
            this.xiaoshouDijia = (double)chanpin["xiaoshouDijia"];
            this.shijiDanjia = (double)chanpin["shijiDanjia"];
            this.xiaoshouDanjia = (double)chanpin["xiaoshouDanjia"];
            this.zongjine = (double)chanpin["zongjine"];
            this.yaokaipiao = (string)chanpin["shifouKaipiao"] == "是";
        }

        public double xiaoshouDijia;
        public double shijiDanjia;
        public double xiaoshouDanjia;
        public double zongjine;
        public bool yaokaipiao;
    }
}
