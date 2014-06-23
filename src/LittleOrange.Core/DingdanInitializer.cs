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
        LittleOrangeInitializer _littleOrangeInitializer;
        public DingdanInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._littleOrangeInitializer = littleOrangeInitializer;
        }

        public void Initialize()
        {
            this._littleOrangeInitializer._coldewManager.Logger.Info("init fahuo");
            ColdewObject cobject = this._littleOrangeInitializer._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("收款管理", "shoukuanGuanli", ColdewObjectType.Standard, true));
            Field fahuoDanhaoiField = cobject.CreateStringField(new StringFieldCreateInfo("fahuoDanhao", "发货单号") { IsFieldName = true });
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期") { Required = true });
            Field yewuyuanField = cobject.CreateUserField(new UserFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "客户名称") { Required = true });
            Field shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            Field shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            Field shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址"));
            Field jiekuanFangshild = cobject.CreateDropdownField(new DropdownFieldCreateInfo("jiekuanFangshi", "结款方式", new List<string> { "1个月月结", "2个月月结", "3个月月结" }) { Required = true });
            Field jiekuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("jiekuanRiqi", "结款日期"));
            Field yingshoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("yingshoukuanJine", "应收款金额") { Precision = 2 });
            Field yishoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("yishoukuanJine", "已收款金额") { Precision = 2 });
            Field weishoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("weishoukuanJine", "未收款金额") { Precision = 2 });
            Field tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2 });
            Field shifouShouwanField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("shifouShouwan", "是否收完", new List<string> { "是", "否" }));
            Field chanpinGridField = cobject.CreateJsonField(new FieldCreateInfo("chanpinGrid", "发货产品", "", false, true) { Required = true });
            Field shoukuanGridField = cobject.CreateJsonField(new FieldCreateInfo("shoukuanGrid", "收款明细", "", false, true));
            Field zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态") { Required = true });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));
            Field liuchengIdField = cobject.CreateStringField(new StringFieldCreateInfo("liuchengId", "流程ID"));

            List<Control> editControls = new List<Control>();
            editControls.Add(new Fieldset("基本信息"));
            int i = 0;
            Row row = null;
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(fahuoDanhaoiField) { Width = 6 });
            row.Children.Add(new Input(fahuoRiqiField) { Width = 6 });
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(yewuyuanField) { Width = 6 });
            row.Children.Add(new Input(kehuField) { Width = 6 });
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(jiekuanFangshild) { Width = 6 });
            row.Children.Add(new Input(jiekuanRiqiField) { Width = 6 });
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(yingshoukuanJineField) { Width = 6 });
            row.Children.Add(new Input(yishoukuanJineField) { Width = 6 });
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(weishoukuanJineField) { Width = 6 });
            row.Children.Add(new Input(tichengField) { Width = 6 });
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(beizhuField) { Width = 12 });
            foreach (Field field in cobject.GetFields())
            {
                if (i % 2 == 0)
                {
                    row = new Row();
                    editControls.Add(row);
                }
                row.Children.Add(new Input(field) { Width = 6 });
                i++;
            }
            editControls.Add(new Fieldset("产品信息"));
            Row chanpinRow = new Row();
            editControls.Add(chanpinRow);
            List<GridViewColumn> chanGridColumns = this._littleOrangeInitializer._dingdanChanpinGridFields.Select(x => new GridViewColumn(x, 80)).ToList();
            List<GridViewFooter> chanGridFooterInfoList = new List<GridViewFooter>();
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "xiaoshouDanjia", ValueType = GridViewFooterValueType.Fixed, Value = "合计" });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "zongjine", ValueType = GridViewFooterValueType.Sum });
            chanGridFooterInfoList.Add(new GridViewFooter { FieldCode = "yewufei", ValueType = GridViewFooterValueType.Sum });
            chanpinRow.Children.Add(new Grid(chanpinGridField, chanGridColumns, this._littleOrangeInitializer._fahuo_chanpin_form, this._littleOrangeInitializer._fahuo_chanpin_form) { Width = 12, Required = true, Footer = chanGridFooterInfoList });
            editControls.Add(new Fieldset("收款明细"));
            row = new Row();
            editControls.Add(row);
            row.Children.Add(new Input(shoukuanGridField) { Width = 12 });

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", editControls, null);

            List<Control> liuchengControls = new List<Control>();
            liuchengControls.Add(new Fieldset("基本信息"));
            i = 0;
            row = new Row();
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
            row.Children.Add(new Input(jiekuanRiqiField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(shouhuorenField) { Width = 6 });
            row.Children.Add(new Input(shouhuorenDianhuaField) { Width = 6 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(shouhuoDizhiField) { Width = 12 });
            row = new Row();
            liuchengControls.Add(row);
            row.Children.Add(new Input(beizhuField) { Width = 12 });
            foreach (Field field in cobject.GetFields())
            {
                if (i % 2 == 0)
                {
                    row = new Row();
                    liuchengControls.Add(row);
                }
                row.Children.Add(new Input(field) { Width = 6 });
                i++;
            }
            liuchengControls.Add(new Fieldset("产品信息"));
            liuchengControls.Add(chanpinRow);
            Form liuchengForm = cobject.FormManager.Create("fahuo_liucheng_form", "", liuchengControls, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = fahuoDanhaoiField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yingshoukuanJineField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yishoukuanJineField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = weishoukuanJineField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = tichengField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = fahuoRiqiField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = yewuyuanField.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = kehuField.ID, Width = 150 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = jiekuanFangshild.ID, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldId = jiekuanRiqiField.ID, Width = 80 });

            List<GridViewFooter> footer = new List<GridViewFooter>();
            footer.Add(new GridViewFooter { FieldCode = fahuoDanhaoiField.Code, Value = "合计", ValueType = GridViewFooterValueType.Fixed });
            footer.Add(new GridViewFooter { FieldCode = yingshoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridViewFooter { FieldCode = yishoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridViewFooter { FieldCode = weishoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            GridView shoukuanJihuanView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "未完成收款", true, true, "{shifouShouwan: '否'}", viewColumns, jiekuanRiqiField.Code, "admin") { Footer = footer });
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "所有收款", true, true, "", viewColumns, jiekuanRiqiField.Code, "admin") { Footer = footer });
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏收款", true, true, "", viewColumns, jiekuanRiqiField.Code, "admin"));

            cobject.ObjectPermission.Create(this._littleOrangeInitializer.kehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(yewuyuanField), MetadataPermissionValue.View, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.kehuAdminGroup), MetadataPermissionValue.View | MetadataPermissionValue.Modify, null);

            this._littleOrangeInitializer._fahuoLiuchengMoban = this._littleOrangeInitializer._coldewManager.LiuchengYinqing.LiuchengMobanManager.Create("FahuoLiucheng", "发货流程", cobject, "~/FahuoLiucheng", "");

            //JObject shoukuanXinxi = new JObject();
            //shoukuanXinxi.Add(fahuoDanhaoiField.Code, "201406001");
            //shoukuanXinxi.Add(fahuoRiqiField.Code, "2014-06-07");
            //shoukuanXinxi.Add(jiekuanFangshild.Code, "2个月月结");
            //shoukuanXinxi.Add(jiekuanRiqiField.Code, "2014-08-31");
            //shoukuanXinxi.Add(yewuyuanField.Code, "admin");
            //shoukuanXinxi.Add(kehuField.Code, "佛山市凯迪电器有限公司");
            //shoukuanXinxi.Add(yingshoukuanJineField.Code, 363);
            //JArray chanpins = JsonConvert.DeserializeObject<JArray>("[{'name':'绝缘漆','guige':'YJ-601B','danwei':'KG','shuliang':33,'tongshu':3,'xiaoshouDanjia':14,'xiaoshouDijia':12.7,'shijiDanjia':13.2,'zongjine':462,'yewulv':0.03,'yewulvFangshi':'按金额','yewufei':10.89,'shifouKaipiao':'是'}]");
            //shoukuanXinxi.Add(chanpinGridField.Code, chanpins);
            //cobject.MetadataManager.Create(this._admin, shoukuanXinxi);
        }
    }
}
