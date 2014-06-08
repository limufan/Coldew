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
            ColdewObjectInfo liuchengObjectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, liuchengObjectInfo.ID, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);

            ColdewObjectInfo xiaoshouObjectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "xiaoshouMingxi");
            FormWebModel chanpinModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, xiaoshouObjectInfo.ID, "fahuo_chanpin_form");
            this.ViewBag.chanpinModelJson = JsonConvert.SerializeObject(chanpinModel);
            
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

                LiuchengXinxi liucheng = WebHelper.LiuchengFuwu.FaqiLiucheng(mobanId, "yewuyuan", "发货申请", "", this.CurrentUser.Account, false, "", biaodanId);
                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "shenhe", "审核", new List<string> { "mengdong"}, "", null);

                JObject modifyObject = new JObject();
                modifyObject.Add("liuchengId", liucheng.Guid);
                WebHelper.WebsiteMetadataService.Modify(objectInfo.ID, this.CurrentUser.Account, biaodanId, JsonConvert.SerializeObject(modifyObject));
                this.CreateShoukuan(liucheng);
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

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "shenhe", "审核", new List<string> { "mengdong" }, "", null);
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

                RenwuXinxi renwuXinxi = WebHelper.RenwuFuwu.GetRenwu(liuchengId, renwuId);
                WebHelper.RenwuFuwu.WanchengRenwu(liuchengId, this.CurrentUser.Account, renwuId, wanchengShuoming);
                WebHelper.RenwuFuwu.WanchengXingdong(liuchengId, renwuXinxi.Xingdong.Guid);

                WebHelper.RenwuFuwu.ChuangjianXingdong(liucheng.Guid, "fahuo", "发货", new List<string> { "fahuoyuan" }, "", null);
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

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectInfo.ID, FormConstCode.DetailsFormCode);
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
                this.CreateXiaoshouMingxi(liucheng);
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

        //private void FillChanpinXinxi(JObject biaodan)
        //{
        //    ColdewObjectInfo chanpinObjectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "chanpin");
        //    foreach (JObject chanpin in biaodan["chanpinGrid"])
        //    {
        //        string chanpinName = chanpin["name"].ToString();
        //        MetadataInfo chanpinInfo = WebHelper.WebsiteMetadataService.GetMetadataByName(this.CurrentUser.Account, chanpinObjectInfo.ID, chanpinName);
        //        if (chanpinInfo == null)
        //        {
        //            throw new ColdewException("没有产品信息:" + chanpinName);
        //        }
        //        string chanpinJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, chanpinObjectInfo.ID, chanpinInfo.ID);
        //        JObject chanpinObject = JsonConvert.DeserializeObject<JObject>(chanpinJson);
        //        double xiaoshouDijia = (double)chanpinObject["xiaoshouDijia"];
        //        chanpin.Add("xiaoshouDijia", xiaoshouDijia);
        //    }
        //}

        private void CreateShoukuan(LiuchengXinxi liucheng)
        {
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            ColdewObjectInfo xiaoshouGuanliObject = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "shoukuanGuanli");
            string liuchengBiaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
            JObject liuchengBiaodan = JsonConvert.DeserializeObject<JObject>(liuchengBiaodanJson);
            JObject shoukuanXinxi = new JObject();
            shoukuanXinxi.Add("name", liuchengBiaodan["name"]);
            shoukuanXinxi.Add("fahuoRiqi", liuchengBiaodan["fahuoRiqi"]);
            shoukuanXinxi.Add("yuejieRiqi", liuchengBiaodan["yuejieRiqi"]);
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
            WebHelper.WebsiteMetadataService.Create(xiaoshouGuanliObject.ID, this.CurrentUser.Account, JsonConvert.SerializeObject(shoukuanXinxi));
        }

        private void CreateXiaoshouMingxi(LiuchengXinxi liucheng)
        {
            ColdewObjectInfo chanpinObjectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "chanpin");
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "FahuoLiucheng");
            ColdewObjectInfo xiaoshouMingxiObject = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, "xiaoshouMingxi");
            string biaodanJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, liucheng.BiaodanId);
            JObject biaodan = JsonConvert.DeserializeObject<JObject>(biaodanJson);
            foreach (JObject chanpin in biaodan["chanpinGrid"])
            {
                JObject dingdanPropertys = new JObject();
                dingdanPropertys.Add("yewuyuan", biaodan["yewuyuan"]);
                dingdanPropertys.Add("kehu", biaodan["kehu"]);
                dingdanPropertys.Add("fahuoRiqi", biaodan["fahuoRiqi"]);
                dingdanPropertys.Add("chuhuoDanhao", biaodan["name"]);
                //计算实际单价
                double xiaoshouDanjia = (double)chanpin["xiaoshouDanjia"];
                double yewufei = (double)chanpin["yewufei"];
                double shuliang = (double)chanpin["shuliang"];
                double zongjine = (double)chanpin["zongjine"];
                double shijiDanjia = (zongjine-yewufei) / shuliang * 0.83;
                dingdanPropertys.Add("shijiDanjia", shijiDanjia);

                //计算提成
                double xiaoshouDijia = (double)chanpin["xiaoshouDijia"];
                string shifouKaipiao = (string)chanpin["shifouKaipiao"];
                double ticheng = TichengJisuanqi.Jisuan(xiaoshouDijia, zongjine, shijiDanjia, xiaoshouDanjia, shifouKaipiao == "是");
                dingdanPropertys.Add("ticheng", ticheng);

                //计算补贴
                double youfeiButie = shijiDanjia * shuliang * 0.01;
                double chanfeiButie = shijiDanjia * shuliang * 0.01;
                dingdanPropertys.Add("butie", youfeiButie + chanfeiButie);

                foreach (JProperty property in chanpin.Properties())
                {
                    dingdanPropertys.Add(property.Name, property.Value.ToString());
                }
                WebHelper.WebsiteMetadataService.Create(xiaoshouMingxiObject.ID, this.CurrentUser.Account, JsonConvert.SerializeObject(dingdanPropertys));
            }
        }
    }
}
