using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Workflow;

namespace Coldew.Api
{
    public class EnumMapper
    {
        public static string Map(RenwuZhuangtai zhuangtai)
        {
            switch (zhuangtai)
            {
                case RenwuZhuangtai.Chulizhong: return "处理中";
                case RenwuZhuangtai.Wanchengle: return "已完成";
            }
            return "";
        }
    }
}
