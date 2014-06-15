using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.Organization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LittleOrange.Core
{
    public class DingdanLiuchengBiaodan
    {
        public DingdanLiuchengBiaodan(Metadata biaodan)
        {
            this.ChuhuoDanhao = biaodan.Name;
            this.Yewuyuan = biaodan.GetProperty("yewuyuan").Value.Value;
            this.Kehu = biaodan.GetProperty("kehu").Value.Value;
            this.FahuoRiqi = biaodan.GetProperty("fahuoRiqi").Value.Value;
            this.JiekuanFangshi = biaodan.GetProperty("jiekuanFangshi").Value.Value;
            string chanpinJson = biaodan.GetProperty("chanpinGrid").Value.Value;
            List<JObject> chanpinObjects = JsonConvert.DeserializeObject<List<JObject>>(chanpinJson);
            this.chanpinList = chanpinObjects.Select(x => new Chanpin(x)).ToList();
        }

        public string ChuhuoDanhao { private set; get; }

        public User Yewuyuan { private set; get; }

        public string Kehu { private set; get; }

        public DateTime FahuoRiqi { private set; get; }

        public string JiekuanFangshi { private set; get; }

        public List<Chanpin> chanpinList { private set; get; }
    }
}
