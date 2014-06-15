using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class Dingdan : JObject
    {
        public Dingdan(JObject dingdan)
        {
            this.chanpinList = new List<Chanpin>();
            JArray chanpinArray = (JArray)dingdan["chanpinGrid"];
            foreach (JObject chanpin in chanpinArray)
            {
                this.chanpinList.Add(new Chanpin(chanpin));
            }
            this.jiekuanRiqi = (DateTime)dingdan["jiekuanRiqi"];
            this.fahuoRiqi = DateTime.Parse(dingdan["fahuoRiqi"].ToString());
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

        public void Jisuan()
        {
            this.yishoukuanJine = this.shoukuanList.Sum(x => x.shoukuanJine);
            this.weishoukuanJine = this.yingshoukuanJine - this.yishoukuanJine;
        }

        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime fahuoRiqi { private set; get; }

        /// <summary>
        /// 结款日期
        /// </summary>
        public DateTime jiekuanRiqi { private set; get; }

        /// <summary>
        /// 应收款
        /// </summary>
        public double yingshoukuanJine { private set; get; }

        /// <summary>
        /// 已收款
        /// </summary>
        public double yishoukuanJine { private set; get; }

        /// <summary>
        /// 未收款
        /// </summary>
        public double weishoukuanJine { private set; get; }

        /// <summary>
        /// 产品
        /// </summary>
        public List<Chanpin> chanpinList { private set; get; }

        /// <summary>
        /// 收款
        /// </summary>
        public List<Shoukuan> shoukuanList { private set; get; }

        /// <summary>
        /// 计算订单提成
        /// </summary>
        /// <param name="dingdan"></param>
        /// <returns></returns>
        public double JisuanTicheng()
        {
            double ticheng = 0;
            foreach (Shoukuan shoukuan in this.shoukuanList)
            {
                ticheng += this.JisuanTicheng(shoukuan);
            }
            return ticheng;
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
            foreach (Chanpin chanpin in this.chanpinList)
            {
                double chanpinShoukuan = this.JisuanChanpinShoukuan(chanpin.zongjine, shoukuan.shoukuanJine);
                ticheng += this.JisuanTicheng(chanpin, chanpinShoukuan, shoukuan.shoukuanRiqi);
            }
            return ticheng;
        }

        /// <summary>
        /// 计算产品提成
        /// </summary>
        /// <param name="chanpin"></param>
        /// <param name="shoukuanList"></param>
        /// <returns></returns>
        public double JisuanTicheng(Chanpin chanpin)
        {
            double ticheng = 0;
            foreach (Shoukuan shoukuan in this.shoukuanList)
            {
                double chanpinShoukuan = this.JisuanChanpinShoukuan(chanpin.zongjine, shoukuan.shoukuanJine);
                ticheng += this.JisuanTicheng(chanpin, chanpinShoukuan, shoukuan.shoukuanRiqi);
            }
            return ticheng;
        }

        public double JisuanTicheng(Chanpin chanpin, double shoukuanJine, DateTime shoukuanRiqi)
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
        public double JisuanChanpinShoukuan(double chanpinZongjine, double shoukuanJine)
        {
            double chanpinShoukuan = shoukuanJine * (chanpinZongjine / this.yingshoukuanJine);
            return chanpinShoukuan;
        }
    }
}