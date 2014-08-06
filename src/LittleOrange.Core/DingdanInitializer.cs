using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.Permission;
using Coldew.Core.Search;
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
        Field liuchengInfoGridField;
        LittleOrangeInitializer _littleOrangeInitializer;
        XiaoshouMingxiInitializer _xiaoshouMingxiInitializer;
        ShoukuanMingxiInitializer _shoukuanMingxi;
        LiuchengInitializer _liuchengInitializer;
        public DingdanInitializer(LittleOrangeInitializer littleOrangeInitializer, XiaoshouMingxiInitializer xiaoshouMingxiInitializer,
            ShoukuanMingxiInitializer shoukuanMingxi, LiuchengInitializer liuchengInitializer)
        {
            this._littleOrangeInitializer = littleOrangeInitializer;
            this._xiaoshouMingxiInitializer = xiaoshouMingxiInitializer;
            this._shoukuanMingxi = shoukuanMingxi;
            this._liuchengInitializer = liuchengInitializer;
        }

        public void Initialize()
        {
            this._littleOrangeInitializer.ColdewManager.Logger.Info("init fahuo");
            cobject = this._littleOrangeInitializer.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo { Name = "订单管理", Code = "shoukuanGuanli" });
            fahuoDanhaoiField = cobject.CreateField(new CodeFieldCreateInfo("fahuoDanhao", "发货单号", "yyyyMMSN{3}") { Required = true});
            cobject.SetNameField(fahuoDanhaoiField);
            fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期") { DefaultValueIsToday = true });
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
            chanpinGridField = cobject.CreateJsonField(new JsonFieldCreateInfo("chanpinGrid", "发货产品") { Required = true });
            shoukuanGridField = cobject.CreateJsonField(new JsonFieldCreateInfo("shoukuanGrid", "收款明细"));
            liuchengInfoGridField = cobject.CreateJsonField(new JsonFieldCreateInfo("liuchengInfoGrid", "审批信息"));
            zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态"));
            beizhuField = cobject.CreateStringField(new StringFieldCreateInfo("beizhu", "备注"));
            cobject.AddPermission(new ObjectPermission(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Create | ObjectPermissionValue.Export));
            this._littleOrangeInitializer.ColdewDataManager.ObjectDataProvider.Insert(cobject);

            MetadataPermissionStrategy permissionStrategy = cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.KehuAdminGroup), MetadataPermissionValue.All, null);
            this._littleOrangeInitializer.ColdewDataManager.MetadataStrategyPermissionDataProvider.Insert(permissionStrategy);

            this.InitEditForm();
            this.InitDetailsForm();
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
            row.Children.Add(new Input(beizhuField) { Width = 6 });
            controls.Add(new Fieldset("产品信息"));
            this.CreateEditFormChanpinGrid(controls);
            controls.Add(new Fieldset("收款明细"));
            this.CreateEditFormShoukuanGrid(controls);

            Form editForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.EditFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(editForm);
            Form createForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.CreateFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(createForm);
        }

        private void InitDetailsForm()
        {
            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("基本信息"));
            Row row = null;
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(fahuoDanhaoiField) { Width = 6, IsReadonly = true });
            row.Children.Add(new Input(fahuoRiqiField) { Width = 6, IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(yewuyuanField) { Width = 6, IsReadonly = true });
            row.Children.Add(new Input(kehuField) { Width = 6, IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(jiekuanFangshild) { Width = 6, IsReadonly = true });
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
            row.Children.Add(new Input(beizhuField) { Width = 6, IsReadonly = true });
            controls.Add(new Fieldset("产品信息"));
            GridInput chanpinGrid = this.CreateEditFormChanpinGrid(controls);
            chanpinGrid.IsReadonly = true;
            controls.Add(new Fieldset("收款明细"));
            GridInput shoukuanGrid = this.CreateEditFormShoukuanGrid(controls);
            shoukuanGrid.IsReadonly = true;

            Form detailsForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.DetailsFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(detailsForm);
        }

        private GridInput CreateEditFormChanpinGrid(List<Control> controls)
        {
            Row row = new Row();
            controls.Add(row);
            List<GridColumn> chanGridColumns = this._xiaoshouMingxiInitializer._dingdanChanpinGridFields.Select(x => new GridColumn(x) as GridColumn).ToList();
            List<GridFooter> chanGridFooterInfoList = new List<GridFooter>();
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "xiaoshouDanjia", ValueType = GridFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "zongjine", ValueType = GridFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "yewufei", ValueType = GridFooterValueType.Sum }); ;
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "shoukuanJine", ValueType = GridFooterValueType.Sum }); ;
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "ticheng", ValueType = GridFooterValueType.Sum });
            GridInput grid = new GridInput(chanpinGridField, chanGridColumns, this._xiaoshouMingxiInitializer._fahuo_chanpin_form,
                this._xiaoshouMingxiInitializer._fahuo_chanpin_form) { Width = 12, Required = true, Footer = chanGridFooterInfoList };
            row.Children.Add(grid);
            return grid;
        }

        private GridInput CreateEditFormShoukuanGrid(List<Control> controls)
        {
            Row row = new Row();
            controls.Add(row);
            List<GridColumn> gridColumns = this._shoukuanMingxi.DingdanGridFields.Select(x => new GridColumn(x) as GridColumn).ToList();
            List<GridFooter> chanGridFooterInfoList = new List<GridFooter>();
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "shoukuanRiqi", ValueType = GridFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "shoukuanJine", ValueType = GridFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "ticheng", ValueType = GridFooterValueType.Sum });
            GridInput grid = new GridInput(shoukuanGridField, gridColumns, this._shoukuanMingxi.EditForm, this._shoukuanMingxi.EditForm) { Width = 12, Required = false, Footer = chanGridFooterInfoList };
            row.Children.Add(grid);
            return grid;
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
            List<GridColumn> chanGridColumns = this._xiaoshouMingxiInitializer._liuchengChanpinGridFields.Select(x => new GridColumn(x) as GridColumn).ToList();
            List<GridFooter> chanGridFooterInfoList = new List<GridFooter>();
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "xiaoshouDanjia", ValueType = GridFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "zongjine", ValueType = GridFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridFooter { FieldCode = "yewufei", ValueType = GridFooterValueType.Sum });
            row.Children.Add(new GridInput(chanpinGridField, chanGridColumns, this._xiaoshouMingxiInitializer._fahuo_chanpin_form, this._xiaoshouMingxiInitializer._fahuo_chanpin_form) { Width = 12, Required = true, Footer = chanGridFooterInfoList });
            liuchengControls.Add(new Fieldset("流程信息"));
            row = new Row();
            liuchengControls.Add(row);
            List<GridColumn> liuchengGridColumns = new List<GridColumn>();
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.nameField));
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.chulirenField));
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.zhuangtaiMingchengField));
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.kaishiShijianField));
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.wanchengShijianField));
            liuchengGridColumns.Add(new GridColumn(this._liuchengInitializer.wanchengShuomingField));
            row.Children.Add(new GridInput(liuchengInfoGridField, liuchengGridColumns, null, null) { Width = 12, IsReadonly = true });
            Form liuchengForm = cobject.FormManager.Create(new FormCreateInfo { Code = "fahuo_liucheng_form", Title = "", Controls = liuchengControls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(liuchengForm);
        }

        private void InitView()
        {
            List<GridColumn> viewColumns = new List<GridColumn>();
            viewColumns.Add(new GridColumn(fahuoDanhaoiField));
            viewColumns.Add(new GridColumn(yingshoukuanJineField));
            viewColumns.Add(new GridColumn(yishoukuanJineField));
            viewColumns.Add(new GridColumn(weishoukuanJineField));
            viewColumns.Add(new GridColumn(tichengField));
            viewColumns.Add(new GridColumn(fahuoRiqiField));
            viewColumns.Add(new GridColumn(yewuyuanField));
            viewColumns.Add(new GridColumn(kehuField));
            viewColumns.Add(new GridColumn(jiekuanFangshild));
            viewColumns.Add(new GridColumn(jiekuanRiqiField));

            List<GridFooter> footer = new List<GridFooter>();
            footer.Add(new GridFooter { FieldCode = fahuoDanhaoiField.Code, Value = "合计", ValueType = GridFooterValueType.Fixed });
            footer.Add(new GridFooter { FieldCode = yingshoukuanJineField.Code, ValueType = GridFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = yishoukuanJineField.Code, ValueType = GridFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = weishoukuanJineField.Code, ValueType = GridFooterValueType.Sum });
            List<FilterExpression> expressions = new List<FilterExpression>();
            expressions.Add(new StringFilterExpression(shifouShouwanField, "否"));
            expressions.Add(new StringFilterExpression(zhuangtaiField, "完成"));
            MetadataFilter weiwanchengShoukuanFilter = new MetadataFilter(expressions);
            GridView shoukuanJihuanView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "未完成收款", true, true, weiwanchengShoukuanFilter, viewColumns, fahuoDanhaoiField, this._littleOrangeInitializer.Admin) { Footer = footer });
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(shoukuanJihuanView);
            expressions = new List<FilterExpression>();
            expressions.Add(new StringFilterExpression(zhuangtaiField, "审核"));
            MetadataFilter shenhezhongFilter = new MetadataFilter(expressions);
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "审核中订单", true, true, shenhezhongFilter, viewColumns, fahuoDanhaoiField, this._littleOrangeInitializer.Admin) { Footer = footer });
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(manageView);
            GridView manage1View = cobject.GridViewManager.Create(new GridViewCreateInfo("", "所有订单", true, true, null, viewColumns, fahuoDanhaoiField, this._littleOrangeInitializer.Admin) { Footer = footer });
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(manage1View);
            expressions = new List<FilterExpression>();
            expressions.Add(new FavoriteFilterExpression(this.cobject));
            MetadataFilter favoriteFilter = new MetadataFilter(expressions);
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收藏订单", true, true, favoriteFilter, viewColumns, fahuoDanhaoiField, this._littleOrangeInitializer.Admin));
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(favoriteView);

        }
    }
}
