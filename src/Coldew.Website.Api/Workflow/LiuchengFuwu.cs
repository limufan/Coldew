using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Workflow;
using Coldew.Core;
using Coldew.Core.Organization;

using Coldew.Website.Api.Models;
using Coldew.Workflow;

namespace Coldew.Website.Api.Workflow
{
    public class LiuchengFuwu : ILiuchengFuwu
    {
        LiuchengYinqing _yinqing;
        ColdewManager _coldewManger;

        public LiuchengFuwu(LiuchengYinqing yinqing)
        {
            this._yinqing = yinqing;
            this._coldewManger = yinqing.ColdewManager;
        }

        public List<LiuchengModel> GetLiuchengXinxiList(string liuchengMobanId, ShijianFanwei faqiShijianFanwei, ShijianFanwei jieshuShijianFanwei, string zhaiyao, int start, int size, out int count)
        {
            List<Liucheng> liuchengList = this._yinqing.LiuchengManager.GetLiuchengList(liuchengMobanId, faqiShijianFanwei, jieshuShijianFanwei, zhaiyao, start, size, out count);
            return liuchengList.Select(x => new LiuchengModel(x)).ToList();
        }

        public LiuchengModel FaqiLiucheng(string liuchengMobanId, string code, string name, string shuoming, string faqirenAccount, bool jinjide, string zhaiyao, string biaodanId)
        {
            User user = this._coldewManger.OrgManager.UserManager.GetUserByAccount(faqirenAccount);
            LiuchengMoban moban = this._yinqing.LiuchengMobanManager.GetMobanById(liuchengMobanId);
            Metadata biaodan = moban.ColdewObject.MetadataManager.GetById(biaodanId);

            Liucheng liucheng = this._yinqing.LiuchengManager.FaqiLiucheng(user, liuchengMobanId, zhaiyao, jinjide, biaodan);
            Xingdong xingdong = liucheng.ChuangjianXingdong(code, name, zhaiyao, null);
            Renwu renwu = xingdong.ChuangjianRenwu(user);
            renwu.Wancheng(user, shuoming);
            xingdong.Wancheng();
            return new LiuchengModel(liucheng);
        }

        public LiuchengModel GetLiucheng(string liuchengId)
        {
            Liucheng liucheng = this._yinqing.LiuchengManager.GetLiucheng(liuchengId);
            return new LiuchengModel(liucheng);
        }

        public void Wancheng(string liuchengId)
        {
            Liucheng liucheng = this._yinqing.LiuchengManager.GetLiucheng(liuchengId);
            liucheng.Wancheng();
        }
    }
}
