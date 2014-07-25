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
        public KehuInitializer kehuInitializer;
        public LianxirenInitializer lianxirenInitializer;
        public LianxiJiluInitializer lianxiJiluInitializer;
        public XiaoshouMingxiInitializer xiaoshouMingxi;
        public ShoukuanMingxiInitializer shoukuanMingxi;
        public DingdanInitializer dingdanInitializer;
        public ChanpinInitializer chanpinInitializer;
        public LiuchengInitializer liuchengInitializer;

        public LittleOrangeInitializer(LittleOrangeManager coldewManager)
        {
            this.ColdewManager = coldewManager;
            this.Admin = this.ColdewManager.OrgManager.UserManager.GetUserByAccount("admin");
#if DEBUG
            this.Init();
#else
            try
            {
                this.Init();
            }
            catch (Exception ex)
            {
                this.ColdewManager.Logger.Error(ex.Message, ex);
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
                kehuInitializer = new KehuInitializer(this);
                kehuInitializer.Initialize();
                lianxirenInitializer = new LianxirenInitializer(this);
                lianxirenInitializer.Initialize();
                lianxiJiluInitializer = new LianxiJiluInitializer(this);
                lianxiJiluInitializer.Initialize();
                xiaoshouMingxi = new XiaoshouMingxiInitializer(this);
                xiaoshouMingxi.Initialize();
                shoukuanMingxi = new ShoukuanMingxiInitializer(this);
                shoukuanMingxi.Initialize();
                liuchengInitializer = new LiuchengInitializer(this);
                liuchengInitializer.Initialize();
                dingdanInitializer = new DingdanInitializer(this, xiaoshouMingxi, shoukuanMingxi, liuchengInitializer);
                dingdanInitializer.Initialize();
                chanpinInitializer = new ChanpinInitializer(this);
                chanpinInitializer.Initialize();

                this.FahuoLiuchengMoban = this.ColdewManager.LiuchengYinqing.LiuchengMobanManager.Create("FahuoLiucheng", "发货流程", dingdanInitializer.cobject, "~/FahuoLiucheng", "");
                this.ColdewManager.BindOrangeEvent();
#if DEBUG
                this.CreateTestData();
#endif
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
            this.KehuAdminGroup.AddMember(this.Admin, this.Admin);
            this.KehuAdminGroup.AddMember(this.Admin, mengdong);
        }

        private void InitOrg1()
        {
            Position topPosition = this.ColdewManager.OrgManager.PositionManager.TopPosition;
            Position yewuyuanPosition = this.ColdewManager.OrgManager.PositionManager.Create(this.ColdewManager.OrgManager.System, new PositionCreateInfo { Name = "业务员", ParentId = topPosition.ID });

            User mengdong = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "刘燕",
                Account = "liuyan",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User luohuaili = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "李世全",
                Account = "lishiquan",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            User lianglin = this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "余佳承",
                Account = "yujiacheng",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "杨科",
                Account = "yangke",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });

            this.ColdewManager.OrgManager.UserManager.Create(this.ColdewManager.OrgManager.System, new UserCreateInfo
            {
                Name = "瞿灯桂",
                Account = "qudenggui",
                Password = "123456",
                Status = UserStatus.Normal,
                MainPositionId = yewuyuanPosition.ID
            });


            this.KehuAdminGroup = this.ColdewManager.OrgManager.GroupManager.Create(this.Admin, new GroupCreateInfo { GroupType = GroupType.Group, Name = "管理员" });
            this.KehuAdminGroup.AddMember(this.Admin, this.Admin);
            this.KehuAdminGroup.AddMember(this.Admin, mengdong);
        }

        private void CreateTestData()
        {
            JObject biaodan = JsonConvert.DeserializeObject<JObject>("{zhuangtai: \"审核\", \"fahuoDanhao\":\"201406001\",\"fahuoRiqi\":\"2014-06-18T00:00:00\",\"yewuyuan\":{\"account\":\"fahuoyuan\",\"name\":\"发货员\"},\"kehu\":\"佛山市凯迪电器有限公司\",\"shouhuoren\":\"\",\"shouhuorenDianhua\":\"佛山 南海区 明沙中路11\",\"jiekuanFangshi\":\"2个月月结\",\"shouhuoDizhi\":\"佛山 南海区 明沙中路11\",\"chanpinGrid\":[{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":131,\"tongshu\":13,\"xiaoshouDanjia\":16,\"shijiDanjia\":12.88,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":2096,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":62.88,\"shifouKaipiao\":\"是\"},{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":1811,\"tongshu\":113,\"xiaoshouDanjia\":18,\"shijiDanjia\":14.49,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":32598,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":977.94,\"shifouKaipiao\":\"否\"},{\"name\":\"绝缘漆\",\"guige\":\"YJ-601B\",\"danwei\":\"KG\",\"shuliang\":1811,\"tongshu\":13,\"xiaoshouDanjia\":18,\"shijiDanjia\":14.49,\"xiaoshouDijia\":12.7,\"butie\":300,\"zongjine\":32598,\"yewulv\":0.03,\"yewulvFangshi\":\"按金额\",\"yewufei\":977.94,\"shifouKaipiao\":\"是\"}],\"beizhu\":\"\"}");
            MetadataValueDictionary value = new MetadataValueDictionary(dingdanInitializer.cobject, biaodan);
            MetadataCreateInfo createInfo = new MetadataCreateInfo() { Creator = this.Admin, Value = value };
            Metadata metadata = dingdanInitializer.cobject.MetadataManager.Create(createInfo);
            Liucheng liucheng = this.ColdewManager.LiuchengYinqing.LiuchengManager.FaqiLiucheng(this.Admin, this.FahuoLiuchengMoban.ID, "", false, metadata);
            Xingdong xingdong = liucheng.ChuangjianXingdong("shenhe", "审核", "", null);
            xingdong.ChuangjianRenwu(this.Admin);

            this.kehuInitializer.CreateTestData();
            this.chanpinInitializer.CreateTestData();
        }

    }
}
