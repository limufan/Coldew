using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Api;
using Coldew.Website.Models;
using Newtonsoft.Json;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Data;
using Newtonsoft.Json.Linq;
using Coldew.Api.Organization;
using System.Text.RegularExpressions;
using Coldew.Api.UI;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Controllers
{
    public class MetadataController : BaseController
    {

        public ActionResult Index(string objectId, string viewId)
        {
            MetadtaGridPageModel pageModel = WebHelper.WebsiteColdewObjectService.GetGridPageModel(this.CurrentUser.Account, objectId, viewId);
            this.ViewBag.pageModelJson = JsonConvert.SerializeObject(pageModel);
            return View();
        }

        [HttpPost]
        public ActionResult Favorites(string objectId, string metadataIdsJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                List<string> metadataIds = JsonConvert.DeserializeObject<List<string>>(metadataIdsJson);
                WebHelper.WebsiteMetadataService.ToggleFavorite(objectId, WebHelper.CurrentUserAccount, metadataIds);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportFirst(string objectId)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);

            this.ViewBag.Title = "导入" + coldewObject.name;
            return View();
        }

        public ActionResult DownloadImportTemplate(string objectId)
        {
            string tempPath = ImportExportHelper.GetImportTemplate(this.CurrentUser.Account, objectId, this);
            return File(tempPath, "application/octet-stream", "Import Template.xls");
        }

        [HttpPost]
        public ActionResult UploadImportFile(string objectId)
        {
            string jsonFilePath = ImportExportHelper.GetUploadImportFileJsonFile(this.CurrentUser.Account, objectId, this);
            return Redirect(this.Url.Action("ImportSecond", new { tempFileName = Path.GetFileName(jsonFilePath), objectId = objectId }));
        }

        public ActionResult ImportSecond(string objectId)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);

            this.ViewBag.Title = "导入" + coldewObject.name;
            List<DataGridColumnModel> columns = ImportExportHelper.GetImportColumns(this.CurrentUser.Account, objectId);
            this.ViewBag.columnsJson = JsonConvert.SerializeObject(columns);

            return View();
        }

        [HttpPost]
        public ActionResult Import(string tempFileName, string objectId)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                string tempFilePath = Path.Combine(Server.MapPath("~/Temp"), tempFileName);
                string importModelJson = WebHelper.WebsiteMetadataService.Import(this.CurrentUser.Account, objectId, System.IO.File.ReadAllText(tempFilePath));
                StreamWriter jsonStreamWriter = new StreamWriter(tempFilePath);
                jsonStreamWriter.Write(importModelJson);
                jsonStreamWriter.Close();
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImportMetadatas(string tempFileName, int start, int size)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                string tempFilePath = Path.Combine(Server.MapPath("~/Temp"), tempFileName);
                List<JObject> importModels = JsonConvert.DeserializeObject<List<JObject>>(System.IO.File.ReadAllText(tempFilePath))
                    .OrderBy(x => x["importResult"]).ToList();

                resultModel.data = new DatagridModel { count = importModels.Count, list = importModels.Skip(start).Take(size) };
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
        public ActionResult Create(string objectId)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            MetadataControllerModel controller = WebHelper.GetMetadataController(coldewObject.code, MetadataActionType.Create);
            if (controller != null)
            {
                return this.RedirectToAction(controller.ActionName, controller.ControllerName, new { objectId = objectId });
            }

            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.CreateFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            this.ViewBag.Title = "创建" + coldewObject.name;
            
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
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            MetadataControllerModel controller = WebHelper.GetMetadataController(coldewObject.code, MetadataActionType.Edit);
            if (controller != null)
            {
                return this.RedirectToAction(controller.ActionName, controller.ControllerName, new { objectId = objectId, metadataId = metadataId });
            }

            this.ViewBag.metadataInfoJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.EditFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            this.ViewBag.Title = "编辑" + coldewObject.name;
            return View();
        }

        [HttpPost]
        public ActionResult EditPost(string objectId, string metadataId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                JObject model = JsonConvert.DeserializeObject<JObject>(json);
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

        [HttpGet]
        public ActionResult Details(string objectId, string metadataId)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            MetadataControllerModel controller = WebHelper.GetMetadataController(coldewObject.code, MetadataActionType.Details);
            if (controller != null)
            {
                return this.RedirectToAction(controller.ActionName, controller.ControllerName, new { objectId = objectId, metadataId = metadataId });
            }

            this.ViewBag.metadataInfoJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.DetailsFormCode);
            this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
            this.ViewBag.Title = coldewObject.name + "详细信息";
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string objectId, string metadataIdsJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                List<string> metadataIds = JsonConvert.DeserializeObject<List<string>>(metadataIdsJson);
                WebHelper.WebsiteMetadataService.Delete(objectId, WebHelper.CurrentUserAccount, metadataIds);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Metadatas(string objectId, string viewId, string searchInfoJson, int start, int size, string orderBy)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                MetadataGridModel model = WebHelper.WebsiteMetadataService.GetMetadataGridModel(objectId, viewId, WebHelper.CurrentUserAccount, searchInfoJson, start, size, orderBy);
                resultModel.data = new DatagridModel { count = model.totalCount, list = JsonConvert.DeserializeObject(model.gridJson), footer = JsonConvert.DeserializeObject(model.footersJson) };
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectDialog()
        {
            return PartialView();
        }

        public ActionResult SelectMetadatas(string objectId, string keyword, int start, int size, string orderBy)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                int totalCount = 0;
                string json = WebHelper.WebsiteMetadataService.GetGridJson(objectId, WebHelper.CurrentUserAccount, string.Format("{{keyword: \"{0}\"}}", keyword), start, size, orderBy, out totalCount);
                resultModel.data = new DatagridModel { count = totalCount, list = JsonConvert.DeserializeObject(json) };
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string objectId, string viewId, string searchInfoJson, string orderBy)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                int skipCount = 0; 
                int takeCount = 10000000;
                MetadataGridModel gridModel = WebHelper.WebsiteMetadataService.GetMetadataGridModel(objectId, viewId, WebHelper.CurrentUserAccount, 
                    searchInfoJson, skipCount, takeCount, orderBy);

                string tempPath = ImportExportHelper.Export(this.CurrentUser.Account, JsonConvert.DeserializeObject<List<JObject>>(gridModel.gridJson), objectId);
                resultModel.data = System.IO.Path.GetFileName(tempPath);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadExportFile(string objectId, string fileName)
        {
            string filePath =Path.Combine(this.Server.MapPath("~/Temp"), fileName);
            return File(filePath, "application/octet-stream", string.Format("Customer Export {0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
        }

        public ActionResult AutoCompleteList(string objectCode, string term)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, objectCode);
            int totalCount = 0;
            string json = WebHelper.WebsiteMetadataService.GetGridJson(coldewObject.id, this.CurrentUser.Account, string.Format("{{name: \"{0}\"}}", term), 0, 20, "", out totalCount);
            return Json(JsonConvert.DeserializeObject(json), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMetadataById(string objectCode, string metadataId)
        {
            ColdewObjectWebModel coldewObject = WebHelper.WebsiteColdewObjectService.GetObjectByCode(this.CurrentUser.Account, objectCode);
            string json = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, coldewObject.id, metadataId);
            if (json != null)
            {
                return Json(JsonConvert.DeserializeObject(json), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
