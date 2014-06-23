using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api;
using Coldew.Core;

namespace Coldew.Website.Api.Models
{
    public class GridViewGridModel
    {
        public GridViewGridModel(GridView view)
        {
            this.id = view.ID;
            this.name = view.Name;
            this.isShared = view.IsShared ? "是" : "否";
        }

        public string id ;

        public string editLink ;

        public string name ;

        public string isShared;
    }
}