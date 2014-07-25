using System;
using System.Linq;
using System.Linq.Expressions;
using Coldew.Api.Organization.Exceptions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Coldew.Data.Organization;
using Coldew.Api.Organization;


namespace Coldew.Core.Organization
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class Group : Member
    {
        public Group(string id, string name, DateTime createTime, User creator, string remark, GroupType type, OrganizationManagement orgMnger)
        {

            this.ID = id;
            this.Name = name;
            this.CreateTime = createTime;
            this.Creator = creator;
            this.Remark = remark;
            this.GroupType = type;
            this._Members = new List<Member>();
            this._orgMnger = orgMnger;
        }

        private OrganizationManagement _orgMnger;

        /// <summary>
        /// 是否个人用户组
        /// </summary>
        public virtual Boolean IsPersonal
        {
            get
            {
                return this.GroupType == GroupType.Personal;
            }
        }

        public virtual Boolean IsSystem
        {
            get
            {
                return this.GroupType == GroupType.System;
            }
        }

        public virtual GroupType GroupType { get; protected set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; protected set; }

        internal List<Member> _Members { set; get; }

        public ReadOnlyCollection<Member> Members
        {
            get
            {
                return this._Members.AsReadOnly();
            }
        }

        public virtual DateTime CreateTime { get; private set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual User Creator { get; private set; }

        public override MemberType Type
        {
            get { return MemberType.Group; }
        }

        /// <summary>
        /// 修改信息之前
        /// </summary>
        public virtual event TEventHandler<Group, GroupChangeInfo> Changing;

        /// <summary>
        /// 修改信息之后
        /// </summary>
        public virtual event TEventHandler<Group, GroupChangeInfo> Changed;

        public virtual event TEventHandler<Group, ChangeEventArgs<GroupMemberInfo, Group>> AddedMember;

        public virtual event TEventHandler<Group, ChangeEventArgs<GroupMemberInfo, Group>> RemovedMember;

        private object _updateLockObject = new object();

        public virtual void AddMember(User operationUser, Member member)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (member == null)
            {
                throw new ArgumentNullException("user");
            }
            
            if (this._Members.Contains(member))
            {
                throw new ContainsUserException(member.Name);
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = member.ID, MemberName = member.Name, MemberType = member.Type };

            lock (_updateLockObject)
            {
                List<Member> members = this._Members.ToList();
                members.Add(member);
                this._Members = members;

                if (this.AddedMember != null)
                {
                    ChangeEventArgs<GroupMemberInfo, Group> args = new ChangeEventArgs<GroupMemberInfo, Group>
                    {
                        
                        ChangeObject = this,
                        ChangeInfo = memberInfo,
                        Operator = operationUser
                    };
                    this.AddedMember(this, args);
                }
            }
        }

        public virtual void RemoveMember(User operationUser, Member member)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (member == null)
            {
                throw new ArgumentNullException("user");
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = member.ID, MemberName = member.Name, MemberType = MemberType.User };
            if (this._Members.Contains(member))
            {
                lock (_updateLockObject)
                {
                    List<Member> members = this._Members.ToList();
                    members.Remove(member);
                    this._Members = members;

                    if (RemovedMember != null)
                    {
                        ChangeEventArgs<GroupMemberInfo, Group> args = new ChangeEventArgs<GroupMemberInfo, Group>
                        {
                            
                            ChangeObject = this,
                            ChangeInfo = memberInfo,
                            Operator = operationUser
                        };
                        this.RemovedMember(this, args);
                    }
                }
            }
        }

        public virtual void ClearMembers(User operationUser)
        {
            List<Member> memberList = this._Members.ToList();
            memberList.ForEach(m => this.RemoveMember(operationUser, m));
        }

        public virtual void Change(User operationUser, GroupChangeInfo changeInfo)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (changeInfo == null)
            {
                throw new ArgumentNullException("groupInfo");
            }
            if (this.IsSystem)
            {
                throw new SystemGroupCannotModifyException();
            }
            if (string.IsNullOrWhiteSpace(changeInfo.Name))
            {
                throw new ArgumentNullException("groupInfo.Name");
            }

            if (changeInfo.Name != this.Name)
            {
                if (this._orgMnger.GroupManager.Groups.Any(x => x.GroupType != GroupType.Personal && x.Name == changeInfo.Name))
                {
                    throw new GroupNameReapeatException();
                }
            }

            if (this.Changing != null)
            {
                this.Changing(this, changeInfo);
            }

            this.Name = changeInfo.Name;
            this.Remark = changeInfo.Remark;

            if (this.Changed != null)
            {

                this.Changed(this, changeInfo);
            }
        }

        public virtual GroupInfo MapGroupInfo()
        {
            return new GroupInfo
            {
                CreatorId = this.Creator.ID,
                CreatorName = this.Creator.Name,
                ID = this.ID,
                IsPersonal = this.IsPersonal,
                GroupType = this.GroupType,
                Name = this.Name,
                Remark = this.Remark,
                IsSystem = this.IsSystem,
                CreateTime = this.CreateTime
            };
        }

        public override List<User> GetUsers(bool recursive)
        {
            return this._Members.Where(m => m is User).Select(m => m as User).ToList();
        }

        public override List<Member> GetParents()
        {
            return new List<Member>();
        }

        public override List<Member> GetChildren()
        {
            return this._Members.ToList();
        }

        public override bool Contains(Member user)
        {
            return this._Members.Contains(user);
        }
    }
}
