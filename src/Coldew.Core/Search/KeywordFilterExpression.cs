using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class KeywordFilterExpression : FilterExpression
    {
        List<Regex> _keywordRegexs;
        public KeywordFilterExpression(string keyword)
        {
            this._keywordRegexs = RegexHelper.GetRegexes(keyword);
            this.Keyword = keyword;
        }

        public string Keyword { private set; get; }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            if (this._keywordRegexs.Any(regex => !regex.IsMatch(metadata.Content)))
            {
                return false;
            }
            
            return true;
        }
    }
}
