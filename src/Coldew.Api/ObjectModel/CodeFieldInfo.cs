﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Api
{
    [Serializable]
    public class CodeFieldInfo : FieldInfo
    {
        public string Format { set; get; }
    }
}
