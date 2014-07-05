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
    public class ShoukuanMingxiInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        LittleOrangeInitializer _littleOrangeInitializer;
        Field chuhuoDanhaoField ;
        Field yewuyuanField ;
        Field fahuoRiqiField;
        Field kehuField ;
        Field shoukuanRiqiField;
        Field shoukuanJineField;
        Field tichengField;
        Field beizhuField;
        public ShoukuanMingxiInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._coldewManager = littleOrangeInitializer.ColdewManager;
            this._littleOrangeInitializer = littleOrangeInitializer;
            this.DingdanGridFields = new List<Field>();
        }

        public List<Field> DingdanGridFields{ private set;get; }

        public Form EditForm { private set; get; }

        public void Initialize()
        {
            this._coldewManager.Logger.Info("init fahuo");

            this.InitObject();
            this.InitDingdanShoukuanGrid();
            this.InitGridViews();
            this.InitDetailsForm();
        }

        private void InitObject()
        {
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("收款明细", "shoukuanMingxi", true));
            chuhuoDanhaoField = cobject.CreateStringField(new StringFieldCreateInfo("fahuoDanhao", "发货单号") { Required = true });
            yewuyuanField = cobject.CreateUserField(new UserFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期") { Required = true });
            kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "发货客户") { Required = true, GridWidth = 120 });
            cobject.SetNameField(chuhuoDanhaoField);
            shoukuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("shoukuanRiqi", "收款日期") { DefaultValueIsToday = true, Required = true });
            shoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("shoukuanJine", "收款金额") { Precision = 2, Required = true });
            tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2 });
            beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            cobject.ObjectPermission.Create(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.KehuAdminGroup), MetadataPermissionValue.View, null);
        }

        private void InitDingdanShoukuanGrid()
        {
            List<Control> controls = new List<Control>();
            Row row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(shoukuanRiqiField));
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(shoukuanJineField));
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(tichengField) { IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(beizhuField));

            this.EditForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "收款信息", controls, null);
            cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", controls, null);

            this.DingdanGridFields.Add(shoukuanRiqiField);
            this.DingdanGridFields.Add(shoukuanJineField);
            this.DingdanGridFields.Add(tichengField);
            this.DingdanGridFields.Add(beizhuField);
        }

        private void InitGridViews()
        {

            List<GridViewColumn> viewColumns = new List<GridViewColumn>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumn(field));
            }
            List<GridViewFooter> footer = new List<GridViewFooter>();
            footer.Add(new GridViewFooter { FieldCode = chuhuoDanhaoField.Code, Value = "合计", ValueType = GridViewFooterValueType.Fixed });
            footer.Add(new GridViewFooter { FieldCode = shoukuanJineField.Code, ValueType = GridViewFooterValueType.Sum });
            footer.Add(new GridViewFooter { FieldCode = tichengField.Code, ValueType = GridViewFooterValueType.Sum });
            List<FilterExpression> expressions = new List<FilterExpression>();
            expressions.Add(new FavoriteFilterExpression(this.cobject));
            MetadataFilter filter = new MetadataFilter(expressions);
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收款管理", true, true, null, viewColumns, shoukuanRiqiField, this._littleOrangeInitializer.Admin) { Footer = footer });
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收藏收款", true, true, filter, viewColumns, shoukuanRiqiField, this._littleOrangeInitializer.Admin));
        }

        public void InitDetailsForm()
        {
            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("基本信息"));
            Row row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(chuhuoDanhaoField) { IsReadonly = true });
            row.Children.Add(new Input(fahuoRiqiField) { IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(yewuyuanField) { IsReadonly = true });
            row.Children.Add(new Input(kehuField) { IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(shoukuanRiqiField) { IsReadonly = true });
            row.Children.Add(new Input(shoukuanJineField) { IsReadonly = true });
            row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(tichengField) { IsReadonly = true });
            row.Children.Add(new Input(beizhuField) { IsReadonly = true });

            cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", controls, null);
        }
    }
}
