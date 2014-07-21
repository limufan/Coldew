using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api.Workflow;
using Coldew.Api.Organization;
using Coldew.Api;
using Coldew.Core.Workflow;

namespace Coldew.Website.Api.Models
{
    public class LiuchengInfoModel
    {
        public LiuchengInfoModel(Renwu renwu)
        {
            this.buzhou = renwu.Xingdong.Name;
            this.chuliren = renwu.Chuliren.Name;
            this.zhuangtai = EnumMapper.Map(renwu.Zhuangtai);
            this.kaishiShijian = renwu.Xingdong.KaishiShijian.ToString();
            if (renwu.ChuliShijian.HasValue)
            {
                this.wanchengShijian = renwu.ChuliShijian.ToString();
            }
            this.wanchengShuoming = renwu.ChuliShuoming;
        }
        public string buzhou;
        public string chuliren;
        public string zhuangtai;
        public string kaishiShijian;
        public string wanchengShijian;
        public string wanchengShuoming;
    }

    public class RenwuModel
    {
        public RenwuModel(Renwu renwu)
        {
            this.bianhao = renwu.Xingdong.Code;
            this.id = renwu.Id;
            this.kaishiShijian = renwu.Xingdong.KaishiShijian.ToString();
            this.mingcheng = renwu.Xingdong.Name;
            if (!renwu.Xingdong.QiwangWanchengShijian.HasValue)
            {
                this.qiwangWanchengShijian = renwu.Xingdong.QiwangWanchengShijian.ToString();
            }
            this.liuchengId = renwu.Xingdong.liucheng.Id;
            this.liuchengGuid = renwu.Xingdong.liucheng.Id;
            this.liuchengMingcheng = renwu.Xingdong.liucheng.Mingcheng;
            if (renwu.ChuliShijian.HasValue)
            {
                this.wanchengShijian = renwu.ChuliShijian.ToString();
            }
            this.zhaiyao = renwu.Xingdong.Zhaiyao;

            this.zhuangtaiMingcheng = EnumMapper.Map(renwu.Zhuangtai);
            this.zhuangtai = renwu.Zhuangtai;
            this.url = string.Format("{0}?renwuId={1}&liuchengId={2}", renwu.Xingdong.liucheng.Moban.TransferUrl, renwu.Id, renwu.Xingdong.liucheng.Id);
            this.faqiren = renwu.Xingdong.liucheng.Faqiren.Name;
            this.chuliren = renwu.Chuliren.Name;
            this.wanchengShuoming = renwu.ChuliShuoming;
            this.icons = "";
            if (renwu.Xingdong.Jinjide)
            {
                this.icons += "<span title='紧急的任务' class='icon-jinji-renwu'></span>";
            }
            this.xingdongId = renwu.Xingdong.Id;
        }

        
        public string id;

        public string icons;

        public string liuchengId;

        public string liuchengGuid;

        public string liuchengMingcheng;

        public string bianhao;

        public string mingcheng;

        public string faqiren;

        public string chuliren;

        public string url;

        /// <summary>
        /// 开始时间
        /// </summary>
        public string kaishiShijian;

        /// <summary>
        /// 期望完成时间
        /// </summary>
        public string qiwangWanchengShijian;

        /// <summary>
        /// 完成时间
        /// </summary>
        public string wanchengShijian;

        public string zhaiyao;

        public string zhuangtaiMingcheng;

        public RenwuZhuangtai zhuangtai;

        public string wanchengShuoming;

        public string xingdongId;
    }
}