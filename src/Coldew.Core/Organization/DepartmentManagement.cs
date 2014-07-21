using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coldew.Api.Organization.Exceptions;
using System.Linq;
using Coldew.Data.Organization;
using Coldew.Api.Organization;


namespace Coldew.Core.Organization
{
    public class DepartmentManagement
    {
        public DepartmentManagement(OrganizationManagement orgMnger)
        {
            this._orgMnger = orgMnger;
            this._departments = new List<Department>();
        }

        private bool _loaded;

        private List<Department> _departments;
        private List<Department> _Departments
        {
            get
            {
                return _departments;
            }
        }
        private OrganizationManagement _orgMnger;

        public event TEventHandler<DepartmentManagement, List<Department>> Loaded;

        /// <summary>
        /// 创建之前
        /// </summary>
        public event TEventHandler<DepartmentManagement, CreatedEventArgs<DepartmentCreateInfo, Department>> Creating;

        /// <summary>
        /// 创建之后
        /// </summary>
        public event TEventHandler<DepartmentManagement, Department> Created;

        /// <summary>
        /// 删除之前
        /// </summary>
        public event TEventHandler<Department, User> Deleting;

        /// <summary>
        /// 删除之后
        /// </summary>
        public event TEventHandler<Department, User> Deleted;

        private object _updateLockObject = new object();

        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="department">部门</param>
        /// <returns>部门</returns>
        public Department Create(User operationUser, DepartmentCreateInfo createInfo)
        {
            lock (_updateLockObject)
            {
                if (operationUser == null)
                {
                    throw new ArgumentNullException("operationUser");
                }
                if (createInfo == null)
                {
                    throw new ArgumentNullException("createInfo");
                }
                if (createInfo.Name == null)
                {
                    throw new ArgumentNullException("createInfo.Name");
                }
                if (createInfo.ManagerPositionInfo == null)
                {
                    throw new ArgumentNullException("createInfo.ManagerPositionInfo");
                }

                List<Department> departments = this._Departments;

                Position managerPositionParent = this._orgMnger.PositionManager.GetPositionById(createInfo.ManagerPositionInfo.ParentId);
                if (managerPositionParent != null)
                {
                    Department child = managerPositionParent.Department.Children.FirstOrDefault(x => x.Name == createInfo.Name);
                    if (child != null)
                    {
                        throw new DepartmentNameReapeatException();
                    }
                }

                CreatedEventArgs<DepartmentCreateInfo, Department> args = new CreatedEventArgs<DepartmentCreateInfo, Department>
                {
                    CreateInfo = createInfo,
                    Operator = operationUser
                };
                Position managerPosition = this._orgMnger.PositionManager.Create(operationUser, createInfo.ManagerPositionInfo, OrganizationType.ManagerPosition);
                string parentDepartmentId = null;
                if(managerPosition.Parent != null)
                {
                    parentDepartmentId = managerPosition.Parent.Department.ID;
                }

                if (this.Creating != null)
                {
                    this.Creating(this, args);
                }

                Department department = new Department(this._orgMnger, Guid.NewGuid().ToString(), createInfo.Name, managerPosition, managerPosition.Remark);

                List<Department> tempDepartments = departments.ToList();
                tempDepartments.Add(department);
                this._departments = tempDepartments;

                if (this.Created != null)
                {
                    this.Created(this, department);
                }

                return department;
            }
        }
        
        

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departmentId">部门ID</param>
        public void Delete(User operationUser, string departmentId)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }
            if (departmentId == null)
            {
                throw new ArgumentNullException("departmentId");
            }
            Department department = this.GetDepartmentById(departmentId);
            if (department != null)
            {
                if (department.AllUsers.Count > 0)
                {
                    throw new DepartmentHasUserDeleteException();
                }
                if (department.AllLogoffedUsers.Count > 0)
                {
                    throw new DepartmentHasLogoffUserDeleteException();
                }
                lock (_updateLockObject)
                {
                    foreach (Position position in department.Positions)
                    {
                        this._orgMnger.PositionManager.Delete(operationUser, position.ID);
                    }

                    if (Deleting != null)
                    {
                        this.Deleting(department, operationUser);
                    }

                    List<Department> tempDepartments = this._Departments.ToList();
                    tempDepartments.Remove(department);
                    this._departments = tempDepartments;

                    if (this.Deleted != null)
                    {
                        this.Deleted(department, operationUser);
                    }
                }
            }
        }

        public Department TopDepartment
        {
            get
            {
                
                return (from d in this._Departments
                        where d.Parent == null
                        select d).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="departmentId">部门ID</param>
        /// <returns>部门</returns>
        public Department GetDepartmentById(string departmentId)
        {
            
            if (string.IsNullOrEmpty(departmentId)) return null;

            return this._Departments.Find(x => x.ID == departmentId);
        }

        public ReadOnlyCollection<Department> Departments
        {
            get
            {
                
                return this._Departments.AsReadOnly();
            }
        }

        internal virtual void AddDepartment(List<Department> departments)
        {
            this._departments.AddRange(departments);
        }

		/// <summary>
		/// 根据部门名称获取第一个匹配的部门 (lvxing)
		/// </summary>
		/// <param name="deptName">部门名称</param>
		/// <returns>部门</returns>
		public Department GetTopDepartmentByDeptName(string deptName)
		{
			return string.IsNullOrEmpty(deptName) ? null : _Departments.Find(x => x.Name.ToUpper().Equals(deptName.ToUpper()));
		}

		/// <summary>
		/// 根据部门名称获取部门列表 (lvxing)
		/// </summary>
		/// <param name="deptName">部门名称</param>
		/// <returns>部门列表</returns>
		public List<Department> GetDepartmentByDeptName(string deptName)
		{
			return string.IsNullOrEmpty(deptName) ? new List<Department>() : _Departments.FindAll(x => x.Name.ToUpper().Equals(deptName.ToUpper()));
		}

        public List<Department> SearchByName(string Name)
        {
            List<Department> deptInfoList = _Departments.Where(x => x.Name.IndexOf(Name, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
            return deptInfoList;
        }

    }
}