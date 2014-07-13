using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.Search;
using Coldew.Core.UI;

namespace LittleOrange.Core
{
    public class XiaoshouMingxiInitializer
    {
        public LittleOrangeManager _coldewManager;
        LittleOrangeInitializer _littleOrangeInitializer;
        public List<Field> _liuchengChanpinGridFields;
        public List<Field> _dingdanChanpinGridFields;
        public Form _fahuo_chanpin_form;
        ColdewObject cobject;
        Field chuhuoDanhaoField;
        Field nameField;
        Field yewuyuanField;
        Field fahuoRiqiField;
        Field kehuField;
        Field guigeField;
        Field danweiField;
        Field shuliangField;
        Field tongshuField;
        Field xiaoshouDanjiaField;
        Field shijiDanjiaField;
        Field xiaoshouDijiaField;
        Field zongjineField;
        Field yewulvField;
        Field yewulvFangshiField;
        Field yewufeiField;
        Field tichengField;
        Field shifouKaipiaoField;
        Field butieField;
        Field shoukuanJineField;
        Field yunfeiField;
        Field lirunField;
        Field beizhuField;
        public XiaoshouMingxiInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._littleOrangeInitializer = littleOrangeInitializer;
            this._coldewManager = this._littleOrangeInitializer.ColdewManager;
        }

        public void Initialize()
        {
            this._coldewManager.Logger.Info("init fahuo");
            this.InitObject();
            this.InitDetailsForm();
            this.InitChanpinGrid();
            this.InitGridViews();
        }

        private void InitObject()
        {
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("销售明细", "xiaoshouMingxi", true));
            chuhuoDanhaoField = cobject.CreateStringField(new StringFieldCreateInfo("fahuoDanhao", "发货单号") { Required = true });
            cobject.SetNameField(chuhuoDanhaoField);
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "产品名称") { Required = true });
            yewuyuanField = cobject.CreateUserField(new UserFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期") { Required = true });
            kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "发货客户") { Required = true, GridWidth = 120 });
            guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格") { Required = true });
            danweiField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位") { Required = true, GridWidth = 60 });
            shuliangField = cobject.CreateNumberField(new NumberFieldCreateInfo("shuliang", "数量") { Required = true, GridWidth = 60 });
            tongshuField = cobject.CreateNumberField(new NumberFieldCreateInfo("tongshu", "桶数") { Required = true, GridWidth = 60 });
            xiaoshouDanjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDanjia", "销售单价") { Precision = 2, Required = true });
            shijiDanjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("shijiDanjia", "实际单价") { Precision = 2, Required = true });
            xiaoshouDijiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDijia", "销售底价") { Precision = 2, Required = true });
            zongjineField = cobject.CreateNumberField(new NumberFieldCreateInfo("zongjine", "金额") { Precision = 2, Required = true });
            yewulvField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewulv", "业务率") { Precision = 2, Required = true });
            yewulvFangshiField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("yewulvFangshi", "业务率方式", new List<string> { "按金额", "按重量" }) { IsSummary = true });
            yewufeiField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewufei", "业务费") { Precision = 2, Required = true });
            tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2 });
            shifouKaipiaoField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("shifouKaipiao", "是否开票", new List<string>() { "是", "否" }) { Required = true });
            butieField = cobject.CreateNumberField(new NumberFieldCreateInfo("butie", "补贴") { Precision = 2, Required = true });
            shoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("shoukuanJine", "收款金额") { Precision = 2 });
            yunfeiField = cobject.CreateNumberField(new NumberFieldCreateInfo("yunfei", "运费") { Precision = 2 });
            lirunField = cobject.CreateNumberField(new NumberFieldCreateInfo("lirun", "利润") { Precision = 2 });
            beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            cobject.ObjectPermission.Create(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.KehuAdminGroup), MetadataPermissionValue.View, null);
        }

        private void InitDetailsForm()
        {
            _liuchengChanpinGridFields = new List<Field>();
            _liuchengChanpinGridFields.Add(nameField);
            _liuchengChanpinGridFields.Add(guigeField);
            _liuchengChanpinGridFields.Add(danweiField);
            _liuchengChanpinGridFields.Add(shuliangField);
            _liuchengChanpinGridFields.Add(tongshuField);
            _liuchengChanpinGridFields.Add(xiaoshouDijiaField);
            _liuchengChanpinGridFields.Add(xiaoshouDanjiaField);
            _liuchengChanpinGridFields.Add(shijiDanjiaField);
            _liuchengChanpinGridFields.Add(zongjineField);
            _liuchengChanpinGridFields.Add(yewulvField);
            _liuchengChanpinGridFields.Add(yewufeiField);
            _liuchengChanpinGridFields.Add(shifouKaipiaoField);
            _dingdanChanpinGridFields = new List<Field>();
            _dingdanChanpinGridFields.AddRange(_liuchengChanpinGridFields);
            _dingdanChanpinGridFields.Add(shoukuanJineField);
            _dingdanChanpinGridFields.Add(tichengField);

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
                row.Children.Add(new Input(field));
                i++;
            }

            cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", controls, null);
        }

        private void InitChanpinGrid()
        {
            List<Control> fahuo_chanpin_controls = new List<Control>();
            Row row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(nameField));
            row.Children.Add(new Input(guigeField));
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(danweiField));
            row.Children.Add(new Input(shuliangField));
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(tongshuField));
            row.Children.Add(new Input(xiaoshouDanjiaField));
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(shijiDanjiaField) { IsReadonly = true });
            row.Children.Add(new Input(xiaoshouDijiaField) { IsReadonly = true });
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(zongjineField) { IsReadonly = true });
            row.Children.Add(new Input(butieField) { IsReadonly = true });
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(yewulvField));
            row.Children.Add(new Input(yewulvFangshiField));
            row = new Row();
            fahuo_chanpin_controls.Add(row);
            row.Children.Add(new Input(yewufeiField));
            row.Children.Add(new Input(shifouKaipiaoField));
            _fahuo_chanpin_form = cobject.FormManager.Create("fahuo_chanpin_form", "产品信息", fahuo_chanpin_controls, null);
        }

        private void InitGridViews()
        {
            List<GridViewColumn> viewColumns = new List<GridViewColumn>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumn(field));
            }
            List<GridFooter> footer = new List<GridFooter>();
            footer.Add(new GridFooter { FieldCode = chuhuoDanhaoField.Code, Value = "合计", ValueType = GridViewFooterValueType.Fixed });
            footer.Add(new GridFooter { FieldCode = zongjineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = yewufeiField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = tichengField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = butieField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = shoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = yunfeiField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridFooter { FieldCode = lirunField.Code, ValueType = GridViewFooterValueType.Sum });

            List<FilterExpression> expressions = new List<FilterExpression>();
            expressions.Add(new FavoriteFilterExpression(this.cobject));
            MetadataFilter filter = new MetadataFilter(expressions);
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "发货管理", true, true, null, viewColumns, chuhuoDanhaoField ,this._littleOrangeInitializer.Admin) { Footer = footer });
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收藏发货", true, true, filter, viewColumns, chuhuoDanhaoField, this._littleOrangeInitializer.Admin));
        }
    }
}
