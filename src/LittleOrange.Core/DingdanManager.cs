using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.Organization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class DingdanManager
    {
        ColdewObject _xiaoshouMingxiObject;
        ColdewObject _shoukuanGuanliObject;
        OrganizationManagement _orgManager;

        public DingdanManager(ColdewManager coldewManager)
        {
            this._xiaoshouMingxiObject = coldewManager.ObjectManager.GetObjectByCode("xiaoshouMingxi");
            this._shoukuanGuanliObject = coldewManager.ObjectManager.GetObjectByCode("shoukuanGuanli");
            this._orgManager = coldewManager.OrgManager;
        }

        public void CreateDingdan(JObject liuchengBiaodan)
        {
            JObject dingdan = this.CreateShoukuan(liuchengBiaodan);
            this.CreateXiaoshouMingxi(dingdan);
        }

        private JObject CreateShoukuan(JObject liuchengBiaodan)
        {
            JObject shoukuanXinxi = new JObject();
            shoukuanXinxi.Add("name", liuchengBiaodan["name"]);
            shoukuanXinxi.Add("fahuoRiqi", liuchengBiaodan["fahuoRiqi"]);
            shoukuanXinxi.Add("yewuyuan", liuchengBiaodan["yewuyuan"]);
            shoukuanXinxi.Add("kehu", liuchengBiaodan["kehu"]);
            string jiekuanFangshi = liuchengBiaodan["jiekuanFangshi"].ToString();
            shoukuanXinxi.Add("jiekuanFangshi", jiekuanFangshi);
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
            shoukuanXinxi.Add("jiekuanRiqi", jiekuanRiqi);
            double yingshoukuanJine = liuchengBiaodan["chanpinGrid"].Sum(x => (double)x["zongjine"]);
            shoukuanXinxi.Add("yingshoukuanJine", yingshoukuanJine);
            shoukuanXinxi.Add("chanpinGrid", liuchengBiaodan["chanpinGrid"]);
            this._shoukuanGuanliObject.MetadataManager.Create(this._orgManager.System, shoukuanXinxi);
            return shoukuanXinxi;
        }

        private void CreateXiaoshouMingxi(JObject dingdanObject)
        {
            foreach (JObject chanpinObject in dingdanObject["chanpinGrid"])
            {
                Chanpin chanpin = new Chanpin(chanpinObject);
                JObject dingdanPropertys = new JObject();
                dingdanPropertys.Add("yewuyuan", dingdanObject["yewuyuan"]);
                dingdanPropertys.Add("kehu", dingdanObject["kehu"]);
                dingdanPropertys.Add("fahuoRiqi", dingdanObject["fahuoRiqi"]);
                dingdanPropertys.Add("chuhuoDanhao", dingdanObject["name"]);

                foreach (JProperty property in chanpinObject.Properties())
                {
                    dingdanPropertys.Add(property.Name, property.Value.ToString());
                }
                this._xiaoshouMingxiObject.MetadataManager.Create(this._orgManager.System, dingdanPropertys);
            }
        }
    }
}
