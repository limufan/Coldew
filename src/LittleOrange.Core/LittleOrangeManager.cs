using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Workflow;
using Newtonsoft.Json.Linq;
using Coldew.Core.Search;

namespace LittleOrange.Core
{
    public class LittleOrangeManager : ColdewManager
    {
        LiuchengMoban _moban;
        ColdewObject _dingdanOobject;

        public LittleOrangeManager()
        {
        }

        protected override void Load()
        {
            this._dingdanOobject = this.ObjectManager.GetObjectByCode("shoukuanGuanli");
            this._dingdanOobject.MetadataManager.MetadataFactory = new DingdanMetadataFactory(this._dingdanOobject.MetadataManager);
            base.Load();
            this.LiuchengYinqing.LiuchengManager.LiuchengWanchenghou += LiuchengManager_LiuchengWanchenghou;
            this.BindOrangeEvent();
        }

        public void BindOrangeEvent()
        {
            this._moban = this.LiuchengYinqing.LiuchengMobanManager.GetMobanByCode("FahuoLiucheng");
            if (_dingdanOobject != null)
            {
                this._dingdanOobject.MetadataManager.Creating += MetadataManager_Creating;
                this._dingdanOobject.MetadataManager.Created += MetadataManager_Created;
                this._dingdanOobject.MetadataManager.MetadataChanging += MetadataManager_MetadataChanging;
                this._dingdanOobject.MetadataManager.MetadataChanged += MetadataManager_MetadataChanged;
                this._dingdanOobject.MetadataManager.MetadataDeleted += MetadataManager_MetadataDeleted;
            }
        }

        void MetadataManager_MetadataDeleted(MetadataManager sender, Metadata args)
        {
            Dingdan dingdan = new Dingdan(args.GetJObject(this.OrgManager.System));
            this.DeleteXiaoshouMingxi(dingdan);
            this.DeleteShoukuanMingxi(dingdan);
        }

        void MetadataManager_MetadataChanging(MetadataManager sender, MetadataChangingEventArgs args)
        {
            Dingdan dingdan = new Dingdan(args.ChangeInfo);
            if (dingdan.chanpinGrid != null && dingdan.chanpinGrid.Count > 0)
            {
                dingdan.Jisuan();
                foreach (JProperty property in dingdan.Properties())
                {
                    args.ChangeInfo[property.Name] = property.Value;
                }
            }
        }

        void MetadataManager_Created(MetadataManager sender, Metadata args)
        {
            this.Tongbu(args);
        }

        void MetadataManager_Creating(MetadataManager sender, MetadataCreateInfo createInfo)
        {
            Dingdan dingdan = new Dingdan(createInfo.JObject);
            dingdan.Jisuan();
            foreach (JProperty property in dingdan.Properties())
            {
                createInfo.JObject[property.Name] = property.Value;
            }
        }

        private void MetadataManager_MetadataChanged(MetadataManager sender, MetadataChangingEventArgs args)
        {
            this.Tongbu(args.Metadata);
        }

        void LiuchengManager_LiuchengWanchenghou(Liucheng liucheng)
        {
            Metadata dingdanMetadata = this._moban.ColdewObject.MetadataManager.GetById(liucheng.BiaodanId);
            JObject dingdanJObject = dingdanMetadata.GetJObject(this.OrgManager.System);
            Dingdan dingdan = new Dingdan(dingdanJObject);
            dingdan.Zhuangtai = DingdanZhuangtai.wancheng;
            dingdanMetadata.SetValue(this.OrgManager.System, dingdan);
        }

        private void Tongbu(Metadata dingdanMetadata)
        {
            Dingdan dingdan = new Dingdan(dingdanMetadata.GetJObject(this.OrgManager.System));
            if (dingdan.Zhuangtai == DingdanZhuangtai.wancheng)
            {
                this.DeleteXiaoshouMingxi(dingdan);
                this.CreateXiaoshouMingxi(dingdan);
                this.DeleteShoukuanMingxi(dingdan);
                this.CreateShoukuanMingxi(dingdan);
            }
        }

        private void CreateXiaoshouMingxi(Dingdan dingdan)
        {
            ColdewObject xiaoshouMingxiObject = this.ObjectManager.GetObjectByCode("xiaoshouMingxi");
            foreach (JObject chanpinObject in dingdan.chanpinGrid)
            {
                JObject dingdanPropertys = new JObject();
                dingdanPropertys.Add("yewuyuan", dingdan["yewuyuan"]);
                dingdanPropertys.Add("kehu", dingdan["kehu"]);
                dingdanPropertys.Add("fahuoRiqi", dingdan["fahuoRiqi"]);
                dingdanPropertys.Add("fahuoDanhao", dingdan["fahuoDanhao"]);

                foreach (JProperty property in chanpinObject.Properties())
                {
                    dingdanPropertys.Add(property.Name, property.Value.ToString());
                }
                MetadataCreateInfo createInfo = new MetadataCreateInfo() { Creator = this.OrgManager.System, JObject = dingdanPropertys };
                xiaoshouMingxiObject.MetadataManager.Create(createInfo);
            }
        }

        private void DeleteXiaoshouMingxi(Dingdan dingdan)
        {
            ColdewObject xiaoshouMingxiObject = this.ObjectManager.GetObjectByCode("xiaoshouMingxi");
            List<MetadataFilter> searchers = new List<MetadataFilter>();
            List<FilterExpression> expressions = new List<FilterExpression>();
            Field fahuoDanhaoField = xiaoshouMingxiObject.GetFieldByCode("fahuoDanhao");
            expressions.Add(new StringFilterExpression(fahuoDanhaoField, dingdan.FahuoDanhao));
            MetadataFilter filter = new MetadataFilter(expressions);
            List<Metadata> metadatas = xiaoshouMingxiObject.MetadataManager.Search(this.OrgManager.System, searchers);
            foreach (Metadata metadata in metadatas)
            {
                metadata.Delete(this.OrgManager.System);
            }
        }

        private void CreateShoukuanMingxi(Dingdan dingdan)
        {
            ColdewObject shoukuanMingxiObject = this.ObjectManager.GetObjectByCode("shoukuanMingxi");
            foreach (JObject chanpinObject in dingdan.shoukuanGrid)
            {
                JObject dingdanPropertys = new JObject();
                dingdanPropertys.Add("yewuyuan", dingdan["yewuyuan"]);
                dingdanPropertys.Add("kehu", dingdan["kehu"]);
                dingdanPropertys.Add("fahuoRiqi", dingdan["fahuoRiqi"]);
                dingdanPropertys.Add("fahuoDanhao", dingdan["fahuoDanhao"]);

                foreach (JProperty property in chanpinObject.Properties())
                {
                    dingdanPropertys.Add(property.Name, property.Value.ToString());
                }
                MetadataCreateInfo createInfo = new MetadataCreateInfo() { Creator = this.OrgManager.System, JObject = dingdanPropertys };
                shoukuanMingxiObject.MetadataManager.Create(createInfo);
            }
        }

        private void DeleteShoukuanMingxi(Dingdan dingdan)
        {
            ColdewObject shoukuanMingxiObject = this.ObjectManager.GetObjectByCode("shoukuanMingxi");
            List<FilterExpression> expressions = new List<FilterExpression>();
            Field fahuoDanhaoField = shoukuanMingxiObject.GetFieldByCode("fahuoDanhao");
            expressions.Add(new StringFilterExpression(fahuoDanhaoField, dingdan.FahuoDanhao));
            MetadataFilter filter = new MetadataFilter(expressions);
            List<MetadataFilter> searchers = new List<MetadataFilter>();
            searchers.Add(filter);
            List<Metadata> metadatas = shoukuanMingxiObject.MetadataManager.Search(this.OrgManager.System, searchers);
            foreach (Metadata metadata in metadatas)
            {
                metadata.Delete(this.OrgManager.System);
            }
        }
    }
}
