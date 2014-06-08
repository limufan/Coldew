using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LittleOrange.Website
{
    public class TichengJisuanqi
    {
        public static double Jisuan(double xiaoshouDijia, double jine, double shijiDanjia, double xiaoshouDanjia, bool kaipiao)
        {
            double jijiaTicheng = 0;
            if (shijiDanjia > xiaoshouDanjia)
            {
                jijiaTicheng = jine * 0.03;
            }
            double chajiaTicheng = 0;
            if (shijiDanjia > xiaoshouDijia)
            {
                chajiaTicheng = (shijiDanjia - xiaoshouDijia) * 0.2 * jine;
            }
            double ticheng = jijiaTicheng + chajiaTicheng;
            if (kaipiao)
            {
                ticheng = ticheng * 0.92;
            }
            return ticheng;
        }

        public static double Jisuan(double xiaoshouDijia, double jine, double shijiDanjia, double xiaoshouDanjia, bool kaipiao, DateTime jiekuanRiqi, DateTime shoukuanRiqi)
        {
            double ticheng = Jisuan(xiaoshouDijia, jine, shijiDanjia, xiaoshouDanjia, kaipiao);
            int shoukuanZhouqi = (shoukuanRiqi - jiekuanRiqi).Days;
            if (shoukuanZhouqi <= 30)
            {
                return ticheng;
            }
            else if (shoukuanZhouqi <= 60)
            {
                return ticheng * 0.5;
            }
            else
            {
                return ticheng * 0.05 * -1;
            }
        }
    }
}