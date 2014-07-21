using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Website.Models;
using Coldew.Api.Workflow;
using Newtonsoft.Json;
using Coldew.Api.UI;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Controllers
{
    public class WorkflowController : BaseController
    {
        //
        // GET: /Workflow/

        public ActionResult Daibande()
        {
            return View();
        }

        public ActionResult DaibanRenwu(string liuchengMobanId, DateTime? kaishiShijian, DateTime? jieshuShijian, string zhaiyao, int start, int size)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                if (jieshuShijian.HasValue)
                {
                    jieshuShijian = jieshuShijian.Value.AddHours(24);
                }
                int count;
                List<RenwuModel> renwuModels = WebHelper.RenwuFuwu.GetChulizhongdeRenwu(WebHelper.CurrentUserAccount, liuchengMobanId, kaishiShijian, jieshuShijian, zhaiyao, start, size, out count);
                DatagridModel gridModel = new DatagridModel();
                gridModel.list = renwuModels;
                gridModel.count = count;
                resultModel.data = gridModel;
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Zhipai(string argsJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                RenwuZhipaiModel model = JsonConvert.DeserializeObject<RenwuZhipaiModel>(argsJson);
                WebHelper.RenwuFuwu.ZhipaiRenwu(model.renwuId, model.yonghuZhanghao);
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ZhipaiSuoyou(string yonghuZhanghao)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                WebHelper.RenwuFuwu.ZhipaiSuoyouRenwu(WebHelper.CurrentUserAccount, yonghuZhanghao);
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult JianglaiZhipai()
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                JianglaiZhipaiModel zhipai = WebHelper.RenwuFuwu.GetJianglaiZhipai(WebHelper.CurrentUserAccount);
                if (zhipai != null)
                {
                    resultModel.data = zhipai;
                }
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult JianglaiZhipai(string dailirenZhanghao, DateTime? kaishiShijian, DateTime? jieshuShijian)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                WebHelper.RenwuFuwu.SetJianglaiZhipai(WebHelper.CurrentUserAccount, dailirenZhanghao, kaishiShijian, jieshuShijian);
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Faqide()
        {
            return View();
        }

        public ActionResult FaqiLiucheng()
        {
            return View();
        }

        public ActionResult Liucheng()
        {

            List<LiuchengMobanModel> list = WebHelper.YinqingFuwu.GetLiuchengMobanByYonghu(this.CurrentUser.Account);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Guidangde()
        {
            return View();
        }

        public ActionResult GuidangdeRenwu(string liuchengMobanId, DateTime? wanchengKaishiShijian, DateTime? wanchengJieshuShijian, string zhaiyao, int start, int size)
        {

            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                if (wanchengJieshuShijian.HasValue)
                {
                    wanchengJieshuShijian = wanchengJieshuShijian.Value.AddHours(24);
                }
                int count;
                var models = WebHelper.RenwuFuwu.GetGuidangdeRenwu(WebHelper.CurrentUserAccount, liuchengMobanId, wanchengKaishiShijian, wanchengJieshuShijian, zhaiyao, start, size, out count);
                DatagridModel gridModel = new DatagridModel();
                gridModel.list = models;
                gridModel.count = count;
                resultModel.data = gridModel;
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Yibande()
        {
            return View();
        }

        public ActionResult YibandeRenwu(string liuchengMobanId, DateTime? wanchengKaishiShijian, DateTime? wanchengJieshuShijian, string zhaiyao, int start, int size)
        {

            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                if (wanchengJieshuShijian.HasValue)
                {
                    wanchengJieshuShijian = wanchengJieshuShijian.Value.AddHours(24);
                }
                int count;
                var models = WebHelper.RenwuFuwu.GetWanchengdeRenwu(WebHelper.CurrentUserAccount, liuchengMobanId, wanchengKaishiShijian, wanchengJieshuShijian, zhaiyao, start, size, out count);
                DatagridModel gridModel = new DatagridModel();
                gridModel.list = models;
                gridModel.count = count;
                resultModel.data = gridModel;
            }
            catch (Exception ex)
            {
                WebHelper.Logger.Error(ex.Message, ex);
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Zhipaide()
        {
            return View();
        }

        public ActionResult ZhipaideRenwu(int start, int size)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                int count;
                List<RenwuModel> xingdongList = WebHelper.RenwuFuwu.GetZhipaideRenwu(WebHelper.CurrentUserAccount, start, size, out count);

                DatagridModel gridModel = new DatagridModel();
                gridModel.list = xingdongList;
                gridModel.count = count;
                resultModel.data = gridModel;
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuxiaoZhipai(string argsJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                QuxiaoZhipaiModel model = JsonConvert.DeserializeObject<QuxiaoZhipaiModel>(argsJson);
                WebHelper.RenwuFuwu.QuxiaoZhipai(model.renwuId);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}
