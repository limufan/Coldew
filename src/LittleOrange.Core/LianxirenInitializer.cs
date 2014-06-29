﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.UI;

namespace LittleOrange.Core
{
    public class LianxirenInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        public LittleOrangeInitializer _littleOrangeInitializer;
        Field nameField;
        Field kehuField;
        Field xingbieField;
        Field zhiweiField;
        Field shoujiField;
        Field chuanzhenField;
        Field zuojiField;
        Field qqField;
        Field emailField;
        Field createTimeField;
        Field remarkField;
        public LianxirenInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._coldewManager = littleOrangeInitializer.ColdewManager;
            this._littleOrangeInitializer = littleOrangeInitializer;
        }

        public void Initialize()
        {
            this.InitObject();
            this.InitForms();
            this.InitGridViews();
        }

        private void InitObject()
        {
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("联系人", "lianxiren", ColdewObjectType.Standard, true));
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "姓名") { Required = true });
            cobject.SetNameField(nameField);
            kehuField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("kehu", "客户", this._littleOrangeInitializer.kehuInitializer.cobject.ID) { Required = true, IsSummary = true });
            xingbieField = cobject.CreateDropdownField(new DropdownFieldCreateInfo("xingbie", "性别", new List<string> { "男", "女" }) { IsSummary = true });
            zhiweiField = cobject.CreateStringField(new StringFieldCreateInfo("zhiwei", "职位") { IsSummary = true });
            shoujiField = cobject.CreateStringField(new StringFieldCreateInfo("shouji", "手机") { IsSummary = true });
            chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            zuojiField = cobject.CreateStringField(new StringFieldCreateInfo("zuoji", "座机") { IsSummary = true });
            qqField = cobject.CreateStringField(new StringFieldCreateInfo("qq", "QQ"));
            emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮件地址"));
            createTimeField = cobject.CreateDateField(new DateFieldCreateInfo("createTime", "创建日期") { Required = true, DefaultValueIsToday = true });
            remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));
            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._coldewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }

        protected void InitForms()
        {
            List<Control> controls = this.CreateControls(false);
            Form editForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);
            Form createForm = cobject.FormManager.Create(FormConstCode.CreateFormCode, "", controls, null);
            controls = this.CreateControls(true);
            Form detailsForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", controls, null);
        }

        protected List<Control> CreateControls(bool isReadonly)
        {
            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("基本信息"));
            int i = 0;
            Row row = null;
            foreach (Field field in cobject.GetFields())
            {
                if (i % 2 == 0)
                {
                    row = new Row();
                    controls.Add(row);
                }
                row.Children.Add(new Input(field) { IsReadonly = isReadonly });
                i++;
            }
            return controls;
        }

        private void InitGridViews()
        {
            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "联系人管理", true, true, "", viewColumns, createTimeField.ID, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏联系人", true, true, "", viewColumns, createTimeField.ID, "admin"));

        }
    }
}