using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Api;

namespace Coldew.Core.Permission
{
    public class ObjectPermission
    {
        public ObjectPermission(Member member, ObjectPermissionValue value)
        {
            this.Member = member;
            this.Value = value;
        }

        public Member Member { private set; get; }

        public ObjectPermissionValue Value { private set; get; }
    }
}
