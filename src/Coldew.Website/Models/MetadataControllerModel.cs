using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coldew.Website.Models
{
    public enum MetadataActionType
    {
        Create,
        Edit,
        Details
    }

    public class MetadataControllerModel
    {
        public string ObjectCode { set; get; }

        public string ControllerName { set; get; }

        public MetadataActionType ActionType { set; get; }

        public string ActionName { set; get; }
    }
}