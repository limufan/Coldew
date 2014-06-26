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
    public class ShoukuanMingxiInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        LittleOrangeInitializer _littleOrangeInitializer;
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
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("收款明细", "shoukuanMingxi", ColdewObjectType.Standard, true));
            Field chuhuoDanhaoField = cobject.CreateStringField(new StringFieldCreateInfo("fahuoDanhao", "发货单号") { Required = true });
            Field yewuyuanField = cobject.CreateUserField(new UserFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "日期") { Required = true });
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "发货客户") { Required = true, GridWidth = 120 });
            cobject.SetNameField(chuhuoDanhaoField);
            Field shoukuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("shoukuanRiqi", "收款日期") { DefaultValueIsToday = true, Required = true });
            Field shoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("shoukuanJine", "收款金额") { Precision = 2, Required = true });
            Field tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2});
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));


            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("基本信息"));
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

            this.EditForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);

            this.DingdanGridFields.Add(shoukuanRiqiField);
            this.DingdanGridFields.Add(shoukuanJineField);
            this.DingdanGridFields.Add(tichengField);
            this.DingdanGridFields.Add(beizhuField);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "收款管理", true, true, "", viewColumns, shoukuanRiqiField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏收款", true, true, "", viewColumns, shoukuanRiqiField.Code, "admin"));

            cobject.ObjectPermission.Create(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._littleOrangeInitializer.KehuAdminGroup), MetadataPermissionValue.View, null);
        }
    }
}
