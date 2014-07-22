using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Coldew.Api.Organization.Exceptions;

using Coldew.Data.Organization;
using Coldew.Api.Organization;



namespace Coldew.Core.Organization
{
    public class Position : Member
    {
        public Position(string id, string name, string parentId, string remark, OrganizationManagement orgMnger)
        {
            this.ID = id;
            this.Name = name;
            this.ParentId = parentId;
            this.Remark = remark;

            this._orgMnger = orgMnger;
            this._users = new List<User>();
        }

        OrganizationManagement _orgMnger;

        /// <summary>
        /// 职位名称
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// 上级职位ID
        /// </summary>
        private string ParentId { get; set; }

        private Position _parent;
        /// <summary>
        /// 上级职位
        /// </summary>
        public virtual Position Parent
        {
            get
            {
                if (_parent == null)
                {
                    

                    _parent = this._orgMnger.PositionManager.Positions.FirstOrDefault(x => x.ID == ParentId);
                }
                return _parent;
            }
            private set
            {
                _parent = value;
            }
        }


        /// <summary>
        /// 职位类型
        /// </summary>
        public virtual OrganizationType PositionType { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; private set; }

        /// <summary>
        /// 修改信息之前
        /// </summary>
        public virtual event TEventHandler<Position, PositionChangeInfo> Changing;

        /// <summary>
        /// 修改信息之后
        /// </summary>
        public virtual event TEventHandler<Position, PositionChangeInfo> Changed;

        public virtual Department Department
        {
            get
            {
                

                Department department = this._orgMnger.DepartmentManager.Departments
                        .FirstOrDefault(x => x.ManagerPosition == this);
                if (department == null && Parent != null)
                {
                    return Parent.Department;
                }
                return department;
            }
        }

        public virtual ReadOnlyCollection<Department> ManagerialDepartments
        {
            get
            {
                return SelfManagerialDepartments
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<Department> SelfManagerialDepartments
        {
            get
            {
                

                return this.Children
                    .Where(p => p.PositionType == OrganizationType.ManagerPosition)
                    .Select(p => p.Department)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public virtual bool IsMySuperior(Position position, bool recursive)
        {
            if (this.Parent != null)
            {
                if (this.Parent == position)
                {
                    return true;
                }
                if (recursive)
                {
                    bool isMyParentSupperior = this.Parent.IsMySuperior(position, recursive);
                    if (isMyParentSupperior)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual ReadOnlyCollection<Position> NormalChildren
        {
            get
            {
                return Children
                    .Where(x => x.PositionType == OrganizationType.Position)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<Position> Children
        {
            get
            {
                

                return this._orgMnger.PositionManager.Positions.
                        Where(x => x.Parent == this)
                        .ToList()
                        .AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<Department> DepartmentChildren
        {
            get
            {
                return this._orgMnger
                    .DepartmentManager
                    .Departments
                    .Where(x => x.PositionParent == this)
                    .ToList()
                    .AsReadOnly();
            }
        }

        private List<User> _users;
        internal void AddUser(List<User> users)
        {
            this._users.AddRange(users);
        }

        public ReadOnlyCollection<User> Users
        {
            get
            {
                return this._users.AsReadOnly();
            }
        }

        public event TEventHandler<Position, User> AddedUser;

        public void AddUser(User opUser, User user)
        {
            this._users.Add(user);
            if (this.AddedUser != null)
            {
                this.AddedUser(this, user);
            }
        }

        public event TEventHandler<Position, User> RemovedUser;

        public void RemoveUser(User opUser, User user)
        {
            this._users.Remove(user);
            if (this.RemovedUser != null)
            {
                this.RemovedUser(this, user);
            }
        }

        private object _updateLockObject = new object();

        public virtual bool InPosition(User user, bool recursive)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            } 
            
            bool contains = this.Users.Contains(user);
            if (!contains && recursive)
            {
                contains = this.Children.Any(x => x.InPosition(user, recursive));
            }
            return contains;
        }

        public virtual void Change(User operationUser, PositionChangeInfo changeInfo)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (string.IsNullOrWhiteSpace(changeInfo.Name))
            {
                throw new ArgumentNullException("name");
            }
            if (changeInfo.ParentId == this.ID)
            {
                throw new PositionParentCannotSelfException();
            }

            if (this.Parent != null && changeInfo.Name != this.Name)
            {
                Position position = this.Parent.Children.FirstOrDefault(x => x.Name == changeInfo.Name);
                if (position != null)
                {
                    throw new PositionNameReapeatException();
                }
            }

            if (this.Changing != null)
            {
                this.Changing(this, changeInfo);
            }

            this.Name = changeInfo.Name;
            this.Remark = changeInfo.Remark;
            this.ParentId = changeInfo.ParentId;
            this.Parent = null;

            if (this.Changed != null)
            {
                
                this.Changed(this, changeInfo);
            }
        }

        public virtual bool SelfChildrenHasUser()
        {
            return this.Children.Any(x => x.Users.Count > 0 || x.SelfChildrenHasUser());
        }

        public PositionInfo MapPositionInfo()
        {
            return new PositionInfo 
            { 
                ID = this.ID,
                Name = this.Name,
                ParentId = this.ParentId,
                PositionType = this.PositionType,
                Remark = this.Remark,
                DepartmentId = Department == null ? null : Department.ID,
                HaveChildren=this.Children.Count>0?true:false
            };
        }

        public override MemberType Type
        {
            get { return MemberType.Position; }
        }

        public override List<Member> GetParents()
        {
            if (this.Parent != null)
            {
                return new List<Member> { this.Parent };
            }
            return new List<Member>();
        }

        public override List<Member> GetChildren()
        {
            List<Member> members = new List<Member>();

            foreach (Position position in this.Children)
            {
                members.Add(position);
            }

            return members;
        }

        public override List<User> GetUsers(bool recursive)
        {
            List<User> users = this.Users.ToList();
            if (recursive)
            {
                users.AddRange(this.Children.SelectMany(x => x.Users).ToList());
            }
            return users.Distinct().ToList();
        }

        public override bool Contains(User user)
        {
            return this.Users.Contains(user);
        }
    }
}
