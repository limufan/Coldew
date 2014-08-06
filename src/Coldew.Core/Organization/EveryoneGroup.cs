using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using Coldew.Api.Organization;

namespace Coldew.Core.Organization
{
    public class EveryoneGroup : Member
    {
        OrganizationManagement _orgManager;
        public EveryoneGroup(OrganizationManagement orgManager)
        {
            this.ID = "Everyone";
            this.Name = "Everyone";
            this._orgManager = orgManager;
        }


        public override MemberType Type
        {
            get { return MemberType.Group; }
        }

        public override List<Member> GetParents()
        {
            return new List<Member>();
        }

        public override List<Member> GetChildren()
        {
            return new List<Member>();
        }

        public override List<User> GetUsers(bool recursive)
        {
            return this._orgManager.UserManager.Users.ToList();
        }

        public override bool Contains(Member user)
        {
            return true;
        }
    }
}
