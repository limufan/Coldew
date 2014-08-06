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
    public class LianxiJiluInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        public LittleOrangeInitializer _littleOrangeInitializer;
        Field nameField;
        Field lianxirenField;
        Field kehuField;
        Field wayField;
        Field lianxiRiqiField;
        Field xiaChiLianxiRiqiField;
        Field remarkField;
        public LianxiJiluInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._coldewManager = littleOrangeInitializer.ColdewManager;
            this._littleOrangeInitializer = littleOrangeInitializer;
        }

        public void Initialize()
        {
            this.InitObject();
            this.InitForms();
            this.InitGridViews();
            this.CreateKehuLianxirenGrid();
        }

        private void InitObject()
        {
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo { Name = "联系记录", Code = "lianxiJilu" });
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "主题") { Required = true });
            cobject.SetNameField(nameField);
            lianxirenField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("lianxiren", "联系人", this._littleOrangeInitializer.lianxirenInitializer.cobject) { IsSummary = true });
            kehuField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("kehu", "客户", this._littleOrangeInitializer.kehuInitializer.cobject) { IsSummary = true });
            wayField = cobject.CreateStringField(new StringFieldCreateInfo("fangshi", "联系方式") { IsSummary = true });
            lianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("lianxiRiqi", "联系日期"));
            xiaChiLianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("xiaChiLianxiRiqi", "下次联系日期"));
            remarkField = cobject.CreateTextField(new TextFieldCreateInfo("neirong", "联系内容"));
            cobject.AddPermission(new ObjectPermission(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.All));
            this._littleOrangeInitializer.ColdewDataManager.ObjectDataProvider.Insert(cobject);

            MetadataPermissionStrategy permissionStrategy = cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._coldewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
            this._littleOrangeInitializer.ColdewDataManager.MetadataStrategyPermissionDataProvider.Insert(permissionStrategy);
        }

        protected void InitForms()
        {
            List<Control> controls = this.CreateControls(false);
            Form editForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.EditFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(editForm);
            Form createForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.CreateFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(createForm);
            controls = this.CreateControls(true);
            Form detailsForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.DetailsFormCode, Title = "", Controls = controls });
            this._littleOrangeInitializer.ColdewDataManager.FormDataProvider.Insert(detailsForm);
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

        private void CreateKehuLianxirenGrid()
        {
            ColdewObjectGrid grid = new ColdewObjectGrid();
            grid.ColdewObject = this.cobject;
            grid.Columns.Add(new GridColumn(this.nameField));
            this._littleOrangeInitializer.kehuInitializer.lianxirenTabPane.Children.Add(grid);
        }

        private void InitGridViews()
        {

            List<GridColumn> viewColumns = new List<GridColumn>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridColumn(field));
            }
            List<FilterExpression> expressions = new List<FilterExpression>();
            expressions.Add(new FavoriteFilterExpression(this.cobject));
            MetadataFilter filter = new MetadataFilter(expressions);
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "联系记录管理", true, true, null, viewColumns, lianxiRiqiField, this._littleOrangeInitializer.Admin));
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(manageView);
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收藏联系记录", true, true, filter, viewColumns, lianxiRiqiField, this._littleOrangeInitializer.Admin));
            this._littleOrangeInitializer.ColdewDataManager.GridViewDataProvider.Insert(favoriteView);
        }
    }
}
