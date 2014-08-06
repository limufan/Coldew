using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api.Workflow;
using Coldew.Workflow;


namespace Coldew.Website.Api.Models
{
    public class JianglaiZhipaiModel
    {
        public JianglaiZhipaiModel(JianglaiRenwuZhipai zhipai)
        {
            this.dailirenXingming = zhipai.Dailiren.Name;
            this.dailirenZhanghao = zhipai.Dailiren.Account;
            if (zhipai.KaishiShijian.HasValue)
            {
                this.kaishiShijian = zhipai.KaishiShijian.Value.ToString("yyyy-MM-dd");
            }
            if (zhipai.JieshuShijian.HasValue)
            {
                this.jieshuShijian = zhipai.JieshuShijian.Value.ToString("yyyy-MM-dd");
            }
        }

        public string dailirenXingming;

        public string dailirenZhanghao;

        public string kaishiShijian;

        public string jieshuShijian ;
    }
}