using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Website
{
    public class Dingdan
    {
        public Dingdan(JObject dingdan)
        {
            this.chnapinList = new List<Chanpin>();
            JArray chanpinArray = (JArray)dingdan["chanpinGrid"];
            foreach (JObject chanpin in chanpinArray)
            {
                this.chnapinList.Add(new Chanpin(chanpin));
            }
            this.jiekuanRiqi = (DateTime)dingdan["jiekuanRiqi"];
            this.yingshoukuanJine = (double)dingdan["yingshoukuanJine"];
            this.shoukuanList = new List<Shoukuan>();
            if (dingdan["shoukuanGrid"] != null)
            {
                foreach (JObject shoukuan in dingdan["shoukuanGrid"])
                {
                    this.shoukuanList.Add(new Shoukuan(shoukuan));
                }
            }
        }

        /// <summary>
        /// 结款日期
        /// </summary>
        public DateTime jiekuanRiqi;

        /// <summary>
        /// 应收款
        /// </summary>
        public double yingshoukuanJine;

        /// <summary>
        /// 已收款
        /// </summary>
        public double yishoukuanJine;

        /// <summary>
        /// 产品
        /// </summary>
        public List<Chanpin> chnapinList;

        /// <summary>
        /// 收款
        /// </summary>
        public List<Shoukuan> shoukuanList;
    }
}