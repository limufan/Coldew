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
        User _shenqiudi;
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

            User chenmei = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "陈梅",
                Account = "chenmei",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = topPosition.ID
            });

            User chenxia = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "程霞",
                Account = "chenxia",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = topPosition.ID
            });

            this._shenqiudi = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "沈秋娣",
                Account = "shenqiudi",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User xiejun = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "谢君",
                Account = "xiejun",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User xianghao = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "向好",
                Account = "xianghao",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User chendong = this._coldewManager.OrgManager.UserManager.Create(this._coldewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "程东",
                Account = "chendong",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            this.kehuAdminGroup = this._coldewManager.OrgManager.GroupManager.Create(this._admin, new GroupCreateInfo { GroupType = GroupType.Group, Name = "管理员" });
            this.kehuAdminGroup.AddUser(this._admin, chenmei);
            this.kehuAdminGroup.AddUser(this._admin, chenxia);
            this.kehuAdminGroup.AddUser(this._admin, this._admin);
        }

        private void InitKehu()
        {
            this._coldewManager.Logger.Info("init gongsiKehu");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("客户", "kehu", ColdewObjectType.Standard, true, "公司名称"));
            Field shengfenField = cobject.CreateStringField(new StringFieldCreateInfo("shengfen", "省份") { IsSummary = true });
            Field diquField = cobject.CreateStringField(new StringFieldCreateInfo("diqu", "地区") { IsSummary = true });
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            Field lianxidianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("lianxidianhua", "公司电话"));
            Field gongsiDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("gongsiDizhi", "公司地址"));
            Field chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            Field emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮箱"));
            Field zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态") { Suggestions = new List<string> { "意向客户", "普通客户", "已成交", "无效客户" }, IsSummary = true });
            Field gongsiXinzhiField = cobject.CreateStringField(new StringFieldCreateInfo("gongsiXinzhi", "公司性质") { Suggestions = new List<string> { "国有", "民营", "私企", "药厂直属公司" }, IsSummary = true });
            Field yunyiFangsiField = cobject.CreateStringField(new StringFieldCreateInfo("yunyiFangsi", "运营方式") { Suggestions = new List<string> { "临床.OTC", "自己开发", "挂靠", "纯配送", "招商", "批发" }, IsSummary = true });
            Field yixiangChanpinField = cobject.CreateStringField(new StringFieldCreateInfo("yixiangChanpin", "意向产品") { IsSummary = true });
            Field guakaoGongsiField = cobject.CreateStringField(new StringFieldCreateInfo("guakaoGongsi", "挂靠公司名称"));
            Field zhuyinQuyuField = cobject.CreateStringField(new StringFieldCreateInfo("zhuyinQuyu", "主营区域及医院"));
            Field zhuyinChanpinField = cobject.CreateStringField(new StringFieldCreateInfo("zhuyinChanpin", "主营产品及月销量"));
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
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = yewuyuanField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = shengfenField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = diquField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = cobject.NameField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = lianxidianhuaField.Code, Width = 110 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = zhuangtaiField.Code, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = gongsiXinzhiField.Code, Width = 80 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = yunyiFangsiField.Code, Width = 120 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = yixiangChanpinField.Code, Width = 120 });

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
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("订单总表", "dingdanZhongbiao", ColdewObjectType.Standard, true, "产品名称"));
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员"));
            Field shengfenField = cobject.CreateStringField(new StringFieldCreateInfo("shengfen", "省份"));
            Field diquField = cobject.CreateStringField(new StringFieldCreateInfo("diqu", "地区"));
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期"));
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "客户名称"));
            Field guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格"));
            Field shengchanQiyeField = cobject.CreateStringField(new StringFieldCreateInfo("shengchanQiye", "生产企业"));
            Field zongshuliangField = cobject.CreateNumberField(new NumberFieldCreateInfo("zongshuliang", "总数量"){ Precision = 2});
            Field chengbenjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("chengbenjia", "成本价"){ Precision = 2});
            Field xiaoshoujiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshoujia", "销售价"){ Precision = 2});
            Field chukujiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("chukuDanjia", "出库单价"){ Precision = 2});
            Field zongjineField = cobject.CreateNumberField(new NumberFieldCreateInfo("zongjine", "总金额"){ Precision = 2});
            Field huikuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("huikuanRiqi", "汇款日期"));
            Field huikuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("huikuanJine", "汇款金额"){ Precision = 2});
            Field huikuanDanweiField = cobject.CreateStringField(new StringFieldCreateInfo("huikuanDanwei", "汇款单位"));
            Field daokuanDanweiField = cobject.CreateStringField(new StringFieldCreateInfo("daokuanDanwei", "到款单位"));
            Field kaipiaoDanweiField = cobject.CreateStringField(new StringFieldCreateInfo("kaipiaoDanwei", "开票单位"));
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
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = yewuyuanField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = shengfenField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = diquField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = fahuoRiqiField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = kehuField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = cobject.NameField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = guigeField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = shengchanQiyeField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = zongshuliangField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = chengbenjiaField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = xiaoshoujiaField.Code, Width = 70 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = chukujiaField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = zongjineField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = huikuanRiqiField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = huikuanJineField.Code, Width = 100 });
            viewColumns.Add(new GridViewColumnSetupInfo { FieldCode = huikuanJineField.Code, Width = 100 });

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "发货管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏发货", true, true, "", viewColumns, cobject.CreatedTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.View | ObjectPermissionValue.Export | ObjectPermissionValue.PermissionSetting);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(yewuyuanField), MetadataPermissionValue.View, null);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.kehuAdminGroup), MetadataPermissionValue.View | MetadataPermissionValue.Modify, null);
            cobject.FieldPermission.Create(chengbenjiaField.Code, this.kehuAdminGroup, FieldPermissionValue.All);
            cobject.FieldPermission.Create(xiaoshoujiaField.Code, this.kehuAdminGroup, FieldPermissionValue.All);
        }

        private void InitFahuoLiucheng()
        {
            this._coldewManager.Logger.Info("init fahuo liucheng");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("发货流程", "FahuoLiucheng", ColdewObjectType.Workflow, true, "单号"));
            Field shengfenField = cobject.CreateStringField(new StringFieldCreateInfo("shengfen", "省份") { Required = true });
            Field diquField = cobject.CreateStringField(new StringFieldCreateInfo("diqu", "地区") { Required = true });
            Field kehuField = cobject.CreateStringField(new StringFieldCreateInfo("kehu", "顾客名称") { Required = true });
            Field jingshourenField = cobject.CreateStringField(new StringFieldCreateInfo("jingshouren", "经手人") { Required = true });
            Field fahuoRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("fahuoRiqi", "发货日期") { Required = true });
            Field huikuanRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("huikuanRiqi", "汇款日期") { Required = true });
            Field huikuanJineField = cobject.CreateNumberField(new NumberFieldCreateInfo("huikuanJine", "汇款金额"){ Precision = 3,Required = true });
            Field huikuanLeixingField = cobject.CreateStringField(new StringFieldCreateInfo("huikuanLeixing", "汇款类型") { Required = true });
            Field huikuanDanweiField = cobject.CreateTextField(new TextFieldCreateInfo("huikuanDanwei", "汇款单位") { Required = true });
            Field daokuanDanweiField = cobject.CreateTextField(new TextFieldCreateInfo("daokuanDanwei", "到款单位") { Required = true });
            Field kaipiaoDanweiField = cobject.CreateTextField(new TextFieldCreateInfo("kaipiaoDanwei", "开票单位") { Required = true });
            Field shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址") { Required = true });
            Field shouhuorenField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoren", "收货人及电话") { Required = true });
            Field songhuoFangshiField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("songhuoFangshi", "送货方式", new List<string> { "物流自提", "送货上门" }) { Required = true });
            Field chanpinGridField = cobject.CreateJsonField(new FieldCreateInfo("chanpinGrid", "产品信息", "", false, true) { Required = true });
            Field suihuoFudaiField = cobject.CreateStringField(new StringFieldCreateInfo("suihuoFudai", "随货附带") { Suggestions = new List<string> { "送货单", "药检", "公司首营", "品种资料" } });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));
            Field liuchengIdField = cobject.CreateStringField(new StringFieldCreateInfo("liuchengId", "流程ID"));

            List<Input> detailsInputs = new List<Input>();
            detailsInputs.Add(new Input(cobject.NameField));
            detailsInputs.Add(new Input(jingshourenField));
            detailsInputs.Add(new Input(kehuField));
            detailsInputs.Add(new Input(shengfenField));
            detailsInputs.Add(new Input(diquField));
            detailsInputs.Add(new Input(fahuoRiqiField));
            detailsInputs.Add(new Input(huikuanRiqiField));
            detailsInputs.Add(new Input(huikuanJineField));
            detailsInputs.Add(new Input(huikuanLeixingField));
            detailsInputs.Add(new Input(huikuanDanweiField));
            detailsInputs.Add(new Input(daokuanDanweiField));
            detailsInputs.Add(new Input(kaipiaoDanweiField));
            detailsInputs.Add(new Input(songhuoFangshiField));
            detailsInputs.Add(new Input(shouhuoDizhiField));
            detailsInputs.Add(new Input(shouhuorenField));
            List<Section> sections = new List<Section>();
            sections.Add(new Section("基本信息", 3, detailsInputs));

            List<Input> chanpinGridInputs = new List<Input>();
            chanpinGridInputs.Add(new Input(chanpinGridField));
            sections.Add(new Section("产品信息", 1, chanpinGridInputs));

            List<Input> beizhuInputs = new List<Input>();
            beizhuInputs.Add(new Input(suihuoFudaiField));
            beizhuInputs.Add(new Input(beizhuField));
            sections.Add(new Section("备注信息", 1, beizhuInputs));

            Form editForm = cobject.FormManager.Create(FormConstCode.DetailsFormCode, "", sections, null);
            Input jingshourenInput = detailsInputs.Find(x => x.Field == jingshourenField);
            detailsInputs.Remove(jingshourenInput);
            Input huikuanLeixingInput = detailsInputs.Find(x => x.Field == huikuanLeixingField);
            detailsInputs.Remove(huikuanLeixingInput);
            Input daokuanDanweiInput = detailsInputs.Find(x => x.Field == daokuanDanweiField);
            detailsInputs.Remove(daokuanDanweiInput);

            Form fahuoForm = cobject.FormManager.Create("form_fahuo", "", sections, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields().Take(12))
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

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "发货流程管理", true, true, "", viewColumns, cobject.CreatedTimeField.Code + " desc", "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏发货流程", true, true, "", viewColumns, cobject.CreatedTimeField.Code + " desc", "admin"));

            this._coldewManager.LiuchengYinqing.LiuchengMobanManager.Create("FahuoLiucheng", "发货流程", cobject, "~/FahuoLiucheng", "");

            cobject.ObjectPermission.Create(this.kehuAdminGroup, ObjectPermissionValue.View);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.kehuAdminGroup), MetadataPermissionValue.View, null);

            cobject.ObjectPermission.Create(this._shenqiudi, ObjectPermissionValue.View);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._shenqiudi), MetadataPermissionValue.View, null);
        }

        private void InitChanpin()
        {
            this._coldewManager.Logger.Info("init chanpin");
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("产品", "chanpin", ColdewObjectType.Standard, true, "名称"));
            Field bianhaoField = cobject.CreateStringField(new StringFieldCreateInfo("bianhao", "编号"));
            Field guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格") { IsSummary = true });
            Field shengchanQiyeField = cobject.CreateStringField(new StringFieldCreateInfo("shengchanQiye", "生产企业") { IsSummary = true });
            Field danweiQiyeField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位"));
            Field chukuDanjiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("chukuDanjia", "出库单价"){ Precision = 2});
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
