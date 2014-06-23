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
            cobject.MetadataManager.Creating += MetadataManager_Creating;
            cobject.MetadataManager.MetadataChanging += MetadataManager_MetadataChanging;
        }

        void MetadataManager_Creating(MetadataManager sender, Metadata metadata)
        {
            this.JisuanDingdanInfo(metadata);
        }

        private void MetadataManager_MetadataChanging(MetadataManager sender, Metadata metadata)
        {
            this.JisuanDingdanInfo(metadata);
        }

        private void JisuanDingdanInfo(Metadata metadata)
        {
            JObject jisuanPropertys = new JObject();
            string jiekuanFangshi = metadata.GetProperty("jiekuanFangshi").Value.Value;
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
            jisuanPropertys["jiekuanRiqi"] = jiekuanRiqi;
            JArray chanpinArray = metadata.GetProperty("chanpinGrid").Value.Value;
            double yingshoukuanJine = chanpinArray.Sum(x => (double)x["zongjine"]);
            jisuanPropertys["yingshoukuanJine"] = yingshoukuanJine;
            jisuanPropertys["zhuangtai"] = "审核";
            metadata.SetPropertys(this.OrgManager.System, jisuanPropertys);
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
