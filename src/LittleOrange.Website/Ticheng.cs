using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LittleOrange.Website
{
    public class Ticheng
    {
        public Ticheng(Dingdan dingdan)
        {
            this.dingdan = dingdan;
        }

        public Dingdan dingdan;

        public double Jisuan()
        {
            double ticheng = 0;
            foreach(Shoukuan shoukuan in this.dingdan.shoukuanList)
            {
                ticheng += this.Jisuan(shoukuan);
            }
            return Math.Round(ticheng, 2);
        }

        public double Jisuan(Shoukuan shoukuan)
        {
            double pingjunShoukuan = shoukuan.shoukuanJine / this.dingdan.chnapinList.Count;
            double ticheng = 0;
            foreach (Chanpin chanpin in this.dingdan.chnapinList)
            {
                ticheng += this.Jisuan(chanpin, pingjunShoukuan, shoukuan.shoukuanRiqi);
            }
            return Math.Round(ticheng, 2);
        }

        public double Jisuan(Chanpin chanpin)
        {
            double ticheng = 0;
            foreach (Shoukuan shoukuan in this.dingdan.shoukuanList)
            {
                double pingjunShoukuan = shoukuan.shoukuanJine / this.dingdan.chnapinList.Count;
                ticheng += this.Jisuan(chanpin, pingjunShoukuan, shoukuan.shoukuanRiqi);
            }
            return Math.Round(ticheng, 2);
        }

        public double Jisuan(Chanpin chanpin, double shoukuanJine, DateTime shoukuanRiqi)
        {
            double jijiaTicheng = 0;
            if (chanpin.shijiDanjia > chanpin.xiaoshouDanjia)
            {
                jijiaTicheng = shoukuanJine * 0.03;
            }
            double chajiaTicheng = 0;
            if (chanpin.shijiDanjia > chanpin.xiaoshouDijia)
            {
                chajiaTicheng = (chanpin.shijiDanjia - chanpin.xiaoshouDijia) * 0.2 * shoukuanJine;
            }
            double ticheng = jijiaTicheng + chajiaTicheng;
            if (chanpin.yaokaipiao)
            {
                ticheng = ticheng * 0.92;
            }
            int shoukuanZhouqi = (shoukuanRiqi - this.dingdan.jiekuanRiqi).Days;
            if (shoukuanZhouqi > 30 && shoukuanZhouqi <= 60)
            {
                ticheng = ticheng * 0.5;
            }
            else if (shoukuanZhouqi > 60)
            {
                ticheng = ticheng * 0.05 * -1;
            }
            return Math.Round(ticheng, 2);
        }
    }
}