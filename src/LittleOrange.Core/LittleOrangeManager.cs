using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Workflow;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class LittleOrangeManager : ColdewManager
    {
        LiuchengMoban _moban;

        public LittleOrangeManager()
        {
        }

        protected override void Load()
        {
            base.Load();
            this.BindOrangeEvent();
        }

        public void BindOrangeEvent()
        {
            this._moban = this.LiuchengYinqing.LiuchengMobanManager.GetMobanByCode("FahuoLiucheng");
            this.LiuchengYinqing.LiuchengManager.LiuchengWanchenghou += LiuchengManager_LiuchengWanchenghou;
            ColdewObject cobject = this.ObjectManager.GetObjectByCode("shoukuanGuanli");
            if (cobject != null)
            {
                cobject.MetadataManager.Creating += MetadataManager_Creating;
                cobject.MetadataManager.MetadataChanging += MetadataManager_MetadataChanging;
            }
        }

        void MetadataManager_Creating(MetadataManager sender, JObject jobject)
        {
            this.JisuanDingdanInfo(jobject);
            jobject["zhuangtai"] = "审核";
        }

        private void MetadataManager_MetadataChanging(MetadataManager sender, JObject jobject)
        {
            this.JisuanDingdanInfo(jobject);
        }

        private void JisuanDingdanInfo(JObject jobject)
        {
            if (jobject["jiekuanFangshi"] != null)
            {
                string jiekuanFangshi = jobject["jiekuanFangshi"].ToString();
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
                jobject["jiekuanRiqi"] = jiekuanRiqi;
            }
            if (jobject["chanpinGrid"] != null)
            {
                double yingshoukuanJine = jobject["chanpinGrid"].Sum(x => (double)x["zongjine"]);
                jobject["yingshoukuanJine"] = yingshoukuanJine;
            }
        }

        void LiuchengManager_LiuchengWanchenghou(Liucheng liucheng)
        {
            Metadata dingdan = this._moban.ColdewObject.MetadataManager.GetById(liucheng.BiaodanId);
            JObject jisuanPropertys = new JObject();
            jisuanPropertys["zhuangtai"] = "完成";
            dingdan.SetPropertys(this.OrgManager.System, jisuanPropertys);
            this.CreateXiaoshouMingxi(dingdan.MapJObject(this.OrgManager.System));
        }

        private void CreateXiaoshouMingxi(JObject dingdanObject)
        {
            ColdewObject xiaoshouMingxiObject = this.ObjectManager.GetObjectByCode("xiaoshouMingxi");
            foreach (JObject chanpinObject in dingdanObject["chanpinGrid"])
            {
                JObject dingdanPropertys = new JObject();
                dingdanPropertys.Add("yewuyuan", dingdanObject["yewuyuan"]);
                dingdanPropertys.Add("kehu", dingdanObject["kehu"]);
                dingdanPropertys.Add("fahuoRiqi", dingdanObject["fahuoRiqi"]);
                dingdanPropertys.Add("fahuoDanhao", dingdanObject["fahuoDanhao"]);

                foreach (JProperty property in chanpinObject.Properties())
                {
                    dingdanPropertys.Add(property.Name, property.Value.ToString());
                }
                xiaoshouMingxiObject.MetadataManager.Create(this.OrgManager.System, dingdanPropertys);
            }
        }
    }
}
