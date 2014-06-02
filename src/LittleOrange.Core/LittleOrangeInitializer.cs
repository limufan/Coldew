using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Api.UI;
using Coldew.Api;
using Coldew.Core.UI;
using Coldew.Api.Organization;

namespace LittleOrange.Core
{
    public class LittleOrangeInitializer
    {
        User _admin;
        Group kehuAdminGroup;
        ColdewManager _coldewManager;
        public LittleOrangeInitializer(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
            this._admin = crmManager.OrgManager.UserManager.GetUserByAccount("admin");
#if DEBUG
            this.Init();
#else
            try
            {
                this.Init();
            }
            catch (Exception ex)
            {
                this._coldewManager.Logger.Error(ex.Message, ex);
                throw;
            }
#endif
        }

        void Init()
        {
            List<ColdewObject> objects = this._coldewManager.ObjectManager.GetObjects();
            if (objects.Count == 0)
            {
                this.InitOrg();
                this.InitConfig();
                this.InitKehu();
                this.InitLianxiren();
                this.InitLianxiJilu();
                this.InitFahuo();
                this.InitFahuoLiucheng();
                this.InitChanpin();
            }
        }

        private void InitConfig()
        {
            this._coldewManager.ConfigManager.SetEmailConfig("2593975773", "2593975773@qq.com", "qwert12345", "smtp.qq.com");
        }

        private void InitOrg()
        {
            Position topPosition = this._coldewManager.OrgManager.PositionManager.TopPosition;
            Position yewuyuanPosition = this._coldewManager.OrgManager.PositionManager.Create(this._coldewManager.OrgManager.System, new PositionCreateInfo { Name = "业务员", ParentId = topPosition.ID });

            User mengdong = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "蒙东",
                Account = "mengdong",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = topPosition.ID
            });

            User luohuaili = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "罗怀莉",
                Account = "luohuaili",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User lianglin = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "梁林",
                Account = "lianglin",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User fahuoyuan = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "发货员",
                Account = "fahuoyuan",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });


            this.kehuAdminGroup = this._coldewManager.OrgManager.GroupManager.Create(this._admin, new GroupCreateInfo { GroupType = GroupType.Group, Name = "管理员" });
            this.kehuAdminGroup.AddUser(this._admin, this._admin);
            this.kehuAdminGroup.AddUser(this._admin, mengdong);
        }

        private void InitKehu()
        {
            this._coldewManager.Logger.Info("init gongsiKehu");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("客户", "kehu", ColdewObjectType.Standard, true, "公司名称"));
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            Field yeuwlvField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewulv", "业务率") { Required = true, Precision = 2, IsSummary = true });
            Field yewulvFangshiField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("yewulvFangshi", "业务率方式", new List<string> { "按金额", "按重量" }) { IsSummary = true});
            Field jiekuanFangshild = cobject.CreateDropdownField(new DropdownFieldCreateInfo("jiekuanFangshi", "结款方式", new List<string> { "现金", "1个月月结", "2个月月结", "3个月月结" }) { IsSummary = true});
            Field lianxidianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("lianxidianhua", "公司电话"));
            Field gongsiDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("gongsiDizhi", "公司地址"));
            Field shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址"));
            Field shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            Field shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            Field chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            Field emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮箱"));
            Field zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态") { Suggestions = new List<string> { "意向客户", "普通客户", "已成交", "无效客户" } });
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 80 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "客户管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏客户", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.Create | ObjectPermissionValue.View);
            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.kehuAdminGroup), MetadataPermissionValue.All, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(yewuyuanField), MetadataPermissionValue.View | MetadataPermissionValue.Modify, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(cobject.CreatedUserField), MetadataPermissionValue.All, null);
        }

        private void InitLianxiren()
        {
            this._coldewManager.Logger.Info("init gongsiKehu");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("联系人", "lianxiren", ColdewObjectType.Standard, true, "姓名"));
            Field kehuField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("kehu", "客户", "kehu") { Required = true, IsSummary = true });
            Field xingbieField = cobject.CreateDropdownField(new DropdownFieldCreateInfo("xingbie", "性别", new List<string> { "男", "女" }) { IsSummary = true });
            Field zhiweiField = cobject.CreateStringField(new StringFieldCreateInfo("zhiwei", "职位") { IsSummary = true});
            Field shoujiField = cobject.CreateStringField(new StringFieldCreateInfo("shouji", "手机") { IsSummary = true });
            Field chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            Field zuojiField = cobject.CreateStringField(new StringFieldCreateInfo("zuoji", "座机") { IsSummary = true });
            Field qqField = cobject.CreateStringField(new StringFieldCreateInfo("qq", "QQ"));
            Field emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮件地址"));
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 100 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "联系人管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏联系人", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.Create | ObjectPermissionValue.View);
            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.All);
            cobject.MetadataPermission.RelatedPermission.Create(kehuField.Code);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(cobject.CreatedUserField), MetadataPermissionValue.All, null);
        }

        private void InitLianxiJilu()
        {
            this._coldewManager.Logger.Info("init lianxiJilu");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("联系记录", "lianxiJilu", ColdewObjectType.Standard, true, "主题"));
            Field kehuField = cobject.CreateRelatedField(new RelatedFieldCreateInfo("kehu", "客户", "lianxiren", "kehu") { IsSummary = true });
            Field lianxirenField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("lianxiren", "联系人", "lianxiren") { IsSummary = true });
            Field wayField = cobject.CreateStringField(new StringFieldCreateInfo("fangshi", "联系方式") { IsSummary = true });
            Field lianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("lianxiRiqi", "联系日期"));
            Field xiaChiLianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("xiaChiLianxiRiqi", "下次联系日期"));
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("neirong", "联系内容"));

            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField ||
                    field == kehuField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 100 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "联系记录管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏联系记录", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.Create | ObjectPermissionValue.View);
            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.All);
            cobject.MetadataPermission.RelatedPermission.Create(kehuField.Code);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(cobject.CreatedUserField), MetadataPermissionValue.All, null);
        }

        private void InitFahuo()
        {
            this._coldewManager.Logger.Info("init fahuo");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("销售明细", "xiaoshouMingxi", ColdewObjectType.Standard, true, "产品名称"));
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员"));
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "日期"));
            Field chuhuoDanhaoField = cobject.CreateStringField(new StringFieldCreateInfo("chuhuoDanhao", "出货单号"));
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "发货客户"));
            Field guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格"));
            Field danweiField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位"));
            Field shuliangField = cobject.CreateNumberField(new NumberFieldCreateInfo("shuliang", "数量"));
            Field tongshuField = cobject.CreateNumberField(new NumberFieldCreateInfo("tongshu", "桶数"));
            Field xiaoshouDijiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDijia", "销售底价") { Precision = 2 });
            Field shijiDanjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("shijiDanjia", "实际单价") { Precision = 2 });
            Field xiaoshouDanjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDanjia", "销售单价") { Precision = 2 });
            Field zongjineField = cobject.CreateNumberField(new NumberFieldCreateInfo("zongjine", "金额"){ Precision = 2});
            Field yewulvField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewulv", "业务率") { Precision = 2 });
            Field yewufeiField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewufei", "业务费") { Precision = 2 });
            Field shoukuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("shoukuanJine", "收款金额") { Precision = 2 });
            Field yunfeiField = cobject.CreateNumberField(new NumberFieldCreateInfo("yunfei", "运费") { Precision = 2 });
            Field tichengField = cobject.CreateNumberField(new NumberFieldCreateInfo("ticheng", "提成") { Precision = 2 });
            Field shifouKaipiaoField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("shifouKaipiao", "是否开票", new List<string>() {"是", "否" }));
            Field butieField = cobject.CreateNumberField(new NumberFieldCreateInfo("butie", "补贴") { Precision = 2 });
            Field lirunField = cobject.CreateNumberField(new NumberFieldCreateInfo("lirun", "利润") { Precision = 2 });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));


            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));
            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);
            detailsInputs.RemoveAll(x => x.Field == yewuyuanField || x.Field == fahuoRiqiField || x.Field == chuhuoDanhaoField || x.Field == kehuField );

            List<Input> fahuo_chanpin_form_inputs = new List<Input>();
            fahuo_chanpin_form_inputs.Add(new Input(cobject.NameField));
            fahuo_chanpin_form_inputs.Add(new Input(guigeField));
            fahuo_chanpin_form_inputs.Add(new Input(danweiField));
            fahuo_chanpin_form_inputs.Add(new Input(shuliangField));
            fahuo_chanpin_form_inputs.Add(new Input(tongshuField));
            fahuo_chanpin_form_inputs.Add(new Input(xiaoshouDanjiaField));
            fahuo_chanpin_form_inputs.Add(new Input(zongjineField));
            fahuo_chanpin_form_inputs.Add(new Input(yewulvField));
            fahuo_chanpin_form_inputs.Add(new Input(yewufeiField));
            fahuo_chanpin_form_inputs.Add(new Input(shifouKaipiaoField));
            List<Section> fahuo_chanpin_form_sections = new List<Section>();
            fahuo_chanpin_form_sections.Add(new Section("基本信息", 2, fahuo_chanpin_form_inputs));
            Form fahuo_chanpin_form = cobject.FormManager.Create("fahuo_chanpin_form", "", fahuo_chanpin_form_sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 80 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "发货管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏发货", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(yewuyuanField), MetadataPermissionValue.View, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.kehuAdminGroup), MetadataPermissionValue.View | MetadataPermissionValue.Modify, null);
            cobject.FieldPermission.Create(xiaoshouDijiaField.Code, this.kehuAdminGroup, FieldPermissionValue.All);
        }

        private void InitFahuoLiucheng()
        {
            this._coldewManager.Logger.Info("init fahuo liucheng");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("发货流程", "FahuoLiucheng", ColdewObjectType.Workflow, true, "出货单号"));
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "日期") { DefaultValueIsToday = true });
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员"));
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "发货客户"));
            Field shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址"));
            Field shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            Field shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            Field chanpinGridField = cobject.CreateJsonField(new FieldCreateInfo("chanpinGrid", "发货产品", "", false, true) { Required = true });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));
            Field liuchengIdField = cobject.CreateStringField(new StringFieldCreateInfo("liuchengId", "流程ID"));

            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField ||
                    field == chanpinGridField ||
                    field == liuchengIdField ||
                    field == beizhuField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));

            List<Input> chanpinGridInputs = new List<Input>();
            chanpinGridInputs.Add(new Input(chanpinGridField));
            sections.Add(new Section("产品信息", 1, chanpinGridInputs));

            List<Input> beizhuInputs = new List<Input>();
            beizhuInputs.Add(new Input(beizhuField));
            sections.Add(new Section("备注信息", 1, beizhuInputs));


            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 80 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "发货流程管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code + " desc", "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏发货流程", true, true, "", viewColumns, cobject.CreatedTimeField.Code + " desc", "admin"));

            this._coldewManager.LiuchengYinqing.LiuchengMobanManager.Create("FahuoLiucheng", "发货流程", cobject, "~/FahuoLiucheng", "");

            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.View);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.kehuAdminGroup), MetadataPermissionValue.View, null);

            cobject.ObjectPermission.Create(this._admin, ObjectPermissionValue.View);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._admin), MetadataPermissionValue.View, null);
        }

        private void InitChanpin()
        {
            this._coldewManager.Logger.Info("init chanpin");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("产品", "chanpin", ColdewObjectType.Standard, true, "名称"));
            Field guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格") { IsSummary = true });
            Field danweiQiyeField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位"));
            Field xiaoshouDijiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDijia", "销售底价") { Precision = 2 });
            Field jinhuojiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("jinhuojia", "进货价") { Precision = 2 });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

            List<Input> detailsInputs = new List<Input>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                detailsInputs.Add(new Input(field));
            }
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 2, detailsInputs));

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                if (field == cobject.CreatedUserField ||
                    field == cobject.CreatedTimeField ||
                    field == cobject.ModifiedUserField ||
                    field == cobject.ModifiedTimeField)
                {
                    continue;
                }
                viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = field.Code, Width = 100 });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "产品管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏产品", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._coldewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }

    }
}
