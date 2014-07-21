using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Workflow;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api.Workflow
{
    public interface ILiuchengFuwu
    {
        List<LiuchengModel> GetLiuchengXinxiList(string liuchengMobanId, ShijianFanwei faqiShijianFanwei, ShijianFanwei jieshuShijianFanwei, string zhaiyao, int start, int size, out int count);
        LiuchengModel FaqiLiucheng(string liuchengMobanId, string code, string name, string shuoming, string faqirenAccount, bool jinjide, string zhaiyao, string biaodanId);
        LiuchengModel GetLiucheng(string liuchengId);
        void Wancheng(string liuchengId);
    }
}
