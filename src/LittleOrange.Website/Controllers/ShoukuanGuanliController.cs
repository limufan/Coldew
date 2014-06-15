using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Website;
using Coldew.Website.Api.Models;
using Coldew.Website.Controllers;
using Coldew.Website.Models;
using LittleOrange.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Website.Controllers
{
    public class ShoukuanGuanliController : BaseController
    {
        //
        // GET: /ShoukuanGuanli/

        [HttpGet]
        public ActionResult Create(string objectId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);

            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            this.ViewBag.Title = "创建" + coldewObject.Name;

            return View();
        }

        [HttpPost]
        public ActionResult Create(string objectId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                JObject model = JsonConvert.DeserializeObject<JObject>(json);
                WebHelper.WebsiteMetadataService.Create(objectId, WebHelper.CurrentUserAccount, json);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string objectId, string metadataId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            this.ViewBag.metadataInfoJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            FormWebModel shoukuanModel = WebHelper.WebsiteFormService.GetFormByCode(this.CurrentUser.Account, "shoukuanMingxi", FormConstCode.DetailsFormCode);
            this.ViewBag.shoukuanModelJson = JsonConvert.SerializeObject(shoukuanModel);
            this.ViewBag.Title = "编辑" + coldewObject.Name;
            return View();
        }

        [HttpPost]
        public ActionResult EditPost(string objectId, string metadataId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                JObject dingdanObject = JsonConvert.DeserializeObject<JObject>(json);
                Dingdan dingdan = new Dingdan(dingdanObject);
                if (dingdanObject["shoukuanGrid"] != null)
                {
                    foreach (JObject shoukuan in dingdanObject["shoukuanGrid"])
                    {
                        shoukuan["ticheng"] = dingdan.JisuanTicheng(new Shoukuan(shoukuan));
                    }
                }
                dingdanObject["ticheng"] = dingdan.JisuanTicheng();
                dingdanObject["yishoukuanJine"] = dingdan.yishoukuanJine;
                dingdanObject["weishoukuanJine"] = dingdan.weishoukuanJine;
                dingdanObject["shifouShouwan"] = dingdan.weishoukuanJine <= 0 ? "是" : "否";

                //更新订单产品提成、收款金额
                foreach (JObject chanpin in dingdanObject["chanpinGrid"])
                {
                    chanpin["ticheng"] = dingdan.JisuanTicheng(new Chanpin(chanpin));
                    chanpin["shoukuanJine"] = dingdan.JisuanChanpinShoukuan((double)chanpin["zongjine"], dingdan.yishoukuanJine);
                }
                JObject xiaoshouMingxiSearchInfo = new JObject();
                xiaoshouMingxiSearchInfo.Add("chuhuoDanhao", dingdanObject["name"]);
                WebHelper.WebsiteMetadataService.Modify(objectId, WebHelper.CurrentUserAccount, metadataId, JsonConvert.SerializeObject(dingdanObject));

                //更新销售明细提成、收款金额
                ColdewObjectInfo xiaoshouMingxiObjectInfo = WebHelper.ColdewObjectService.GetObjectByCode("admin", "xiaoshouMingxi");
                string xiaoshouMingxiJson = WebHelper.WebsiteMetadataService.GetMetadatas("xiaoshouMingxi", "admin", JsonConvert.SerializeObject(xiaoshouMingxiSearchInfo), "");
                List<JObject> xiaoshouMingxiList = JsonConvert.DeserializeObject<List<JObject>>(xiaoshouMingxiJson);
                foreach (JObject xiaoshouMingxi in xiaoshouMingxiList)
                {
                    xiaoshouMingxi["ticheng"] = dingdan.JisuanTicheng(new Chanpin(xiaoshouMingxi));
                    xiaoshouMingxi["shoukuanJine"] = dingdan.JisuanChanpinShoukuan((double)xiaoshouMingxi["zongjine"], dingdan.yishoukuanJine);
                }
                foreach (JObject xiaoshouMingxi in xiaoshouMingxiList)
                {
                    WebHelper.WebsiteMetadataService.Modify(xiaoshouMingxiObjectInfo.ID, "admin", 
                        xiaoshouMingxi["id"].ToString(), JsonConvert.SerializeObject(xiaoshouMingxi));
                }
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult JisuanTicheng(string objectId, string metadataId, string shoukuanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                string metadataJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
                JObject dingdanObject = JsonConvert.DeserializeObject<JObject>(metadataJson);
                Dingdan dingdan = new Dingdan(dingdanObject);
                Shoukuan shoukuan = JsonConvert.DeserializeObject<Shoukuan>(shoukuanJson);
                resultModel.data = dingdan.JisuanTicheng(shoukuan);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Details(string objectId, string metadataId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            this.ViewBag.Title = coldewObject.Name + " - 详细信息";

            this.ViewBag.metadataInfoJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            return View();
        }

    }
}
