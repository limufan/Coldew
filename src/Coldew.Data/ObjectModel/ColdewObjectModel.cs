﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class ColdewObjectModel
    {
        public virtual string ID { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual bool IsSystem { set; get; }

        public virtual int Index { set; get; }

        public virtual string NameFieldId { set; get; }

        public virtual string FieldsJson { set; get; }

        public virtual string PermissionJson { set; get; }
    }
}
