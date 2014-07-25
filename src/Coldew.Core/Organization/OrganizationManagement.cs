﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Coldew.Data.Organization;
using System.IO;
using Coldew.Api.Organization.Exceptions;
using System.Collections.Specialized;
using Coldew.Api.Organization;
using System.Threading;
namespace Coldew.Core.Organization
{
    public class OrganizationManagement
    {
        private Dictionary<string, Member> _memberDicById;
        protected ReaderWriterLock _lock;

        public OrganizationManagement()
        {
            log4net.Config.XmlConfigurator.Configure();
            this.Logger = log4net.LogManager.GetLogger("logger");
            this._memberDicById = new Dictionary<string, Member>();
            this._lock = new ReaderWriterLock();
            try
            {
                this.Everyone = new EveryoneGroup(this);
                this._memberDicById.Add(this.Everyone.ID, this.Everyone);
                this.InitManagers();

                this.UserManager.AddUser(new List<User>(){this.System});
                this.UserManager.Deleted += new TEventHandler<UserManagement, DeleteEventArgs<User>>(UserManager_Deleted);
                this.UserManager.Added += new TEventHandler<UserManagement, List<User>>(UserManager_Loaded);
                this.UserManager.Created += new TEventHandler<UserManagement, User>(UserManager_Created);

                this.PositionManager.Deleted += new TEventHandler<PositionManagement, DeleteEventArgs<Position>>(PositionManager_Deleted);
                this.PositionManager.Added += new TEventHandler<PositionManagement, List<Position>>(PositionManager_Loaded);
                this.PositionManager.Created += new TEventHandler<PositionManagement, Position>(PositionManager_Created);

                this.DepartmentManager.Deleted += new TEventHandler<DepartmentManagement, DeleteEventArgs<Department>>(DepartmentManager_Deleted);
                this.DepartmentManager.Loaded += new TEventHandler<DepartmentManagement, List<Department>>(DepartmentManager_Loaded);
                this.DepartmentManager.Created += new TEventHandler<DepartmentManagement, Department>(DepartmentManager_Created);

                this.GroupManager.Deleted += new TEventHandler<GroupManagement, DeleteEventArgs<Group>>(GroupManager_Deleted);
                this.GroupManager.Added += new TEventHandler<GroupManagement, List<Group>>(GroupManager_Loaded);
                this.GroupManager.Created += new TEventHandler<GroupManagement, Group>(GroupManager_Created);
            }
            catch(Exception ex)
            {
                this.Logger.Error(ex.Message, ex);
                throw;
            }
        }

        void GroupManager_Created(GroupManagement sender, Group args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                this._memberDicById.Add(args.ID, args);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void GroupManager_Loaded(GroupManagement sender, List<Group> args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                foreach (Group group in args)
                {
                    this._memberDicById.Add(group.ID, group);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void DepartmentManager_Created(DepartmentManagement sender, Department args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                this._memberDicById.Add(args.ID, args);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void DepartmentManager_Loaded(DepartmentManagement sender, List<Department> args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                foreach (Department dept in args)
                {
                    this._memberDicById.Add(dept.ID, dept);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void PositionManager_Created(PositionManagement sender, Position args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                this._memberDicById.Add(args.ID, args);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void PositionManager_Loaded(PositionManagement sender, List<Position> args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                foreach (Position position in args)
                {
                    this._memberDicById.Add(position.ID, position);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void UserManager_Created(UserManagement sender, User user)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                this._memberDicById.Add(user.ID, user);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        void UserManager_Loaded(UserManagement sender, List<User> args)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                foreach (User user in args)
                {
                    this._memberDicById.Add(user.ID, user);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public void ValidateLicense()
        {
            
        }

        void GroupManager_Deleted(GroupManagement sender, DeleteEventArgs<Group> args)
        {
            foreach (Group group in this.GroupManager.Groups)
            {
                if (group.Contains(args.DeleteObject))
                {
                    group.RemoveMember(args.Operator, args.DeleteObject);
                }
            }
        }

        void DepartmentManager_Deleted(DepartmentManagement sender, DeleteEventArgs<Department> args)
        {
            foreach (Group group in this.GroupManager.Groups)
            {
                if (group.Contains(args.DeleteObject))
                {
                    group.RemoveMember(args.Operator, args.DeleteObject);
                }
            }
        }

        void PositionManager_Deleted(PositionManagement sender, DeleteEventArgs<Position> args)
        {
            foreach (Group group in this.GroupManager.Groups)
            {
                if (group.Contains(args.DeleteObject))
                {
                    group.RemoveMember(args.Operator, args.DeleteObject);
                }
            }
        }

        void UserManager_Deleted(UserManagement sender, DeleteEventArgs<User> args)
        {
            foreach (Group group in this.GroupManager.Groups)
            {
                if (group.Contains(args.DeleteObject))
                {
                    group.RemoveMember(args.Operator, args.DeleteObject);
                }
            }
        }

        #region Managers
        /// <summary>
        /// 用户管理
        /// </summary>

        UserManagement _userManager;
        public UserManagement UserManager
        {
            get
            {
                return this._userManager;
            }
        }

        /// <summary>
        /// 部门管理
        /// </summary>
        DepartmentManagement _departmentManager;
        public DepartmentManagement DepartmentManager
        {
            get
            {
                return this._departmentManager;
            }
        }

        /// <summary>
        /// 职位管理
        /// </summary>
        PositionManagement _positionManager;
        public PositionManagement PositionManager
        {
            get
            {
                return this._positionManager;
            }
        }

        /// <summary>
        /// 用户组管理
        /// </summary>
        GroupManagement _groupManager;
        public GroupManagement GroupManager
        {
            get
            {
                return this._groupManager;
            }
        }

        /// <summary>
        /// 登录认证管理
        /// </summary>
        AuthenticationManagement _authenticationManager;
        public AuthenticationManagement AuthenticationManager
        {
            get
            {
                return this._authenticationManager;
            }
        }

        /// <summary>
        /// 操作日志管理
        /// </summary>
        OperationLogManagement _operationLogManager;
        public OperationLogManagement OperationLogManager
        {
            get
            {
                return this._operationLogManager;
            }
        }

        /// <summary>
        /// 功能权限管理
        /// </summary>
        FunctionManagement _functionManager;
        public FunctionManagement FunctionManager
        {
            get
            {
                return this._functionManager;
            }
        }
        #endregion

        ILog _logger;
        public ILog Logger 
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _logger = value;
            }
            get
            {
                return _logger;
            }
        }

        private void InitManagers()
        {
            this._userManager = new UserManagement(this);
            this._positionManager = new PositionManagement(this);
            this._departmentManager = new DepartmentManagement(this);
            this._groupManager = new GroupManagement(this);
            this._authenticationManager = new AuthenticationManagement(this);
            //this._operationLogManager = new OperationLogManagement(this);
            this._functionManager = new FunctionManagement(this);
        }

        public EveryoneGroup Everyone { private set; get; }

        User _system;
        public User System
        {
            get
            {
                if (_system == null)
                {
                    _system = new User
                    {
                        OrgManager = this,
                        ID = "system",
                        Account = "system",
                        Gender = UserGender.Man,
                        Role = UserRole.System,
                        Name = "System",
                        Password = Cryptography.MD5Encode("edoc2")
                    };
                }
                return _system;
            }
        }

        public Member GetMember(string id)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                if (this._memberDicById.ContainsKey(id))
                {
                    return this._memberDicById[id];
                }
                return null;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }
    }
}
