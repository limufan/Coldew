﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data.Organization
{
    public class FunctionModel
    {
        public FunctionModel()
        {

        }

        public virtual string ID { get; set; }

        public virtual string Name { set; get; }

        public virtual string Url { set; get; }

        public virtual string IconClass { set; get; }

        public virtual int Sort { set; get; }

        public virtual string OwnerMemberIds { set; get; }
    }
}
