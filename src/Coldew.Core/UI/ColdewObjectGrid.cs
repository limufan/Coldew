using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json.Linq;

namespace Coldew.Core.UI
{
    public class ColdewObjectGrid : Grid
    {
        public ColdewObject ColdewObject { set; get; }

        public override void FillJObject(Metadata metadata, User user, JObject jobject)
        {
            List<Metadata> relatedMetadatas = this.ColdewObject.MetadataManager.GetRelatedList(metadata, "");
            JArray relatedMetadataJArray = new JArray(relatedMetadatas.Select(m => m.GetJObject(user)).ToList());
            jobject.Add(this.ColdewObject.Code, relatedMetadataJArray);
        }
    }
}
