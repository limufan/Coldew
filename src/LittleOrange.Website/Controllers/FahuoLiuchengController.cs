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
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            
            return View();
        }

        [HttpPost]
        public ActionResult Faqi(string mobanId, string biaodanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

                string createdBiaodanJson = WebHelper.WebsiteMetadataService.Create(objectInfo.ID, this.CurrentUser.Account, biaodanJson);
                JObject biaodanJObject = JsonConvert.DeserializeObject<JObject>(createdBiaodanJson);
                string biaodanId = biaodanJObject["id"].ToString();

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.FaqiLiucheng(mobanId, "yewuyuan", "业务员", "", this.CurrentUser.Account, false, "", biaodanId);
                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "shenhe", "审核", new List<string> { "chenxia", "chenmei" }, "", null);

                JObject modifyObject = new JObject();
                modifyObject.Add("liuchengId", liucheng.Guid);
                WebHelper.WebsiteMetadataService.Modify(objectInfo.ID, this.CurrentUser.Account, biaodanId, JsonConvert.SerializeObject(modifyObject));
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
        public ActionResult Faqi_Tuihui(string renwuId, string liuchengId)
        {
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpPost]
        public ActionResult Faqi_Tuihui_Submit(string renwuId, string liuchengId, string wanchengShuoming, string biaodanJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                WebHelper.WebsiteMetadataService.Modify(objectInfo.ID, this.CurrentUser.Account, liucheng.BiaodanId, biaodanJson);

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Guid);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "shenhe", "审核", new List<string> { "chenxia", "chenmei" }, "", null);
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
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
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

                ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Guid);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "fahuo", "发货", new List<string> { "shenqiudi" }, "", null);

                ColdewObjectInfo dingdanZhongbiaoObject = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "dingdanZhongbiao");
                string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
                JObject biaodan = JsonConvert.DeserializeObject<JObject>(biaodanJson);
                foreach (JObject chanpin in biaodan["chanpinGrid"])
                {
                    JObject dingdanPropertys = new JObject();
                    dingdanPropertys.Add("yewuyuan", liucheng.Faqiren.Account);
                    
                    dingdanPropertys.Add("kehu", biaodan["kehu"]);
                    dingdanPropertys.Add("shengfen", biaodan["shengfen"]);
                    dingdanPropertys.Add("diqu", biaodan["diqu"]);
                    dingdanPropertys.Add("fahuoRiqi", biaodan["fahuoRiqi"]);
                    dingdanPropertys.Add("huikuanRiqi", biaodan["huikuanRiqi"]);
                    dingdanPropertys.Add("huikuanJine", biaodan["huikuanJine"]);
                    dingdanPropertys.Add("huikuanLeixing", biaodan["huikuanLeixing"]);
                    dingdanPropertys.Add("huikuanDanwei", biaodan["huikuanDanwei"]);
                    dingdanPropertys.Add("daokuanDanwei", biaodan["daokuanDanwei"]);
                    dingdanPropertys.Add("kaipiaoDanwei", biaodan["kaipiaoDanwei"]);
                    dingdanPropertys.Add("shouhuoDizhi", biaodan["shouhuoDizhi"]);
                    dingdanPropertys.Add("shouhuoren", biaodan["shouhuoren"]);

                    foreach (JProperty property in chanpin.Properties())
                    {
                        dingdanPropertys.Add(property.Name, property.Value.ToString());
                    }
                    WebHelper.WebsiteMetadataService.Create(dingdanZhongbiaoObject.ID, this.CurrentUser.Account, JsonConvert.SerializeObject(dingdanPropertys));
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
        public ActionResult Tuihui(string renwuId, string liuchengId, string wanchengShuoming)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);

                ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Guid);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "faqi_tuihui", "退回业务员", new List<string> { liucheng.Faqiren.Account }, "", null);

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
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, "form_fahuo");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
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

                ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);

                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Guid);
                WebHelper.LiuchengFuwu.Wancheng(liuchengId);
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
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
            this.ViewBag.biaodanJson = biaodanJson;

            return View();
        }

        [HttpGet]
        public ActionResult FahuoMingxi(string liuchengId)
        {
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            this.ViewBag.objectInfo = objectInfo;

            LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.GetLiucheng(liuchengId);
            List<RenwuXinxi> renwuXinxi = WebHelper.RenwuFuwu.GetLiuchengRenwu(liuchengId);
            this.ViewBag.renwuModelsJson = JsonConvert.SerializeObject(renwuXinxi.Select(x => new RenwuModel(x, this, this.CurrentUser)).ToList());

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, "form_fahuo");
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
            this.ViewBag.biaodanJson = biaodanJson;

            return View("Mingxi");
        }

        [HttpGet]
        public ActionResult Details(string metadataId, string objectId)
        {
            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            JObject biaodan = JsonConvert.DeserializeObject<JObject>(biaodanJson);
            string liuchengId = biaodan["liuchengId"].ToString();
            return this.RedirectToAction("Mingxi", new { liuchengId = liuchengId });
        }
    }
}
