using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.UI;
using Coldew.Core.Organization;
using Coldew.Data;

using Newtonsoft.Json;

namespace Coldew.Core.UI
{
    public class Form
    {
        FormDataService _dataService;
        public Form(string id, string code, string title, List<Control> controls, List<RelatedObject> relateds, FormDataService dataService)
        {
            this.ID = id;
            this.Code = code;
            this.Title = title;
            this.Controls = controls;
            this.Relateds = relateds;
            if (this.Relateds == null)
            {
                this.Relateds = new List<RelatedObject>();
            }
            this._dataService = dataService;
        }

        public string ID { private set; get; }

        public string Code { private set; get; }

        public string Title {private set; get; }

        public List<Control> Controls {private set; get; }

        public List<RelatedObject> Relateds { private set; get; }

        public void ClearFieldData(Field field)
        {
            this.Relateds.ForEach(x => x.ClearFieldData(field));

            this._dataService.Update(this);
        }
    }
}
