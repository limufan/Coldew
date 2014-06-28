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
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.EditFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            FormWebModel shoukuanModel = WebHelper.WebsiteFormService.GetFormByCode(this.CurrentUser.Account, "shoukuanMingxi", FormConstCode.EditFormCode);
            this.ViewBag.shoukuanModelJson = JsonConvert.SerializeObject(shoukuanModel);

            return View();
        }

        [HttpPost]
        public ActionResult Create(string objectId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                JObject dingdanObject = JsonConvert.DeserializeObject<JObject>(json);
                Dingdan dingdan = new Dingdan(dingdanObject);
                dingdan.Zhuangtai = DingdanZhuangtai.wancheng;
                json = JsonConvert.SerializeObject(dingdan);
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
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.EditFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            FormWebModel shoukuanModel = WebHelper.WebsiteFormService.GetFormByCode(this.CurrentUser.Account, "shoukuanMingxi", FormConstCode.EditFormCode);
            this.ViewBag.shoukuanModelJson = JsonConvert.SerializeObject(shoukuanModel);
            return View();
        }

        [HttpPost]
        public ActionResult EditPost(string objectId, string metadataId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                WebHelper.WebsiteMetadataService.Modify(objectId, WebHelper.CurrentUserAccount, metadataId, json);
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
        public ActionResult JisuanTicheng(string objectId, string dingdanJson, string shoukuanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            Shoukuan shoukuan = null;
            try
            {

                shoukuan = new Shoukuan(JsonConvert.DeserializeObject<JObject>(shoukuanJson));
            }
            catch
            {
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            }
            try
            {
                JObject dingdanObject = JsonConvert.DeserializeObject<JObject>(dingdanJson);
                Dingdan dingdan = new Dingdan(dingdanObject);
                dingdan.Jisuan();
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
    }
}
