using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.UI;

namespace LittleOrange.Core
{
    public class DingdanInitializer
    {
        public ColdewObject cobject;
        Field fahuoDanhaoiField;
        Field fahuoRiqiField;
        Field yewuyuanField;
        Field kehuField;
        Field shouhuorenField;
        Field shouhuorenDianhuaField;
        Field shouhuoDizhiField;
        Field jiekuanFangshild;
        Field jiekuanRiqiField;
        Field yingshoukuanJineField;
        Field yishoukuanJineField;
        Field weishoukuanJineField;
        Field tichengField;
        Field shifouShouwanField;
        Field chanpinGridField;
        Field shoukuanGridField;
        Field zhuangtaiField;
        Field beizhuField;
        Field liuchengIdField;
        LittleOrangeInitializer _littleOrangeInitializer;
        XiaoshouMingxiInitializer _xiaoshouMingxiInitializer;
        ShoukuanMingxiInitializer _shoukuanMingxi;
        public DingdanInitializer(LittleOrangeInitializer littleOrangeInitializer, XiaoshouMingxiInitializer xiaoshouMingxiInitializer, ShoukuanMingxiInitializer shoukuanMingxi)
        {
            this._littleOrangeInitializer = littleOrangeInitializer;
            this._xiaoshouMingxiInitializer = xiaoshouMingxiInitializer;
            this._shoukuanMingxi = shoukuanMingxi;
        }

        public void Initialize()
        {
            this._littleOrangeInitializer.ColdewManager.Logger.Info("init fahuo");
            cobject = this._littleOrangeInitializer.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("收款管理", "shoukuanGuanli", ColdewObjectType.Standard, true));
            fahuoDanhaoiField = cobject.CreateField(new CodeFieldCreateInfo("fahuoDanhao", "发货单号", "yyyyMMSN{3}") { Required = true});
            cobject.SetNameField(fahuoDanhaoiField);
            fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "日期") { DefaultValueIsToday = true });
            yewuyuanField = cobject.CreateUserField(new UserFieldCreateInfo("yewuyuan", "业务员") { Required = true, DefaultValueIsCurrent = true });
            kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "客户名称") { Required = true, GridWidth = 120 });
            shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            shouhuoDizhiField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoDizhi", "收货地址"));
            jiekuanFangshild = cobject.CreateDropdownField(new DropdownFieldCreateInfo("jiekuanFangshi", "结款方式", new List<string> { "1个月月结", "2个月月结", "3个月月结" }) { Required = true });
            jiekuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("jiekuanRiqi", "结款日期"));
            yingshoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("yingshoukuanJine", "应收款金额") { Precision = 2 });
            yishoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("yishoukuanJine", "已收款金额") { Precision = 2 });
            weishoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("weishoukuanJine", "未收款金额") { Precision = 2 });
            tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2 });
            shifouShouwanField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("shifouShouwan", "是否收完", new List<string> { "是", "否" }));
            chanpinGridField = cobject.CreateJsonField(new FieldCreateInfo("chanpinGrid", "发货产品", "", false, true) { Required = true });
            shoukuanGridField = cobject.CreateJsonField(new FieldCreateInfo("shoukuanGrid", "收款明细", "", false, true));
            zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态"));
            beizhuField = cobject.CreateStringField(new StringFieldCreateInfo("beizhu", "备注"));
            liuchengIdField = cobject.CreateStringField(new StringFieldCreateInfo("liuchengId", "流程ID"));

            this.InitEditForm();
            this.InitLiuchengForm();
            this.InitView();
        }

        private void InitEditForm()
        {
            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("基本信息"));
            Row row = null;
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(fahuoDanhaoiField) { Width = 6 });
            row.Children.Add(new Input(fahuoRiqiField) { Width = 6 });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(yewuyuanField) { Width = 6 });
            row.Children.Add(new Input(kehuField) { Width = 6 });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(jiekuanFangshild) { Width = 6 });
            row.Children.Add(new Input(jiekuanRiqiField) { Width = 6, IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(yingshoukuanJineField) { Width = 6, IsReadonly = true });
            row.Children.Add(new Input(yishoukuanJineField) { Width = 6, IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(weishoukuanJineField) { Width = 6, IsReadonly = true });
            row.Children.Add(new Input(tichengField) { Width = 6, IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(beizhuField) { Width = 6 });
            controls.Add(new Fieldset("产品信息"));
            this.CreateEditFormChanpinGrid(controls);
            controls.Add(new Fieldset("收款明细"));
            this.CreateEditFormShoukuanGrid(controls);

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", controls, null);
        }

        private void CreateEditFormChanpinGrid(List<Control> controls)
        {
            Row row = new Row();
            controls.Add(row);
            List<GridViewColumn> chanGridColumns = this._xiaoshouMingxiInitializer._dingdanChanpinGridFields.Select(x => new GridViewColumn(x)).ToList();
            List<GridViewFooter> chanGridFooterInfoList = new List<GridViewFooter>();
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "xiaoshouDanjia", ValueType = GridViewFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "zongjine", ValueType = GridViewFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "yewufei", ValueType = GridViewFooterValueType.Sum }); ;
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "shoukuanJine", ValueType = GridViewFooterValueType.Sum }); ;
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "ticheng", ValueType = GridViewFooterValueType.Sum });
            row.Children.Add(new Grid(chanpinGridField, chanGridColumns, this._xiaoshouMingxiInitializer._fahuo_chanpin_form, 
                this._xiaoshouMingxiInitializer._fahuo_chanpin_form) { Width = 12, Required = true, Footer = chanGridFooterInfoList });
        }

        private void CreateEditFormShoukuanGrid(List<Control> controls)
        {
            Row row = new Row();
            controls.Add(row);
            List<GridViewColumn> gridColumns = this._shoukuanMingxi.DingdanGridFields.Select(x => new GridViewColumn(x)).ToList();
            List<GridViewFooter> chanGridFooterInfoList = new List<GridViewFooter>();
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "shoukuanRiqi", ValueType = GridViewFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "shoukuanJine", ValueType = GridViewFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "ticheng", ValueType = GridViewFooterValueType.Sum });
            row.Children.Add(new Grid(shoukuanGridField, gridColumns, this._shoukuanMingxi.EditForm, this._shoukuanMingxi.EditForm) 
            { Width = 12, Required = true, Footer = chanGridFooterInfoList });
        }

        private void InitLiuchengForm()
        {
            List<Control> liuchengControls = new List<Control>();
            liuchengControls.Add(new Fieldset("基本信息"));
            int i = 0;
            Row row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(fahuoDanhaoiField) { Width = 6 });
            row.Children.Add(new Input(fahuoRiqiField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(yewuyuanField) { Width = 6 });
            row.Children.Add(new Input(kehuField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(jiekuanFangshild) { Width = 6 });
            row.Children.Add(new Input(shouhuorenField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(shouhuorenDianhuaField) { Width = 6 });
            row.Children.Add(new Input(shouhuoDizhiField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(beizhuField) { Width = 6 });
            liuchengControls.Add(new Fieldset("产品信息"));
            row = new Row();
            liuchengControls.Add(row);
            List<GridViewColumn> chanGridColumns = this._xiaoshouMingxiInitializer._liuchengChanpinGridFields.Select(x => new GridViewColumn(x)).ToList();
            List<GridViewFooter> chanGridFooterInfoList = new List<GridViewFooter>();
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "xiaoshouDanjia", ValueType = GridViewFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "zongjine", ValueType = GridViewFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "yewufei", ValueType = GridViewFooterValueType.Sum });
            row.Children.Add(new Grid(chanpinGridField, chanGridColumns, this._xiaoshouMingxiInitializer._fahuo_chanpin_form, this._xiaoshouMingxiInitializer._fahuo_chanpin_form) { Width = 12, Required = true, Footer = chanGridFooterInfoList });
            Form liuchengForm = cobject.FormManager.Create("fahuo_liucheng_form", "", liuchengControls, null);
        }

        private void InitView()
        {
            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = fahuoDanhaoiField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yingshoukuanJineField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yishoukuanJineField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = weishoukuanJineField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = tichengField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = fahuoRiqiField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yewuyuanField.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = kehuField.ID});
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = jiekuanFangshild.ID });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = jiekuanRiqiField.ID });

            List<GridViewFooter> footer = new List<GridViewFooter>();
            footer.Add(new GridViewFooter { FieldCode = fahuoDanhaoiField.Code, Value = "合计", ValueType = GridViewFooterValueType.Fixed });
            footer.Add(new GridViewFooter { FieldCode = yingshoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridViewFooter { FieldCode = yishoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridViewFooter { FieldCode = weishoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            GridView shoukuanJihuanView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "未完成收款", true, true, "{shifouShouwan: '否', zhuangtai: '完成'}", viewColumns, jiekuanRiqiField.Code, "admin") { Footer = footer });
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "审核中订单", true, true, "{zhuangtai: '审核'}", viewColumns, jiekuanRiqiField.Code, "admin") { Footer = footer });
            GridView manage1View = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "所有收款", true, true, "", viewColumns, jiekuanRiqiField.Code, "admin") { Footer = footer });
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏收款", true, true, "", viewColumns, jiekuanRiqiField.Code, "admin"));

            cobject.ObjectPermission.Create(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(yewuyuanField), MetadataPermissionValue.View, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.KehuAdminGroup), MetadataPermissionValue.View | MetadataPermissionValue.Modify, null);
        }
    }
}
