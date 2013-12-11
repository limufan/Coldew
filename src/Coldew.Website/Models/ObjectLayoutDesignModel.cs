using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coldew.Api;
using Coldew.Api.UI;

namespace Coldew.Website.Models
{
    public class ObjectLayoutDesignModel
    {
        public List<FieldModel> fields;
        public List<SectionModel> sections;
    }

    public class SectionModel
    {
        public string name ;

        public int columnCount ;

        public List<FieldModel> fields;
    }

    public class SectionSaveModel
    {
        public string name;

        public int columnCount;

        public List<string> fields;
    }
}