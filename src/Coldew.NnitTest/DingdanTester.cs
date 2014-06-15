using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LittleOrange.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Coldew.NnitTest
{
    public class DingdanTester
    {
        [Test]
        public void Test()
        {
            string dingdanJson = @"{
    name: '201406001',
    fahuoRiqi: '2014-06-07T00:00:00',
    yewuyuan: 'lianglin',
    kehu: '佛山市凯迪电器有限公司',
    jiekuanFangshi: '2个月月结',
    chanpinGrid: [{'name':'绝缘漆','guige':'YJ-601B','danwei':'KG','shuliang':33,'tongshu':3,'xiaoshouDanjia':14,'xiaoshouDijia':12.7,'shijiDanjia':13.2,'zongjine':462,'yewulv':0.03,'yewulvFangshi':'按金额','yewufei':10.89,'shifouKaipiao':'是', 'butie': 20.31, 'ticheng': 10},{'name':'稀释剂','guige':'YX-115','danwei':'KG','shuliang':20,'tongshu':3,'xiaoshouDanjia':14,'xiaoshouDijia':12.7,'shijiDanjia':13.2,'zongjine':280,'yewulv':0.03,'yewulvFangshi':'按金额','yewufei':10.89,'shifouKaipiao':'是', 'butie': 20.31, 'ticheng': 10}],
    shoukuanGrid: [{shoukuanRiqi: '2014-06-15T00:00:00', shoukuanJine: 300}]
}";
            Dingdan dingdan = new Dingdan(JsonConvert.DeserializeObject<JObject>(dingdanJson));
            Assert.AreEqual(742, dingdan.yingshoukuanJine);
            Assert.AreEqual(300, dingdan.yishoukuanJine);
            Assert.AreEqual(742 - 300, dingdan.weishoukuanJine);
            Assert.AreEqual(27.60, Math.Round(dingdan.ticheng, 2));
            Assert.AreEqual(27.60, Math.Round(dingdan.chanpinGrid.Sum(x => x.ticheng), 2));
            Assert.AreEqual(300, Math.Round(dingdan.chanpinGrid.Sum(x => x.shoukuanJine), 2));
            Assert.AreEqual(27.60, Math.Round(dingdan.shoukuanGrid.Sum(x => x.ticheng), 2));
        }
    }
}
