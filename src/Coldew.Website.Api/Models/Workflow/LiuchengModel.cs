using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api.Workflow;
using Coldew.Api.Organization;
using Coldew.Core.Workflow;

namespace Coldew.Website.Api.Models
{
    public class LiuchengModel
    {
        public LiuchengModel(Liucheng liucheng)
        {
            this.faqiren = liucheng.Faqiren.Name;
            this.faqirenAccount = liucheng.Faqiren.Account;
            this.faqiShijian = liucheng.FaqiShijian.ToString();
            this.Id = liucheng.Id;
            if (liucheng.JieshuShijian.HasValue)
            {
                this.jieshuShijian = liucheng.JieshuShijian.Value.ToString();
            }
            this.mingcheng = liucheng.Mingcheng;
            this.zhuangtai = this.Map(liucheng.Zhuangtai);
            this.zhaiyao = liucheng.Zhaiyao;
            this.biaodanId = liucheng.BiaodanId;
        }

        private string Map(LiuchengZhuangtai zhuangtai)
        {
            switch (zhuangtai)
            {
                case LiuchengZhuangtai.Chulizhong: return "处理中";
                case LiuchengZhuangtai.Wanchengle: return "已完成";
            }
            return "";
        }

        public string Id;

        public string mingcheng;

        public string faqiren;

        public string faqirenAccount;

        public string faqiShijian;

        public string jieshuShijian;

        public string zhuangtai;

        public string zhaiyao;

        public string biaodanId;
    }
}