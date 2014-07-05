using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public abstract class FilterExpression
    {
        public abstract bool IsTrue(User opUser, Metadata metadata);
    }
}
