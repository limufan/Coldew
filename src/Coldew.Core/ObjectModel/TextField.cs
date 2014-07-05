﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

namespace Coldew.Core
{
    public class TextField : StringField
    {
        internal TextField()
        {
        }

        public override string TypeName
        {
            get { return "长文本"; }
        }
    }
}
