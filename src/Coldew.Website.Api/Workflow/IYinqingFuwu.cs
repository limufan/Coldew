using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api.Workflow
{
    public interface IYinqingFuwu
    {
        List<LiuchengMobanModel> GetLiuchengMobanByYonghu(string yonghu);
        List<LiuchengMobanModel> GetSuoyouLiuchengMoban();
    }
}
