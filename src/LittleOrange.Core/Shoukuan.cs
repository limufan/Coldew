using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Shoukuan : JObject
    {
        Dingdan _dingdan;

        public Shoukuan(JObject jobject)
        {
            foreach (JProperty property in jobject.Properties())
            {
                this.Add(property.Name, property.Value);
            }
        }

        public DateTime shoukuanRiqi
        {
            set
            {
                this["shoukuanRiqi"] = value;
            }
            get
            {
                return (DateTime)this["shoukuanRiqi"];
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
    }
}