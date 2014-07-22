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
            this._departments = new List<Department>();
            this._groupUsers = new List<User>();
            this._positions = new List<Position>();
            this._groups = new List<Group>();
            this._orgMnger = orgMnger;
        }

        private OrganizationManagement _orgMnger;

        private bool _memberLoaded;

        /// <summary>
        /// 组名称
        /// </summary>
        public virtual string Name { get; protected set; }

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

        private List<User> _groupUsers;
        internal List<User> _GroupUsers
        {
            get
            {
                return _groupUsers;
            }
        }

        public ReadOnlyCollection<User> GroupUsers
        {
            get
            {
                return this._GroupUsers
                    .Where(x => x.Status != UserStatus.Logoff)
                    .ToList()
                    .AsReadOnly();
            }
        }

        private List<Department> _departments;
        internal List<Department> _Departments
        {
            get
            {
                return _departments;
            }
        }

        public ReadOnlyCollection<Department> Departments
        {
            get
            {
                return this._Departments.AsReadOnly();
            }
        }

        private List<Position> _positions;
        internal List<Position> _Positions
        {
            get
            {
                return _positions;
            }
        }

        public ReadOnlyCollection<Position> Positions
        {
            get
            {
                return this._Positions.AsReadOnly();
            }
        }

        private List<Group> _groups;
        internal List<Group> _Groups
        {
            get
            {
                return _groups;
            }
        }

        public ReadOnlyCollection<Group> Groups
        {
            get
            {
                return this._Groups.AsReadOnly();
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

        public virtual ReadOnlyCollection<User> Users
        {
            get
            {
                List<User> users = new List<User>();
                var departmentUsers = this._Departments.SelectMany(x => x.Users);
                users.AddRange(departmentUsers);
                var positionUsers = this._Positions.SelectMany(x => x.Users);
                users.AddRange(positionUsers);
                var groupUsers = this._Groups.SelectMany(x => x.Users);
                users.AddRange(groupUsers);
                users.AddRange(this._GroupUsers.Where(x => x.Status != UserStatus.Logoff));

                return users.Distinct().ToList().AsReadOnly();
            }
        }

        private object _updateLockObject = new object();

        public virtual void AddUser(User operationUser, User user)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            if (this._GroupUsers.Contains(user))
            {
                throw new ContainsUserException(user.Name);
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = user.ID, MemberName = user.Name, MemberType = MemberType.User };

            lock (_updateLockObject)
            {
                List<User> users = this._GroupUsers.ToList();
                users.Add(user);
                this._groupUsers = users;

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

        public virtual void RemoveUser(User operationUser, User user)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = user.ID, MemberName = user.Name, MemberType = MemberType.User };
            if (this._GroupUsers.Contains(user))
            {
                lock (_updateLockObject)
                {
                    List<User> users = this._GroupUsers.ToList();
                    users.Remove(user);
                    this._groupUsers = users;

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

        public virtual void AddDepartment(User operationUser, Department department)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (department == null)
            {
                throw new ArgumentNullException("department");
            }
            if (this._Departments.Contains(department))
            {
                throw new ContainsDepartmentException(department.Name);
            }

            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = department.ID, MemberName = department.Name, MemberType = MemberType.Department };

            lock (_updateLockObject)
            {
                List<Department> departments = this._Departments.ToList();
                departments.Add(department);
                this._departments = departments;

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

        public virtual void RemoveDepartment(User operationUser, Department department)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (department == null)
            {
                throw new ArgumentNullException("department");
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = department.ID, MemberName = department.Name, MemberType = MemberType.Department };
           
            if (this._Departments.Contains(department))
            {
                lock (_updateLockObject)
                {
                    List<Department> departments = this._Departments.ToList();
                    departments.Remove(department);
                    this._departments = departments;

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

        public virtual void AddPosition(User operationUser, Position position)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }
            if (this._Positions.Contains(position))
            {
                throw new ContainsPositionException(position.Name);
            }

            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = position.ID, MemberName = position.Name, MemberType = MemberType.Position };

            lock (_updateLockObject)
            {
                List<Position> positions = this._Positions.ToList();
                positions.Add(position);
                this._positions = positions;

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

        public virtual void RemovePoisition(User operationUser, Position position)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = position.ID, MemberName = position.Name, MemberType = MemberType.Position };
            
            if (this._Positions.Contains(position))
            {
                lock (_updateLockObject)
                {
                    List<Position> positions = this._Positions.ToList();
                    positions.Remove(position);
                    this._positions = positions;

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

        public virtual void AddGroup(User operationUser, Group group)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            if (group == this)
            {
                throw new ContainsSelfGroupException();
            }
            if (this._Groups.Contains(group))
            {
                throw new ContainsGroupException(group.Name);
            }
            if (group.InGroup(this))
            {
                throw new ContainsCircleGroupException(this.Name, group.Name);
            }

            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = group.ID, MemberName = group.Name, MemberType = MemberType.Group };

            lock (_updateLockObject)
            {
                List<Group> groups = this._Groups.ToList();
                groups.Add(group);
                this._groups = groups;

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

        public virtual void RemoveGroup(User operationUser, Group group)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            GroupMemberInfo memberInfo = new GroupMemberInfo { GroupId = this.ID, MemberId = group.ID, MemberName = group.Name, MemberType = MemberType.Group };
            if (this._Groups.Contains(group))
            {
                lock (_updateLockObject)
                {
                    List<Group> groups = this._Groups.ToList();
                    groups.Remove(group);
                    this._groups = groups;

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
            List<User> users = this._GroupUsers.ToList();
            users.ForEach(x => this.RemoveUser(operationUser, x));

            List<Department> departments = this._Departments.ToList();
            departments.ForEach(x => this.RemoveDepartment(operationUser, x));

            List<Position> positions = this._Positions.ToList();
            positions.ForEach(x => this.RemovePoisition(operationUser, x));

            List<Group> groups = this._Groups.ToList();
            groups.ForEach(x => this.RemoveGroup(operationUser, x));
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

        public virtual bool InGroup(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (Users.Contains(user))
            {
                return true;
            }
            return false;
        }

        public virtual bool InGroup(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (this._Groups.Exists(x => x == group || x.InGroup(group)))
            {
                return true;
            }
            return false;
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
            return this.Users.ToList();
        }

        public override List<Member> GetParents()
        {
            return new List<Member>();
        }

        public override List<Member> GetChildren()
        {
            return new List<Member>();
        }

        public override bool Contains(User user)
        {
            return this.Users.Contains(user);
        }
    }
}
