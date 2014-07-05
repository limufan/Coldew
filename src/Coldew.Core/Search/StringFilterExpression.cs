using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class StringFilterExpression : FilterExpression
    {
        List<Regex> _keywordRegexs;
        public StringFilterExpression(Field field, string keyword)
        {
            this._keywordRegexs = RegexHelper.GetRegexes(keyword);
            this.Field = field;
            this.Keyword = keyword;
        }

        public Field Field { private set; get; }

        public string Keyword { private set; get; }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }
            MetadataProperty property = metadata.GetProperty(this.Field.Code);
            if (property != null)
            {
                if (this._keywordRegexs.Any(regex => !regex.IsMatch(property.Value.ShowValue)))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
