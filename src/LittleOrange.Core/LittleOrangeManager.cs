using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Workflow;

namespace LittleOrange.Core
{
    public class LittleOrangeManager : ColdewManager
    {
        DingdanManager _dingdanManager;

        public LittleOrangeManager()
        {
        }

        protected override void Load()
        {
            base.Load();
            LiuchengMoban moban = this.LiuchengYinqing.LiuchengMobanManager.GetMobanByCode("FahuoLiucheng");
            this.LiuchengYinqing.LiuchengManager.LiuchengWanchenghou += LiuchengManager_LiuchengWanchenghou;
            this._dingdanManager = new DingdanManager(this);
        }

        void LiuchengManager_LiuchengWanchenghou(Liucheng liucheng)
        {
            ColdewObject liuchengObject = this.ObjectManager.GetObjectByCode("FahuoLiucheng");
            Metadata biaodanMetadata = liuchengObject.MetadataManager.GetById(liucheng.BiaodanId);
            _dingdanManager.CreateDingdan(biaodanMetadata.MapJObject(this.OrgManager.System));
        }
    }
}
