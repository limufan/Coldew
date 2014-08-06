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
    public class YinqingFuwu : IYinqingFuwu
    {
        LiuchengYinqing _yingqing;

        public YinqingFuwu(LiuchengYinqing yingqing)
        {
            this._yingqing = yingqing;
        }

        public List<LiuchengMobanModel> GetLiuchengMobanByYonghu(string yonghuZhanghao)
        {
            User yonghu = this._yingqing.GetYonghu(yonghuZhanghao);
            return this._yingqing.LiuchengMobanManager.GetAllMoban().Where(x => x.NengFaqi(yonghu)).Select(x => new LiuchengMobanModel(x)).ToList();
        }

        public List<LiuchengMobanModel> GetSuoyouLiuchengMoban()
        {
            return this._yingqing.LiuchengMobanManager.GetAllMoban().Select(x => new LiuchengMobanModel(x)).ToList();
        }
    }
}
