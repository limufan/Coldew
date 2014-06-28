using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api.Workflow;
using System.Web.Mvc;
using Coldew.Api.Organization;
using Coldew.Api;

namespace Coldew.Website.Models
{
    public class LiuchengInfoModel
    {
        public LiuchengInfoModel(RenwuXinxi renwu)
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
        public RenwuModel(RenwuXinxi renwu, Controller controller, UserInfo currentUser)
        {
            this.bianhao = renwu.Bianhao;
            this.id = renwu.Id;
            this.guid = renwu.Guid;
            this.kaishiShijian = renwu.Xingdong.KaishiShijian.ToString();
            this.mingcheng = renwu.Xingdong.Name;
            if (!renwu.Xingdong.QiwangWanchengShijian.HasValue)
            {
                this.qiwangWanchengShijian = renwu.Xingdong.QiwangWanchengShijian.ToString();
            }
            this.liuchengId = renwu.Xingdong.liucheng.Id;
            this.liuchengGuid = renwu.Xingdong.liucheng.Guid;
            this.liuchengtuUrl = string.Format("{0}?liuchengId={1}", controller.Url.Action("Liuchengtu"), renwu.Xingdong.liucheng.Guid);
            this.liuchengMingcheng = renwu.Xingdong.LiuchengMingcheng;
            if (renwu.ChuliShijian.HasValue)
            {
                this.wanchengShijian = renwu.ChuliShijian.ToString();
            }
            this.zhaiyao = renwu.Xingdong.Zhaiyao;

            this.zhuangtaiMingcheng = EnumMapper.Map(renwu.Zhuangtai);
            this.zhuangtai = renwu.Zhuangtai;
            this.url = string.Format("{0}?renwuId={1}&liuchengId={2}&uid={3}", 
                controller.Url.Content(renwu.Xingdong.liucheng.Liucheng.TransferUrl), renwu.Guid, renwu.Xingdong.liucheng.Guid, currentUser.ID);
            this.faqiren = renwu.Xingdong.liucheng.Faqiren.Name;
            this.chuliren = renwu.Chuliren.Name;
            this.wanchengShuoming = renwu.ChuliShuoming;
            this.icons = "";
            if (renwu.Xingdong.Jinjide)
            {
                this.icons += "<span title='紧急的任务' class='icon-jinji-renwu'></span>";
            }
            
            //this.mingcheng = string.Format("<span>{0}</span>{1}", renwu.Xingdong.Mingcheng, this.icons);
        }

        

        public int id;

        public string guid;

        public string icons;

        public int liuchengId;

        public string liuchengGuid;

        public string liuchengtuUrl;

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
    }
}