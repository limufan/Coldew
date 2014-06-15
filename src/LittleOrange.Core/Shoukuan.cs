using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Shoukuan
    {
        public Shoukuan()
        {

        }

        public Shoukuan(JObject shoukuan)
        {
            this.shoukuanRiqi = (DateTime)shoukuan["shoukuanRiqi"];
            this.shoukuanJine = (double)shoukuan["shoukuanJine"];
        }

        public DateTime shoukuanRiqi;
        public double shoukuanJine;
    }
}