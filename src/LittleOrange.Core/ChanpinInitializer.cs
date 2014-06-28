using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.UI;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class ChanpinInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        public LittleOrangeInitializer _littleOrangeInitializer;
        Field nameField;
        Field guigeField;
        Field danweiField;
        Field xiaoshouDijiaField;
        Field jinhuojiaField;
        Field beizhuField;
        public ChanpinInitializer(LittleOrangeInitializer littleOrangeInitializer)
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
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("产品", "chanpin", ColdewObjectType.Standard, true));
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "名称") { Required = true });
            cobject.SetNameField(nameField);
            guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格") { IsSummary = true });
            danweiField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位"));
            xiaoshouDijiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDijia", "销售底价") { Precision = 2 });
            jinhuojiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("jinhuojia", "进货价") { Precision = 2 });
            beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));
            cobject.ObjectPermission.Create(this._littleOrangeInitializer.KehuAdminGroup, ObjectPermissionValue.All);
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

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "产品管理", true, true, "", viewColumns, nameField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏产品", true, true, "", viewColumns, nameField.Code, "admin"));
        }

        public void CreateTestData()
        {
            JObject chanpinXinxi = new JObject();
            chanpinXinxi.Add(nameField.Code, "绝缘漆");
            chanpinXinxi.Add(guigeField.Code, "YJ-601B");
            chanpinXinxi.Add(danweiField.Code, "KG");
            chanpinXinxi.Add(xiaoshouDijiaField.Code, "12.7");
            chanpinXinxi.Add(jinhuojiaField.Code, "11");
            cobject.MetadataManager.Create(this._littleOrangeInitializer.Admin, chanpinXinxi);
        }
    }
}
