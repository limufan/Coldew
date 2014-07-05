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
    public class LiuchengInitializer
    {
        public ColdewObject cobject;
        public LittleOrangeManager _coldewManager;
        public LittleOrangeInitializer _littleOrangeInitializer;
        public Field nameField;
        public Field chulirenField;
        public Field zhuangtaiMingchengField;
        public Field kaishiShijianField;
        public Field wanchengShijianField;
        public Field wanchengShuomingField;
        public LiuchengInitializer(LittleOrangeInitializer littleOrangeInitializer)
        {
            this._coldewManager = littleOrangeInitializer.ColdewManager;
            this._littleOrangeInitializer = littleOrangeInitializer;
        }

        public void Initialize()
        {
            this.InitObject();
        }

        private void InitObject()
        {
            cobject = this._coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo("流程", "liucheng", true));
            nameField = cobject.CreateStringField(new StringFieldCreateInfo("buzhou", "步骤名") {  GridWidth = 100 });
            cobject.SetNameField(nameField);
            chulirenField = cobject.CreateStringField(new StringFieldCreateInfo("chuliren", "处理人"));
            zhuangtaiMingchengField = cobject.CreateStringField(new StringFieldCreateInfo("zhuangtai", "状态"));
            kaishiShijianField = cobject.CreateStringField(new StringFieldCreateInfo("kaishiShijian", "开始时间") { GridWidth = 150 });
            wanchengShijianField = cobject.CreateStringField(new StringFieldCreateInfo("wanchengShijian", "完成时间") { GridWidth = 150 });
            wanchengShuomingField = cobject.CreateStringField(new StringFieldCreateInfo("wanchengShuoming", "备注") { GridWidth = 200 });
            cobject.ObjectPermission.Create(this._coldewManager.OrgManager.Everyone, ObjectPermissionValue.None);
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this._coldewManager.OrgManager.Everyone), MetadataPermissionValue.All, null);
        }
    }
}
