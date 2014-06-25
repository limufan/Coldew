using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public enum DingdanZhuangtai
    {
        shenhe,
        wancheng,
        none
    }

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

            if (this.shoukuanGrid == null)
            {
                this.shoukuanGrid = new List<Shoukuan>();
            }
            
        }

        /// <summary>
        /// 发货单号
        /// </summary>
        public string FahuoDanhao
        {
            get
            {
                return this["fahuoDanhao"].ToString();
            }
        }

        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime FahuoRiqi
        {
            private set
            {
                this["fahuoRiqi"] = value;
            }
            get
            {
                if (this["fahuoRiqi"].Type == JTokenType.Date)
                {
                    return (DateTime)this["fahuoRiqi"];
                }
                else
                {
                    return DateTime.Parse(this["fahuoRiqi"].ToString()); ;
                }
            }
        }

        /// <summary>
        /// 结款日期
        /// </summary>
        public DateTime JiekuanRiqi
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
        public double YingshoukuanJine
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
        public double YishoukuanJine
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
        public double WeishoukuanJine
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
        public double Ticheng
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

        public DingdanZhuangtai Zhuangtai
        {
            set
            {
                if (value == DingdanZhuangtai.wancheng)
                {
                    this["zhuangtai"] = "完成";
                }
                else if(value == DingdanZhuangtai.shenhe)
                {
                    this["zhuangtai"] = "审核";
                }
                else if (value == DingdanZhuangtai.none)
                {
                    this["zhuangtai"] = "";
                }
            }
            get 
            {
                if (this["zhuangtai"] == null)
                {
                    return DingdanZhuangtai.none;
                }
                else if (this["zhuangtai"].ToString() == "完成")
                {
                    return DingdanZhuangtai.wancheng;
                }
                else if (this["zhuangtai"].ToString() == "审核")
                {
                    return DingdanZhuangtai.shenhe;
                }
                return DingdanZhuangtai.none;
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
        /// 计算月结日期
        /// </summary>
        public void JisuanJiekuanRiqi()
        {
            if (this["jiekuanFangshi"] != null)
            {
                string jiekuanFangshi = this["jiekuanFangshi"].ToString();
                DateTime jiekuanRiqi = DateTime.Now;
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
                this.JiekuanRiqi = jiekuanRiqi;
            }
        }

        /// <summary>
        /// 计算收款信息
        /// </summary>
        public void JisuanShoukuanXinxi()
        {
            this.YingshoukuanJine = this.chanpinGrid.Sum(x => x.zongjine);
            this.YishoukuanJine = this.shoukuanGrid.Sum(x => x.shoukuanJine);
            this.WeishoukuanJine = this.YingshoukuanJine - this.YishoukuanJine;
            this["shifouShouwan"] = this.WeishoukuanJine <= 0 ? "是" : "否";
        }

        /// <summary>
        /// 计算订单提成,收款提成
        /// </summary>
        /// <param name="dingdan"></param>
        /// <returns></returns>
        public void Jisuan()
        {
            this.JisuanJiekuanRiqi();
            this.JisuanShoukuanXinxi();
            this.JisuanTicheng();
        }

        /// <summary>
        /// 计算订单提成,收款提成
        /// </summary>
        /// <param name="dingdan"></param>
        /// <returns></returns>
        public void JisuanTicheng()
        {
            foreach (Shoukuan shoukuan in this.shoukuanGrid)
            {
                shoukuan.ticheng = Math.Round(this.JisuanTicheng(shoukuan), 2);
            }
            this.Ticheng = this.shoukuanGrid.Sum(x => x.ticheng);
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
                chanpin.shoukuanJine = Math.Round(chanpin.shoukuanJine, 2);
                chanpin.ticheng = Math.Round(chanpin.ticheng, 2);
            }
        }

        /// <summary>
        /// 计算收款提成
        /// </summary>
        /// <param name="chanpinList"></param>
        /// <param name="shoukuan"></param>
        /// <param name="dingdanJine"></param>
        /// <returns></returns>
        public double JisuanTicheng(Shoukuan shoukuan)
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
            int shoukuanZhouqi = (shoukuanRiqi - this.JiekuanRiqi).Days;
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
            double chanpinShoukuan = shoukuanJine * (chanpinZongjine / this.YingshoukuanJine);
            return Math.Round(chanpinShoukuan, 2);
        }
    }
}