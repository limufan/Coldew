using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coldew.Api.Organization.Exceptions;
using System.Linq;

using System.Text.RegularExpressions;
using Coldew.Data.Organization;

using NHibernate.Criterion;
using Coldew.Api.Organization;


namespace Coldew.Core.Organization
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : Member
    {
        internal OrganizationManagement OrgManager { set; get; }

        public User(string id, string name, string account, string password, string email, UserGender gender, UserRole role, 
            UserStatus status, DateTime? lastLoginTime, string lastLoginIp, string remark, string mainPositionId, OrganizationManagement orgMnger)
        {
            this.OrgManager = orgMnger;
            this.ID = id;
            this.Name = name;
            this.Account = account;
            this.Password = password;
            this.Email = email;
            this.Gender = gender;
            this.Role = role;
            this.Status = status;
            this.LastLoginTime = lastLoginTime;
            this.LastLoginIp = lastLoginIp;
            this.Remark = remark;
            this._mainPositionId = mainPositionId;
        }

        public User()
        {

        }

        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string Account { get; internal set; }

        /// <summary>
        /// 密码
        /// </summary>
        public virtual string Password { get; internal set; }

        /// <summary>
        /// Email
        /// </summary>
        public virtual string Email { get; internal set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public virtual string Name { get; internal set; }

        /// <summary>
        /// 性别
        /// </summary>
        public virtual UserGender Gender { get; internal set; }

        public virtual UserRole Role { get; internal set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public virtual UserStatus Status { get; internal set; }

        private string _mainPositionId;
        private Position _mainPosition;
        public virtual Position MainPosition
        {
            set
            {
                _mainPosition = value;
            }
            get
            {
                if (_mainPosition == null)
                {
                    _mainPosition = this.OrgManager.PositionManager.GetPositionById(_mainPositionId);
                }
                return _mainPosition;
            }
        }

        public virtual List<Position> Positions
        {
            get
            {
                return this.OrgManager.PositionManager.Positions
                    .Where(x => x.Contains(this)).ToList();
            }
        }

        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        public virtual DateTime? LastLoginTime { get; internal set; }

        /// <summary>
        /// 最后一次登录IP
        /// </summary>
        public virtual string LastLoginIp { get; internal set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; internal set; }

        /// <summary>
        /// 修改用户信息之后
        /// </summary>
        public virtual event TEventHandler<User, UserChangeInfo> Changing;
        public virtual event TEventHandler<User, UserChangeInfo> Changed;
        public virtual event TEventHandler<User, UserChangeInfo> Logoffed;
        public virtual event TEventHandler<User, UserChangeInfo> Locked;
        public virtual event TEventHandler<User, UserChangeInfo> Activated;
        public virtual event TEventHandler<User, UserChangeInfo> PasswordReseted;

        public virtual void Change(User operationUser, UserChangeInfo changeInfo)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (string.IsNullOrEmpty(changeInfo.Name))
            {
                throw new UserNameEmptyException();
            }
            
            if (this.Changing != null)
            {
                this.Changing(this, changeInfo);
            }
            
            this.Name = changeInfo.Name;
            this.Email = changeInfo.Email;
            this.Gender = changeInfo.Gender;

            if (this.Changed != null)
            {
                this.Changed(this, changeInfo);
            }
        }

        public event TEventHandler<User, UserChangeInfo> ChangedSignInInfo;

        public virtual void ChangeSignInInfo(UserChangeInfo changeInfo)
        {
            this.LastLoginTime = changeInfo.LastLoginTime;
            this.LastLoginIp = changeInfo.LastLoginIp;
            if (this.ChangedSignInInfo != null)
            {
                this.ChangedSignInInfo(this, changeInfo);
            }
        }

        public virtual void ChangePassword(string oldPassword, string newPassword)
        {
            string encodedOldPassword = Cryptography.MD5Encode(oldPassword);
            if (this.Password != encodedOldPassword)
            {
                throw new OldPasswordWrongException();
            }
            string encodedNewPassword = Cryptography.MD5Encode(newPassword);
            this.ImportPassword(this, encodedNewPassword);
        }

        public virtual void ResetPassword(User opUser, string newPassword)
        {
            string encodedNewPassword = Cryptography.MD5Encode(newPassword);
            this.ImportPassword(this, encodedNewPassword);
        }

        public virtual void ImportPassword(User operationUser, string password)
        {
            UserChangeInfo changeInfo = new UserChangeInfo(this.MapUserInfo());
            changeInfo.Password = password;
            this.Password = password;

            if (this.PasswordReseted != null)
            {
                this.PasswordReseted(this, changeInfo);
            }
        }

        public virtual void Lock(User operationUser)
        {
            UserStatus status = UserStatus.Lock;
            UserChangeInfo changeInfo = new UserChangeInfo(this.MapUserInfo());
            changeInfo.Status = status;

            this.Status = status;

            if (this.Locked != null)
            {
                this.Locked(this, changeInfo);
            }
        }

        public virtual void Unlock(User operationUser)
        {
            UserStatus status = UserStatus.Normal;
            UserChangeInfo changeInfo = new UserChangeInfo(this.MapUserInfo());
            changeInfo.Status = status;
            this.Status = status;

            if (this.Activated != null)
            {

                this.Activated(this, changeInfo);
            }
        }

        public virtual void Logoff(User operationUser)
        {
            UserStatus status = UserStatus.Logoff;
            UserChangeInfo changeInfo = new UserChangeInfo(this.MapUserInfo());
            changeInfo.Status = status;
            this.Status = status;

            if (this.Logoffed != null)
            {
                this.Logoffed(this, changeInfo);
            }
        }

        public virtual void Activate(User operationUser)
        {
            UserStatus status = UserStatus.Normal;
            UserChangeInfo changeInfo = new UserChangeInfo(this.MapUserInfo());
            changeInfo.Status = status;
            this.Status = status;

            if (this.Activated != null)
            {
                this.Activated(this, changeInfo);
            }
        }

        public virtual Department MainDepartment
        {
            get
            {
                if (MainPosition == null)
                {
                    return null;
                }
                return this.MainPosition.Department;
            }
        }
        
        public virtual ReadOnlyCollection<Department> Departments
        {
            get
            {
                List<Department> departments = new List<Department>();
                foreach (Position p in Positions)
                {
                    if (p.Department != null)
                    {
                        departments.Add(p.Department);
                    }
                }
                return departments.AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<Position> LesserPosition
        {
            get
            {
                return Positions.Where(x => x != MainPosition).ToList().AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<Group> Groups
        {
            get
            {
                

                var userGroups = from g in this.OrgManager.GroupManager.Groups
                                    where g.GroupUsers.Contains(this)
                                    select g;

                return userGroups.ToList().AsReadOnly();
            }
        }

        public virtual ReadOnlyCollection<User> Superiors
        {
            get
            {
                return this.Positions.SelectMany(
                    p => 
                    {
                        if (p.Parent != null)
                        {
                            return p.Parent.Users;
                        }
                        return new List<User>().AsReadOnly();
                        
                    }).Distinct().ToList().AsReadOnly();
            }
        }

        public virtual bool IsMySuperior(User user, bool recursive)
        {
            if (user == this)
            {
                return false;
            }

            //我的上级职位中有了user的职位
            foreach (Position p in user.Positions)
            {
                if (this.Positions.Any(x => x.IsMySuperior(p, recursive)))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual ReadOnlyCollection<User> MyDepartmentManagers
        {
            get
            {
                var managers = (from manager in Superiors
                               where manager.Departments.FirstOrDefault(x => this.Departments.Contains(x)) != null
                               select manager).ToList();
                return managers.AsReadOnly();
            }
        }

        public virtual bool IsMyDepartmentManagers(User user)
        {
            if (Superiors == null)
            {
                return false;
            }

            //我的上级中找到这个用户
            bool contains = this.MyDepartmentManagers.Contains(user);
            if (contains)
            {
                return true;
            }
            return false;
        }

        public UserInfo MapUserInfo()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.ID = this.ID;
            userInfo.Account = this.Account;
            userInfo.Email = this.Email;
            userInfo.Gender = this.Gender;
            userInfo.Role = this.Role;
            userInfo.LastLoginIp = this.LastLoginIp;
            userInfo.LastLoginTime = this.LastLoginTime;
            if (this.MainPosition != null)
            {
                userInfo.MainPositionId = this.MainPosition.ID;
                userInfo.MainPositionName = this.MainPosition.Name;
            }
            userInfo.Name = this.Name;
            userInfo.Password = this.Password;
            userInfo.Remark = this.Remark;
            userInfo.Status = this.Status;
            if(this.MainDepartment != null)
            {
                userInfo.MainDepartmentId = this.MainDepartment.ID;
                userInfo.MainDepartmentName = this.MainDepartment.Name;
            }
            return userInfo;
        }

        public override MemberType Type
        {
            get { return MemberType.User; }
        }

        public override List<Member> GetParents()
        {
            if (this.MainPosition == null)
            {
                return new List<Member>();
            }
            List<Member> members = new List<Member>();
            foreach (User user in this.MainPosition.Parent.Users)
            {
                members.Add(user);
            }
            return members;
        }

        public override List<Member> GetChildren()
        {
            if (this.MainPosition == null)
            {
                return new List<Member>();
            }
            
            List<Member> members = new List<Member>();
            foreach (Position position in this.MainPosition.Children)
            {
                members.AddRange(position.Users);
            }
            return members;
        }

        public override List<User> GetUsers(bool recursive)
        {
            return new List<User> { this};
        }

        public override bool Contains(User user)
        {
            return this == user;
        }
    }
}
