using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    public class UserListFieldCreateInfo : FieldCreateInfo
    {
        public UserListFieldCreateInfo(string code, string name)
            :base(code, name)
        {
            
        }

        public bool DefaultValueIsCurrent{set;get;} 
    }
}
