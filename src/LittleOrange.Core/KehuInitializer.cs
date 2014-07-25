using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Api.UI;
using Coldew.Core;
using Coldew.Core.Search;
using Coldew.Core.UI;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class KehuInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        LittleOrangeInitializer _littleOrangeInitializer;
        public Field nameField;
        Field createTimeField;
        Field yewuyuanField;
        Field yeuwlvField;
        Field yewulvFangshiField;
        Field jiekuanFangshild;
        Field lianxidianhuaField;
        Field gongsiDizhiField;
        Field shouhuoDizhiField;
        Field shouhuorenField;
        Field shouhuorenDianhuaField;
        Field chuanzhenField;
        Field emailField;
        Field zhuangtaiField;
        Field remarkField;
        public KehuInitializer(LittleOrangeInitializer littleOrangeInitializer)
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
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("客户", "kehu", true));
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "公司名称") { Required = true });
            cobject.SetNameField(nameField);
            createTimeField = cobject.CreateDateField(new DateFieldCreateInfo("createTime", "创建日期") { Required = true, DefaultValueIsToday = true });
            yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            yeuwlvField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewulv", "业务率") { Required = true, Precision = 2, IsSummary = true });
            yewulvFangshiField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("yewulvFangshi", "业务率方式", new List<string> { "按金额", "按重量" }) { IsSummary = true });
            jiekuanFangshild = cobject.CreateDropdownField(new DropdownFieldCreateInfo("jiekuanFangshi", "结款方式", new List<string> { "1个月月结", "2个月月结", "3个月月结" }) { IsSummary = true });
            lianxidianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("lianxidianhua", "公司电话"));
            gongsiDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("gongsiDizhi", "公司地址"));
            shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址"));
            shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮箱"));
            zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态") { Suggestions = new List<string> { "意向客户", "普通客户", "已成交", "无效客户" } });
            remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._coldewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }

        protected void InitForms()
        {
            List<Control> controls = this.CreateControls(false);
            Form editForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.EditFormCode, Title = "", Controls = controls });
            Form createForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.CreateFormCode, Title = "", Controls = controls });
            controls = this.CreateControls(true);
            Form detailsForm = cobject.FormManager.Create(new FormCreateInfo { Code = FormConstCode.DetailsFormCode, Title = "", Controls = controls });
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
            List<GridViewColumn> viewColumns = new List<GridViewColumn>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumn(field));
            }
            List<FilterExpression> expressions = new List<FilterExpression>();
            expressions.Add(new FavoriteFilterExpression(this.cobject));
            MetadataFilter filter = new MetadataFilter(expressions);
            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "客户管理", true, true, null, viewColumns, createTimeField, this._littleOrangeInitializer.Admin));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo("", "收藏客户", true, true, filter, viewColumns, createTimeField ,this._littleOrangeInitializer.Admin));
        }

        public void CreateTestData()
        {
            JObject kehuXinxi = new JObject();
            kehuXinxi.Add(nameField.Code, "佛山市凯迪电器有限公司");
            kehuXinxi.Add(yewuyuanField.Code, "luohuaili");
            kehuXinxi.Add(yeuwlvField.Code, "0.03");
            kehuXinxi.Add(yewulvFangshiField.Code, "按金额");
            kehuXinxi.Add(jiekuanFangshild.Code, "2个月月结");
            kehuXinxi.Add(gongsiDizhiField.Code, "佛山 南海区 明沙中路11");
            kehuXinxi.Add(shouhuoDizhiField.Code, "佛山 南海区 明沙中路11");
            kehuXinxi.Add(shouhuorenDianhuaField.Code, "佛山 南海区 明沙中路11");
            kehuXinxi.Add(createTimeField.Code, DateTime.Now);
            MetadataValueDictionary value = new MetadataValueDictionary(this.cobject, kehuXinxi);
            MetadataCreateInfo createInfo = new MetadataCreateInfo() { Creator = this._littleOrangeInitializer.Admin, Value = value };
            cobject.MetadataManager.Create(createInfo);
        }
    }
}
