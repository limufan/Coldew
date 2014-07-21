using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api.Workflow
{
    public interface IRenwuFuwu
    {
        List<RenwuModel> GetChulizhongdeRenwu(string chulirenZhanghao, string mobanId, DateTime? kaishiShijian, DateTime? jieshuShijian, string zhaiyao, int start, int size, out int count);

        List<RenwuModel> GetZhipaideRenwu(string zhipairen, int start, int size, out int count);

        void SetJianglaiZhipai(string zhipairenZhanghao, string dailirenZhanghao, DateTime? kaishiShijian, DateTime? jieshuShijian);

        JianglaiZhipaiModel GetJianglaiZhipai(string zhipairenZhanghao);

        void XiugaiRenwuChuliren(string renwuId, string chulirenZhanghao);

        void ZhipaiRenwu(string[] renwuId, string dailirenZhanghao);

        void ZhipaiSuoyouRenwu(string zhipairenZhanghao, string dailirenZhanghao);

        void QuxiaoZhipai(string[] renwuId);

        List<RenwuModel> GetWanchengdeRenwu(string chulirenZhanghao, string mobanId, DateTime? wanchengKaishiShijian, DateTime? wanchengJieshuShijian, string zhaiyao, int start, int size, out int count);

        List<RenwuModel> GetGuidangdeRenwu(string chulirenZhanghao, string mobanId, DateTime? wanchengKaishiShijian, DateTime? wanchengJieshuShijian, string zhaiyao, int start, int size, out int count);

        RenwuModel GetRenwu(string liuchengId, string renwuId);

        List<RenwuModel> GetLiuchengRenwu(string liuchengId);

        RenwuModel WanchengRenwu(string liuchengId, string chulirenAccount, string renwuId, string shuoming);

        void WanchengXingdong(string liuchengId, string xingdongId);

        void ChuangjianXingdong(string liuchengId, string code, string name, List<string> chulirenAccounts, string zhaiyao, DateTime? qiwangWanchengShijian);
    }
}
