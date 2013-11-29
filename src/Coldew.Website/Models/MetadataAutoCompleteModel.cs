using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api;

namespace Coldew.Website.Models
{
    public class MetadataAutoCompleteModel
    {
        public MetadataAutoCompleteModel(MetadataInfo metadataInfo)
        {
            this.id = metadataInfo.ID;
            this.name = metadataInfo.Name;
            this.summary = metadataInfo.Summary;
        }

        public string id;
        public string name;
        public string summary;
    }
}