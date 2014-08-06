using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Coldew.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;
using Coldew.Core.Search;


namespace Coldew.Core
{
    public class MetadataFilter
    {
        public MetadataFilter(List<FilterExpression> expressions)
        {
            this.Expressions = expressions;
        }

        public List<FilterExpression> Expressions { private set; get; }

        public bool Accord(User opUser, Metadata metadata)
        {
            foreach (FilterExpression expression in this.Expressions)
            {
                if (!expression.IsTrue(opUser, metadata))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
