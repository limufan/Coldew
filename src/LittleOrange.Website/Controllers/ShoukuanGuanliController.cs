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
                JObject model = JsonConvert.DeserializeObject<JObject>(json);
                JArray chanpinArray = (JArray)model["chanpinGrid"];
                int chanpinShuliang = chanpinArray.Count;
                DateTime jiekuanRiqi = (DateTime)model["jiekuanRiqi"];
                double yingshoukuanJine = (double)model["yingshoukuanJine"];
                double yishoukuanJine = 0;
                double ticheng = 0; 
                if (model["shoukuanGrid"] != null)
                {
                    foreach (JObject shoukuan in model["shoukuanGrid"])
                    {
                        double shoukuanTicheng = 0;
                        DateTime shoukuanRiqi = (DateTime)shoukuan["shoukuanRiqi"];
                        double shoukuanJine = (double)shoukuan["shoukuanJine"];
                        yishoukuanJine += shoukuanJine;

                        foreach (JObject chanpin in model["chanpinGrid"])
                        {
                            double xiaoshouDijia = (double)chanpin["xiaoshouDijia"];
                            double shijiDanjia = (double)chanpin["shijiDanjia"];
                            double xiaoshouDanjia = (double)chanpin["xiaoshouDanjia"];
                            string shifouKaipiao = (string)chanpin["shifouKaipiao"];
                            shoukuanTicheng += TichengJisuanqi.Jisuan(xiaoshouDijia, shoukuanJine / chanpinShuliang, shijiDanjia, xiaoshouDanjia, shifouKaipiao == "是", jiekuanRiqi, shoukuanRiqi);
                        }
                        shoukuan["ticheng"] = shoukuanTicheng;
                        ticheng += shoukuanTicheng;
                    }
                }
                model["ticheng"] = ticheng;
                model["yishoukuanJine"] = yishoukuanJine;
                double weishoukuanJine = yingshoukuanJine - yishoukuanJine;
                model["weishoukuanJine"] = weishoukuanJine;
                if (weishoukuanJine <= 0)
                {
                    model["shifouShouwan"] = "是";
                }
                WebHelper.WebsiteMetadataService.Modify(objectId, WebHelper.CurrentUserAccount, metadataId, JsonConvert.SerializeObject(model));
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
