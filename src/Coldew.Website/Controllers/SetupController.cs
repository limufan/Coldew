﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Website.Models;
using Coldew.Api;
using Newtonsoft.Json;
using Coldew.Api.UI;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Controllers
{
    public class SetupController : BaseController
    {
        //
        // GET: /Setup/

        //public ActionResult Index()
        //{
        //    EmailConfigInfo info = WebHelper.ColdewConfigService.GetEmailConfig();
        //    EmailConfigModel emailConfigModel = new EmailConfigModel(info);
        //    this.ViewBag.emailConfigModelJson = JsonConvert.SerializeObject(emailConfigModel);
        //    return View();
        //}

        //public ActionResult LeftMenu(string leftMenu)
        //{
        //    this.ViewBag.LeftMenu = leftMenu;
        //    return PartialView();
        //}

        //public ActionResult CustomerArea()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public ActionResult CreateCustomerArea()
        //{
        //    return View();
        //}
        
        //[HttpPost]
        //public ActionResult SetEmailConfig(string account, string address, string password, string server)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        WebHelper.ColdewConfigService.SetEmailConfig(account, address, password, server);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}
        
        //[HttpPost]
        //public ActionResult TestEmailConfig(string account, string address, string password, string server)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        WebHelper.ColdewConfigService.TestEmailConfig(account, address, password, server);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult CreateField()
        //{
        //    return View();
        //}

        //public ActionResult ObjectLayoutDesign(string objectId)
        //{
        //    ColdewObjectInfo objectInfo = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
        //    this.ViewBag.objectId = objectId;
        //    this.ViewBag.Title = objectInfo.Name + "扩展";
        //    this.ViewBag.objectInfo = objectInfo;

        //    ObjectLayoutDesignModel model = new ObjectLayoutDesignModel();
        //    model.fields = new List<FieldModel>();
        //    foreach (FieldInfo field in objectInfo.Fields)
        //    {
        //        model.fields.Add(new FieldModel(field));
        //    }
        //    FormWebModel formModel = WebHelper.WebsiteFormService.GetForm(this.CurrentUser.Account, objectId, FormConstCode.EditFormCode);

        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(formModel);
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult SaveObjectLayout(string objectId, string sectionJson)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
                
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Extend(string objectId)
        //{
        //    ColdewObjectInfo formInfo = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);
        //    this.ViewBag.objectId = objectId;
        //    this.ViewBag.Title = formInfo.Name + "扩展";

        //    return View();
        //}

        //public ActionResult GetFields(string objectId)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        ColdewObjectInfo coldewObject = WebHelper.ColdewObjectService.GetObjectById(this.CurrentUser.Account, objectId);

        //        resultModel.data = coldewObject.Fields.Select(x => new FieldGridModel(x, this, objectId));
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult DeleteFields(string fieldIdsJson)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        List<int> fieldIds = JsonConvert.DeserializeObject<List<int>>(fieldIdsJson);
        //        foreach (int fielId in fieldIds)
        //        {
        //            WebHelper.ColdewObjectService.DeleteField(WebHelper.CurrentUserAccount, fielId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateStringField(string objectId)
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateStringField(string objectId, string name, bool? required, string defaultValue, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.CreateStringField(objectId, new StringFieldCreateInfo("", name) { Required = required.Value, DefaultValue = defaultValue });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateDateField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateDateField(string objectId, string name, bool? required, bool defaultValueIsToday, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.CreateDateField(objectId, new DateFieldCreateInfo("", name) { Required = required.Value, DefaultValueIsToday = defaultValueIsToday });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateNumberField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateNumberField(string objectId, string name, bool? required, decimal? defaultValue, decimal? max, decimal? min, int precision, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.CreateNumberField(objectId, new NumberFieldCreateInfo("", name) { Required = required.Value, DefaultValue = defaultValue, Max = max, Min = min, Precision = precision });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditStringField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new StringFieldEditModel(field as StringFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditStringField(string objectId, int fieldId, string name, bool? required, string defaultValue, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.ModifyStringField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditDateField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new DateFieldEditModel(field as DateFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditDateField(string objectId, int fieldId, string name, bool? required, bool defaultValueIsToday, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.ModifyDateField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValueIsToday);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditNumberField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new NumberFieldEditModel(field as NumberFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditNumberField(string objectId, int fieldId, string name, bool? required, decimal? defaultValue, decimal? max, decimal? min, int precision, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.ModifyNumberField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValue, max, min, precision);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateTextField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateTextField(string objectId, string name, bool? required, string defaultValue, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.CreateTextField(objectId, new TextFieldCreateInfo("", name) { Required = required.Value, DefaultValue = defaultValue });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditTextField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new StringFieldEditModel(field as StringFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditTextField(string objectId, int fieldId, string name, bool? required, string defaultValue, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.ModifyTextField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateDropdownListField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateDropdownListField(string objectId, string name, bool? required, string selectList, string defaultValue, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }

        //        WebHelper.ColdewObjectService.CreateDropdownField(objectId, new DropdownFieldCreateInfo("", name, selectList.Split(',').ToList()) { Required = required.Value, DefaultValue = defaultValue });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditDropdownListField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new ListFieldEditModel(field as ListFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditDropdownListField(int fieldId, string name, bool? required, string selectList, string defaultValue, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        WebHelper.ColdewObjectService.ModifyDropdownField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValue, selectList.Split(',').ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateRadioboxListField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateRadioboxListField(string objectId, string name, bool? required, string selectList, string defaultValue, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }

        //        WebHelper.ColdewObjectService.CreateRadioListField(objectId, new RadioListFieldCreateInfo("", name, selectList.Split(',').ToList()) { Required = required.Value, DefaultValue = defaultValue });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditRadioboxListField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new ListFieldEditModel(field as ListFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditRadioboxListField(int fieldId, string name, bool? required, string selectList, string defaultValue, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
                
        //        WebHelper.ColdewObjectService.ModifyRadioListField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValue, selectList.Split(',').ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult CreateCheckboxListField(string objectId)
        //{
            
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateCheckboxListField(string objectId, string name, bool? required, string selectList, string defaultValue, string code)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        List<string> defaultValues = new List<string>();
        //        if (!string.IsNullOrEmpty(defaultValue))
        //        {
        //            defaultValue = defaultValue.Replace("，", ",");
        //            defaultValues = defaultValue.Split(',').ToList();
        //        }

        //        WebHelper.ColdewObjectService.CreateCheckboxListField(objectId, new CheckboxListFieldCreateInfo("", name, selectList.Split(',').ToList()) { Required = required.Value, DefaultValues = defaultValues });
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult EditCheckboxListField(int fieldId)
        //{
        //    FieldInfo field = WebHelper.ColdewObjectService.GetField(fieldId);
        //    this.ViewBag.modelJson = JsonConvert.SerializeObject(new ListFieldEditModel(field as CheckboxFieldInfo));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditCheckboxListField(int fieldId, string name, bool? required, string selectList, string defaultValue, int index)
        //{
        //    ControllerResultModel resultModel = new ControllerResultModel();
        //    try
        //    {
        //        selectList = selectList.Replace("，", ",");
        //        if (!required.HasValue)
        //        {
        //            required = false;
        //        }
        //        List<string> defaultValues = new List<string>();
        //        if (!string.IsNullOrEmpty(defaultValue))
        //        {
        //            defaultValue = defaultValue.Replace("，", ",");
        //            defaultValues = defaultValue.Split(',').ToList();
        //        }
        //        WebHelper.ColdewObjectService.ModifyCheckboxListField(fieldId, new FieldModifyBaseInfo{ Name = name, Required = required.Value}, defaultValues, selectList.Split(',').ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        resultModel.result = ControllerResult.Error;
        //        resultModel.message = ex.Message;
        //        WebHelper.Logger.Error(ex.Message, ex);
        //    }
        //    return Json(resultModel, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult CreateTextField()
        //{
        //    return View();
        //}

    }
}
