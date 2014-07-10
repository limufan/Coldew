using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Website.Models;
using Newtonsoft.Json;
using Coldew.Website.Controllers;
using Coldew.Website;
using Coldew.Api;
using Newtonsoft.Json.Linq;
using Coldew.Api.Workflow;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;
using Coldew.Api.UI;
using Coldew.Api.Exceptions;
using LittleOrange.Core;

namespace LittleOrange.Website.Controllers
{
    public class FahuoLiuchengController : BaseController
    {
        //
        // GET: /FahuoLiucheng/

        public ActionResult Index(string renwuId, string liuchengId, string mobanId)
        {
            if (string.IsNullOrEmpty(renwuId))
            {
                return this.RedirectToAction("Faqi", new { mobanId = mobanId });
            }
            RenwuXinxi renwu = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
            if (renwu == null)
            {
                this.ViewBag.error = "找不到该任务，或者该任务已经被取消！";
                return View("Error");
            }
            if (renwu.Bianhao == "fahuo")
            {
                if (renwu.Zhuangtai == RenwuZhuangtai.Wanchengle)
                {
                    return this.RedirectToAction("FahuoMingxi", new { renwuId = renwuId, liuchengId = liuchengId });
                }
                else
                {
                    return this.RedirectToAction("Fahuo", new { renwuId = renwuId, liuchengId = liuchengId });
                }
            }
            else if (renwu.Zhuangtai == RenwuZhuangtai.Wanchengle)
            {
                return this.RedirectToAction("Mingxi", new { renwuId = renwuId, liuchengId = liuchengId });
            }
            else if (renwu.Bianhao == "shenhe")
            {
                return this.RedirectToAction("Shenhe", new { renwuId = renwuId, liuchengId = liuchengId });
            }
            else if (renwu.Bianhao == "faqi_tuihui")
            {
                return this.RedirectToAction("Faqi_Tuihui", new { renwuId = renwuId, liuchengId = liuchengId });
            }

            return View();
        }

        [HttpGet]
        public ActionResult Faqi()
        {
            ColdewObjectWebModel liuchengObjectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, liuchengObjectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            
            return View();
        }

        [HttpPost]
        public ActionResult Faqi(string mobanId, string biaodanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
                
                string createdBiaodanJson = WebHelper.WebsiteMetadataService.Create(objectInfo.id, this.CurrentUser.Account, biaodanJson);
                JObject biaodanJObject = JsonConvert.DeserializeObject<JObject>(createdBiaodanJson);
                string biaodanId = biaodanJObject["id"].ToString();

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.FaqiLiucheng(mobanId, "yewuyuan", "发货申请", "", this.CurrentUser.Account, false, "", biaodanId);
                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Id, "shenhe", "审核", new List<string> { "mengdong"}, "", null);

                this.SetLliuchengInfo(objectInfo, liucheng);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        private void SetLliuchengInfo(ColdewObjectWebModel objectInfo, LiuchengXinxi liucheng)
        {
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liucheng.Id);
            JArray liuchengInfoModels = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(renwuXinxi.Select(x => new LiuchengInfoModel(x)).ToList()));
            JObject modifyObject = new JObject();
            modifyObject.Add("liuchengInfoGrid", liuchengInfoModels);
            WebHelper.WebsiteMetadataService.Modify(objectInfo.id, this.CurrentUser.Account, liucheng.BiaodanId, JsonConvert.SerializeObject(modifyObject));
        }

        [HttpGet]
        public ActionResult Faqi_Tuihui(string renwuId, string liuchengId)
        {
            ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetFormJson(this.CurrentUser.Account, objectInfo.id, liucheng.BiaodanId, formModel.id);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpPost]
        public ActionResult Faqi_Tuihui_Submit(string renwuId, string liuchengId, string wanchengShuoming, string biaodanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                WebHelper.WebsiteMetadataService.Modify(objectInfo.id, this.CurrentUser.Account, liucheng.BiaodanId, biaodanJson);

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Id);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Id, "shenhe", "审核", new List<string> { "mengdong" }, "", null);

                this.SetLliuchengInfo(objectInfo, liucheng);
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
        public ActionResult Shenhe(string renwuId, string liuchengId)
        {
            ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetFormJson(this.CurrentUser.Account, objectInfo.id, liucheng.BiaodanId, formModel.id);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpPost]
        public ActionResult Shenhe(string renwuId, string liuchengId, string wanchengShuoming)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Id);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Id, "fahuo", "发货", new List<string> { "fahuoyuan" }, "", null);
                ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
                this.SetLliuchengInfo(objectInfo, liucheng);
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
        public ActionResult Tuihui(string renwuId, string liuchengId, string wanchengShuoming)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Id);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Id, "faqi_tuihui", "退回业务员", new List<string> { liucheng.Faqiren.Account }, "", null);

                this.SetLliuchengInfo(objectInfo, liucheng);
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
        public ActionResult Fahuo(string renwuId, string liuchengId)
        {
            ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetFormJson(this.CurrentUser.Account, objectInfo.id, liucheng.BiaodanId, formModel.id);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpPost]
        public ActionResult Fahuo(string renwuId, string liuchengId, string wanchengShuoming)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);

                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Id);
                WebHelper.LiuchengFuwu.Wancheng(liuchengId);

                this.SetLliuchengInfo(objectInfo, liucheng);
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
        public ActionResult Mingxi(string liuchengId)
        {
            ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetFormJson(this.CurrentUser.Account, objectInfo.id, liucheng.BiaodanId, formModel.id);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpGet]
        public ActionResult FahuoMingxi(string liuchengId)
        {
            ColdewObjectWebModel objectInfo = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.id, "fahuo_liucheng_form");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetFormJson(this.CurrentUser.Account, objectInfo.id, liucheng.BiaodanId, formModel.id);
            this.ViewBag.biaodanJson = biaodanJson;

            return View("Mingxi");
        }

    }
}
