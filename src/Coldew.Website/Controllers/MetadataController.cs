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
            if (string.IsNullOrEmpty(viewId))
            {
                List<GridViewInfo> views = WebHelper.GridViewService.GetGridViews(objectId, this.CurrentUser.Account);
                return this.RedirectToAction("Index", new { objectId = objectId, viewId = views[0].ID });
            }
            
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            GridViewInfo viewInfo = WebHelper.GridViewService.GetGridView(viewId);

            List<DataGridColumnModel> columns = viewInfo.Columns.Select(x => new DataGridColumnModel(x)).ToList();
            this.ViewBag.columnsJson = JsonConvert.SerializeObject(columns);
            this.ViewBag.viewId = viewInfo.ID;
            this.ViewBag.Title = viewInfo.Name;
            this.ViewBag.canSettingView = viewInfo.Creator.Account == this.CurrentUser.Account;

            ColdewObjectWebModel coldewObjectMoel = WebHelper.WebsiteColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.fieldsJson = JsonConvert.SerializeObject(coldewObjectMoel.fields);
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
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;

            this.ViewBag.Title = "导入" + coldewObject.Name;
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
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;

            this.ViewBag.Title = "导入" + coldewObject.Name;
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
                List<JObject> importModels = JsonConvert.DeserializeObject<List<JObject>>(System.IO.File.ReadAllText(tempFilePath));
                ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
                foreach (JObject model in importModels)
                {
                    JObject propertysObject = new JObject();
                    List<FieldInfo> fieldInfos = coldewObject.Fields.ToList();
                    foreach (FieldInfo filed in fieldInfos)
                    {
                        if (model[filed.Code] != null)
                        {
                            if (filed.Type == FieldType.Metadata)
                            {
                                MetadataInfo metadata = WebHelper.WebsiteMetadataService.GetMetadataByName(this.CurrentUser.Account, objectId, model[filed.Code].ToString());
                                propertysObject.Add(filed.Code, metadata.ID);
                            }
                            else
                            {
                                propertysObject.Add(filed.Code, model[filed.Code]);
                            }
                        }
                    }
                    try
                    {
                        WebHelper.WebsiteMetadataService.Create(objectId, WebHelper.CurrentUserAccount, JsonConvert.SerializeObject(propertysObject));

                        model["importResult"] = true;
                        model["importMessage"] = "导入成功";
                    }
                    catch(Exception ex)
                    {
                        WebHelper.Logger.Error(ex.Message, ex);
                        model["importResult"] = false;
                        model["importMessage"] = "导入失败" + ex.Message;
                    }
                }

                StreamWriter jsonStreamWriter = new StreamWriter(tempFilePath);
                jsonStreamWriter.Write(JsonConvert.SerializeObject(importModels));
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
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            this.ViewBag.Title = coldewObject.Name + " - 详细信息";

            this.ViewBag.metadataInfoJson = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectId, metadataId);
            if (coldewObject.Type == ColdewObjectType.Workflow)
            {
                return this.RedirectToAction("Details", coldewObject.Code, new { objectId = objectId, metadataId = metadataId });
            }
            else
            {
                FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.DetailsFormCode);
                this.ViewBag.formModelJson = JsonConvert.SerializeObject(formModel);
                return View();
            }
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
                int totalCount;
                string json = null;
                if (string.IsNullOrEmpty(searchInfoJson))
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJson(objectId, viewId, WebHelper.CurrentUserAccount, start, size, orderBy, out totalCount);
                }
                else
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJsonBySerach(objectId, viewId, WebHelper.CurrentUserAccount, searchInfoJson, start, size, orderBy, out  totalCount);
                }
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
                string json = null;
                if (string.IsNullOrEmpty(keyword))
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJson(objectId, WebHelper.CurrentUserAccount, start, size, orderBy, out totalCount);
                }
                else
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJsonBySerach(objectId, WebHelper.CurrentUserAccount, string.Format("{{keyword: \"{0}\"}}", keyword), start, size, orderBy, out totalCount);
                }

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
                string json = null;
                if (string.IsNullOrEmpty(searchInfoJson))
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJson(objectId, viewId, WebHelper.CurrentUserAccount, orderBy);
                }
                else
                {
                    json = WebHelper.WebsiteMetadataService.GetGridJsonBySerach(objectId, viewId, WebHelper.CurrentUserAccount, searchInfoJson, orderBy);
                }
                string tempPath = ImportExportHelper.Export(this.CurrentUser.Account, JsonConvert.DeserializeObject<List<JObject>>(json), objectId);
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

        public ActionResult GridViewManage(string objectId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;
            return View();
        }

        public ActionResult GetGridViews(string objectId)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                List<GridViewInfo> views = WebHelper.GridViewService.GetMyGridViews(objectId, WebHelper.CurrentUserAccount);
                var models = views.Select(x => new GridViewGridModel(x, this, objectId));
                resultModel.data = models;
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateGridView(string objectId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;

            List<ViewSetupFieldModel> fields = new List<ViewSetupFieldModel>();
            fields.AddRange(coldewObject.Fields.Select(x => new ViewSetupFieldModel(x, false, 80)));
            this.ViewBag.fields = fields;
            return View();
        }

        [HttpPost]
        public ActionResult CreateGridView(string objectId, string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                GridViewCreateModel model = JsonConvert.DeserializeObject<GridViewCreateModel>(json);
                List<GridViewColumnSetupInfo> columns = model.columns.Select(x => new GridViewColumnSetupInfo(x.fieldCode, x.width)).ToList();
                string searchJson = JsonConvert.SerializeObject(model.search);
                WebHelper.GridViewService.Create(model.name, objectId, WebHelper.CurrentUserAccount, model.isShared, searchJson, columns, model.OrderBy);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGridView(string objectId, string viewId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;

            List<ViewSetupFieldModel> fields = new List<ViewSetupFieldModel>();
            fields.AddRange(coldewObject.Fields.Select(x => new ViewSetupFieldModel(x, false, 80)));
            GridViewInfo viewInfo = WebHelper.GridViewService.GetGridView(viewId);
            foreach (GridViewColumnInfo column in viewInfo.Columns)
            {
                ViewSetupFieldModel model = fields.Find(x => x.code == column.Code);
                if (model != null)
                {
                    model.selected = true;
                    model.width = column.Width;
                }
            }
            this.ViewBag.fields = fields;
            this.ViewBag.viewInfoJson = JsonConvert.SerializeObject(new GridViewEditModel(viewInfo));
            return View();
        }

        [HttpPost]
        public ActionResult EditGridView(string json)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                GridViewEditPostModel model = JsonConvert.DeserializeObject<GridViewEditPostModel>(json);
                List<GridViewColumnSetupInfo> columns = model.columns.Select(x => new GridViewColumnSetupInfo(x.fieldCode, x.width)).ToList();
                string searchJson = JsonConvert.SerializeObject(model.search);
                WebHelper.GridViewService.Modify(model.id, model.name, model.isShared, searchJson, columns);
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
        public ActionResult ViewSetup(string viewId, string objectId)
        {
            ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
            this.ViewBag.coldewObject = coldewObject;
            this.ViewBag.objectPermValue = coldewObject.PermissionValue;

            this.ViewBag.viewId = viewId;
            this.ViewBag.objectId = objectId;

            List<ViewSetupFieldModel> fields = new List<ViewSetupFieldModel>();
            fields.AddRange(coldewObject.Fields.Select(x => new ViewSetupFieldModel(x, false, 80)));

            GridViewInfo viewInfo = WebHelper.GridViewService.GetGridView(viewId);
            foreach (GridViewColumnInfo column in viewInfo.Columns)
            {
                ViewSetupFieldModel model = fields.Find(x => x.code == column.Code);
                if (model != null)
                {
                    model.selected = true;
                    model.width = column.Width;
                }
            }

            this.ViewBag.fields = fields;
            return this.PartialView();
        }

        [HttpPost]
        public ActionResult SetViewSetup(string viewId, string columnsJson)
        {
            ControllerResultModel resultModel = new ControllerResultModel();
            try
            {
                List<GridViweColumnSetupModel> columnModels = JsonConvert.DeserializeObject<List<GridViweColumnSetupModel>>(columnsJson);
                List<GridViewColumnSetupInfo> columns = columnModels.Select(x => new GridViewColumnSetupInfo(x.fieldCode, x.width)).ToList();
                WebHelper.GridViewService.Modify(viewId, columns);
            }
            catch (Exception ex)
            {
                resultModel.result = ControllerResult.Error;
                resultModel.message = ex.Message;
                WebHelper.Logger.Error(ex.Message, ex);
            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteList(string objectCode, string term)
        {
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, objectCode);
            int totalCount = 0;
            string json = WebHelper.WebsiteMetadataService.GetGridJsonBySerach(objectInfo.ID, this.CurrentUser.Account, string.Format("{{name: \"{0}\"}}", term), 0, 20, "", out totalCount);
            return Json(JsonConvert.DeserializeObject(json), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMetadataById(string objectCode, string metadataId)
        {
            ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectByCode(this.CurrentUser.Account, objectCode);
            string json = WebHelper.WebsiteMetadataService.GetEditJson(this.CurrentUser.Account, objectInfo.ID, metadataId);
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
