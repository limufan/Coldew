using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Dingdan : JObject
    {
        public Dingdan(JObject jobject)
        {
            foreach (JProperty property in jobject.Properties())
            {
                if (property.Name == "chanpinGrid")
                {
                    this.chanpinGrid = new List<Chanpin>();
                    foreach (JObject chanpinObject in property.Value)
                    {
                        this.chanpinGrid.Add(new Chanpin(chanpinObject));
                    }
                    this.Add("chanpinGrid", new JArray(this.chanpinGrid));
                }
                else if (property.Name == "shoukuanGrid")
                {
                    this.shoukuanGrid = new List<Shoukuan>();
                    foreach (JObject chanpinObject in property.Value)
                    {
                        this.shoukuanGrid.Add(new Shoukuan(chanpinObject));
                    }
                    this.Add("shoukuanGrid", new JArray(this.shoukuanGrid));
                }
                else
                {
                    this.Add(property.Name, property.Value);
                }
            }

            this.fahuoRiqi = DateTime.Parse(this["fahuoRiqi"].ToString());
            this.yingshoukuanJine = this.chanpinGrid.Sum(x => x.zongjine);
            if (this.shoukuanGrid == null)
            {
                this.shoukuanGrid = new List<Shoukuan>();
            }
            this.yishoukuanJine = this.shoukuanGrid.Sum(x => x.shoukuanJine);
            this.weishoukuanJine = this.yingshoukuanJine - this.yishoukuanJine;
            this["shifouShouwan"] = this.weishoukuanJine <= 0 ? "是" : "否";
            string jiekuanFangshi = this["jiekuanFangshi"].ToString();
            if (jiekuanFangshi == "1个月月结")
            {
                DateTime nextMonth = DateTime.Now.AddMonths(1);
                jiekuanRiqi = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            }
            else if (jiekuanFangshi == "2个月月结")
            {
                DateTime nextMonth = DateTime.Now.AddMonths(2);
                jiekuanRiqi = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            }
            else if (jiekuanFangshi == "3个月月结")
            {
                DateTime nextMonth = DateTime.Now.AddMonths(3);
                jiekuanRiqi = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            }
            this.Jisuan();
        }

        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime fahuoRiqi
        {
            private set
            {
                this["fahuoRiqi"] = value;
            }
            get
            {
                return (DateTime)this["fahuoRiqi"];
            }
        }

        /// <summary>
        /// 结款日期
        /// </summary>
        public DateTime jiekuanRiqi
        {
            private set
            {
                this["jiekuanRiqi"] = value;
            }
            get
            {
                return (DateTime)this["jiekuanRiqi"];
            }
        }

        /// <summary>
        /// 应收款
        /// </summary>
        public double yingshoukuanJine
        {
            private set
            {
                this["yingshoukuanJine"] = value;
            }
            get
            {
                if (this["yingshoukuanJine"] != null)
                {
                    return (double)this["yingshoukuanJine"];
                }
                return 0;
            }
        }

        /// <summary>
        /// 已收款
        /// </summary>
        public double yishoukuanJine
        {
            private set
            {
                this["yishoukuanJine"] = value;
            }
            get
            {
                if (this["yishoukuanJine"] != null)
                {
                    return (double)this["yishoukuanJine"];
                }
                return 0;
            }
        }

        /// <summary>
        /// 未收款
        /// </summary>
        public double weishoukuanJine
        {
            private set
            {
                this["weishoukuanJine"] = value;
            }
            get
            {
                if (this["weishoukuanJine"] != null)
                {
                    return (double)this["weishoukuanJine"];
                }
                return 0;
            }
        }

        /// <summary>
        /// 提成
        /// </summary>
        public double ticheng
        {
            private set
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

        /// <summary>
        /// 产品
        /// </summary>
        public List<Chanpin> chanpinGrid { private set; get; }

        /// <summary>
        /// 收款
        /// </summary>
        public List<Shoukuan> shoukuanGrid { private set; get; }

        /// <summary>
        /// 计算订单提成
        /// </summary>
        /// <param name="dingdan"></param>
        /// <returns></returns>
        public void Jisuan()
        {
            double ticheng = 0;
            foreach (Shoukuan shoukuan in this.shoukuanGrid)
            {
                shoukuan.ticheng = this.JisuanTicheng(shoukuan);
                ticheng += shoukuan.ticheng;
            }
            this.ticheng = ticheng;
            this.JisuanChanpinGrid();
        }

        /// <summary>
        /// 计算产品收款，提成
        /// </summary>
        private void JisuanChanpinGrid()
        {
            foreach (Chanpin chanpin in this.chanpinGrid)
            {
                chanpin.shoukuanJine = 0;
                chanpin.ticheng = 0;
                foreach (Shoukuan shoukuan in this.shoukuanGrid)
                {
                    double chanpinShoukuan = this.JisuanChanpinShoukuan(chanpin.zongjine, shoukuan.shoukuanJine);
                    chanpin.shoukuanJine += chanpinShoukuan;
                    chanpin.ticheng += this.JisuanTicheng(chanpin, chanpinShoukuan, shoukuan.shoukuanRiqi);
                }
            }
        }

        /// <summary>
        /// 计算收款提成
        /// </summary>
        /// <param name="chanpinList"></param>
        /// <param name="shoukuan"></param>
        /// <param name="dingdanJine"></param>
        /// <returns></returns>
        private double JisuanTicheng(Shoukuan shoukuan)
        {
            double ticheng = 0;
            foreach (Chanpin chanpin in this.chanpinGrid)
            {
                double chanpinShoukuan = this.JisuanChanpinShoukuan(chanpin.zongjine, shoukuan.shoukuanJine);
                ticheng += this.JisuanTicheng(chanpin, chanpinShoukuan, shoukuan.shoukuanRiqi);
            }
            return ticheng;
        }

        private double JisuanTicheng(Chanpin chanpin, double shoukuanJine, DateTime shoukuanRiqi)
        {
            //基价提成
            double jijiaTicheng = 0;
            if (chanpin.shijiDanjia > chanpin.xiaoshouDanjia)
            {
                jijiaTicheng = shoukuanJine * 0.03;
            }
            //差价提成
            double chajiaTicheng = 0;
            if (chanpin.shijiDanjia > chanpin.xiaoshouDijia)
            {
                chajiaTicheng = (chanpin.shijiDanjia - chanpin.xiaoshouDijia) * 0.2 * shoukuanJine;
            }
            double ticheng = jijiaTicheng + chajiaTicheng;
            //开票提成扣0.08
            if (chanpin.yaokaipiao)
            {
                ticheng = ticheng * 0.92;
            }
            int shoukuanZhouqi = (shoukuanRiqi - this.jiekuanRiqi).Days;
            if (shoukuanZhouqi > 30 && shoukuanZhouqi <= 60)
            {
                //收款延期30到60天提成扣0.5
                ticheng = ticheng * 0.5;
            }
            else if (shoukuanZhouqi > 60)
            {
                //收款延期60天以上提成倒扣0.05
                ticheng = ticheng * 0.05 * -1;
            }
            return ticheng;
        }

        /// <summary>
        /// 计算产品收款金额
        /// </summary>
        /// <param name="chanpin"></param>
        /// <param name="shoukuanJine"></param>
        /// <returns></returns>
        private double JisuanChanpinShoukuan(double chanpinZongjine, double shoukuanJine)
        {
            double chanpinShoukuan = shoukuanJine * (chanpinZongjine / this.yingshoukuanJine);
            return chanpinShoukuan;
        }
    }
}