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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Coldew.Core.Workflow;

namespace LittleOrange.Core
{
    public class LittleOrangeInitializer
    {
        public User Admin { private set; get; }
        public Group KehuAdminGroup { private set; get; }
        public LittleOrangeManager ColdewManager { private set; get; }
        public LiuchengMoban FahuoLiuchengMoban { private set; get; }
        public LittleOrangeInitializer(LittleOrangeManager coldewManager)
        {
            this.ColdewManager = coldewManager;
            this.Admin = coldewManager.OrgManager.UserManager.GetUserByAccount("admin");
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
            List<ColdewObject> objects = this.ColdewManager.ObjectManager.GetObjects();
            if (objects.Count == 0)
            {
                this.InitOrg();
                this.InitConfig();
                this.InitKehu();
                this.InitLianxiren();
                this.InitLianxiJilu();
                XiaoshouMingxiInitializer xiaoshouMingxi = new XiaoshouMingxiInitializer(this);
                xiaoshouMingxi.Initialize();
                ShoukuanMingxiInitializer shoukuanMingxi = new ShoukuanMingxiInitializer(this);
                shoukuanMingxi.Initialize();
                DingdanInitializer dingdan = new DingdanInitializer(this, xiaoshouMingxi, shoukuanMingxi);
                dingdan.Initialize();
                this.InitChanpin();
                this.FahuoLiuchengMoban = this.ColdewManager.LiuchengYinqing.LiuchengMobanManager.Create("FahuoLiucheng", "发货流程", dingdan.cobject, "~/FahuoLiucheng", "");
                this.ColdewManager.BindOrangeEvent();
                JObject biaodan = JsonConvert.DeserializeObject<JObject>("{zhuangtai: \"审核\", \"fahuoDanhao\":\"201406001\",\"fahuoRiqi\":\"2014-06-18T00:00:00\",\"yewuyuan\":{\"account\":\"fahuoyuan\",\"name\":\"发货员\"},\"kehu\":\"佛山市凯迪电器有限公司\",\"shouhuoren\":\"\",\"shouhuorenDianhua\":\"佛山 南海区 明沙中路11\",\"jiekuanFangshi\":\"2个月月结\",\"shouhuoDizhi\":\"佛山 南海区 明沙中路11\",\"chanpinGrid\":[{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":131,\"tongshu\":13,\"xiaoshouDanjia\":16,\"shijiDanjia\":12.88,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":2096,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":62.88,\"shifouKaipiao\":\"是\"},{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":1811,\"tongshu\":113,\"xiaoshouDanjia\":18,\"shijiDanjia\":14.49,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":32598,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":977.94,\"shifouKaipiao\":\"否\"},{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":1811,\"tongshu\":13,\"xiaoshouDanjia\":18,\"shijiDanjia\":14.49,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":32598,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":977.94,\"shifouKaipiao\":\"是\"}],\"beizhu\":\"\"}");
                Metadata metadata = dingdan.cobject.MetadataManager.Create(this.Admin, biaodan);
                Liucheng liucheng = this.ColdewManager.LiuchengYinqing.LiuchengManager.FaqiLiucheng(this.Admin, this.FahuoLiuchengMoban.ID, "", false, metadata);
                Xingdong xingdong = liucheng.ChuangjianXingdong("shenhe", "审核", "", null);
                xingdong.ChuangjianRenwu(this.Admin);
            }
        }

        private void InitConfig()
        {
            this.ColdewManager.ConfigManager.SetEmailConfig("2593975773", "2593975773@qq.com", "qwert12345", "smtp.qq.com");
        }

        private void InitOrg()
        {
            Position topPosition = this.ColdewManager.OrgManager.PositionManager.TopPosition;
            Position yewuyuanPosition = this.ColdewManager.OrgManager.PositionManager.Create(this.ColdewManager.OrgManager.System, new PositionCreateInfo { Name = "业务员", ParentId = topPosition.ID });

            User mengdong = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "蒙东",
                Account = "mengdong",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = topPosition.ID
            });

            User luohuaili = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "罗怀莉",
                Account = "luohuaili",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User lianglin = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "梁林",
                Account = "lianglin",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User fahuoyuan = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "发货员",
                Account = "fahuoyuan",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });


            this.KehuAdminGroup = this.ColdewManager.OrgManager.GroupManager.Create(this.Admin, new GroupCreateInfo { GroupType = GroupType.Group, Name = "管理员" });
            this.KehuAdminGroup.AddUser(this.Admin, this.Admin);
            this.KehuAdminGroup.AddUser(this.Admin, mengdong);
        }

        private void InitKehu()
        {
            this.ColdewManager.Logger.Info("init gongsiKehu");
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("客户", "kehu", ColdewObjectType.Standard, true));
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "公司名称") { Required = true});
            cobject.SetNameField(nameField);
            Field createTimeField = cobject.CreateDateField(new DateFieldCreateInfo("createTime", "创建日期") { Required = true, DefaultValueIsToday = true });
            Field yewuyuanField = cobject.CreateUserListField(new UserListFieldCreateInfo("yewuyuan", "业务员") { Required = true });
            Field yeuwlvField = cobject.CreateNumberField(new NumberFieldCreateInfo("yewulv", "业务率") { Required = true, Precision = 2, IsSummary = true });
            Field yewulvFangshiField = cobject.CreateRadioListField(new RadioListFieldCreateInfo("yewulvFangshi", "业务率方式", new List<string> { "按金额", "按重量" }) { IsSummary = true});
            Field jiekuanFangshild = cobject.CreateDropdownField(new DropdownFieldCreateInfo("jiekuanFangshi", "结款方式", new List<string> { "1个月月结", "2个月月结", "3个月月结" }) { IsSummary = true});
            Field lianxidianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("lianxidianhua", "公司电话"));
            Field gongsiDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("gongsiDizhi", "公司地址"));
            Field shouhuoDizhiField = cobject.CreateTextField(new TextFieldCreateInfo("shouhuoDizhi", "收货地址"));
            Field shouhuorenField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuoren", "收货人"));
            Field shouhuorenDianhuaField = cobject.CreateStringField(new StringFieldCreateInfo("shouhuorenDianhua", "收货人电话"));
            Field chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            Field emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮箱"));
            Field zhuangtaiField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态") { Suggestions = new List<string> { "意向客户", "普通客户", "已成交", "无效客户" } });
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

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

            Form editForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "客户管理", true, true, "", viewColumns, createTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏客户", true, true, "", viewColumns, createTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this.ColdewManager.OrgManager.Everyone, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.ColdewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);

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
            cobject.MetadataManager.Create(this.Admin, kehuXinxi);
        }

        private void InitLianxiren()
        {
            this.ColdewManager.Logger.Info("init gongsiKehu");
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("联系人", "lianxiren", ColdewObjectType.Standard, true));
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "姓名") { Required = true});
            cobject.SetNameField(nameField);
            Field kehuField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("kehu", "客户", "kehu") { Required = true, IsSummary = true });
            Field xingbieField = cobject.CreateDropdownField(new DropdownFieldCreateInfo("xingbie", "性别", new List<string> { "男", "女" }) { IsSummary = true });
            Field zhiweiField = cobject.CreateStringField(new StringFieldCreateInfo("zhiwei", "职位") { IsSummary = true});
            Field shoujiField = cobject.CreateStringField(new StringFieldCreateInfo("shouji", "手机") { IsSummary = true });
            Field chuanzhenField = cobject.CreateStringField(new StringFieldCreateInfo("chuanzhen", "传真"));
            Field zuojiField = cobject.CreateStringField(new StringFieldCreateInfo("zuoji", "座机") { IsSummary = true });
            Field qqField = cobject.CreateStringField(new StringFieldCreateInfo("qq", "QQ"));
            Field emailField = cobject.CreateStringField(new StringFieldCreateInfo("email", "邮件地址"));
            Field createTimeField = cobject.CreateDateField(new DateFieldCreateInfo("createTime", "创建日期") { Required = true, DefaultValueIsToday = true });
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

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

            Form editForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID });
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "联系人管理", true, true, "", viewColumns, createTimeField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏联系人", true, true, "", viewColumns, createTimeField.Code, "admin"));

            cobject.ObjectPermission.Create(this.ColdewManager.OrgManager.Everyone, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.ColdewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }

        private void InitLianxiJilu()
        {
            this.ColdewManager.Logger.Info("init lianxiJilu");
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("联系记录", "lianxiJilu", ColdewObjectType.Standard, true));
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "主题") { Required = true});
            cobject.SetNameField(nameField);
            Field kehuField = cobject.CreateRelatedField(new RelatedFieldCreateInfo("kehu", "客户", "lianxiren", "kehu") { IsSummary = true });
            Field lianxirenField = cobject.CreateMetadataField(new MetadataFieldCreateInfo("lianxiren", "联系人", "lianxiren") { IsSummary = true });
            Field wayField = cobject.CreateStringField(new StringFieldCreateInfo("fangshi", "联系方式") { IsSummary = true });
            Field lianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("lianxiRiqi", "联系日期"));
            Field xiaChiLianxiRiqiField = cobject.CreateDateField(new DateFieldCreateInfo("xiaChiLianxiRiqi", "下次联系日期"));
            Field remarkField = cobject.CreateTextField(new TextFieldCreateInfo("neirong", "联系内容"));

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

            Form editForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID});
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "联系记录管理", true, true, "", viewColumns, lianxiRiqiField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏联系记录", true, true, "", viewColumns, lianxiRiqiField.Code, "admin"));

            cobject.ObjectPermission.Create(this.ColdewManager.OrgManager.Everyone, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.ColdewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }

        private void InitXiaoshouGuanli()
        {
            
        }

        private void InitChanpin()
        {
            this.ColdewManager.Logger.Info("init chanpin");
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("产品", "chanpin", ColdewObjectType.Standard, true));
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "名称") { Required = true});
            cobject.SetNameField(nameField);
            Field guigeField = cobject.CreateStringField(new StringFieldCreateInfo("guige", "规格") { IsSummary = true });
            Field danweiField = cobject.CreateStringField(new StringFieldCreateInfo("danwei", "单位"));
            Field xiaoshouDijiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("xiaoshouDijia", "销售底价") { Precision = 2 });
            Field jinhuojiaField = cobject.CreateNumberField(new NumberFieldCreateInfo("jinhuojia", "进货价") { Precision = 2 });
            Field beizhuField = cobject.CreateTextField(new TextFieldCreateInfo("beizhu", "备注"));

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
                row.Children.Add(new Input(field) { Width = 6 });
                i++;
            }
            Form editForm = cobject.FormManager.Create(FormConstCode.EditFormCode, "", controls, null);

            List<GridViewColumnSetupInfo> viewColumns = new List<GridViewColumnSetupInfo>();
            foreach (Field field in cobject.GetFields())
            {
                viewColumns.Add(new GridViewColumnSetupInfo { FieldId = field.ID});
            }

            GridView manageView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Standard, "", "产品管理", true, true, "", viewColumns, nameField.Code, "admin"));
            GridView favoriteView = cobject.GridViewManager.Create(new GridViewCreateInfo(GridViewType.Favorite, "", "收藏产品", true, true, "", viewColumns, nameField.Code, "admin"));

            cobject.ObjectPermission.Create(this.KehuAdminGroup, ObjectPermissionValue.All);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.ColdewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);

            JObject chanpinXinxi = new JObject();
            chanpinXinxi.Add(nameField.Code, "绝缘漆");
            chanpinXinxi.Add(guigeField.Code, "YJ-601B");
            chanpinXinxi.Add(danweiField.Code, "KG");
            chanpinXinxi.Add(xiaoshouDijiaField.Code, "12.7");
            chanpinXinxi.Add(jinhuojiaField.Code, "11");
            cobject.MetadataManager.Create(this.Admin, chanpinXinxi);
        }

    }
}
