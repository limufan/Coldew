using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data.Organization;

namespace Coldew.Core.DataProviders
{
    public class DepartmentDataProvider
    {
        OrganizationManagement _orgManager;
        public DepartmentDataProvider(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
        }

        public void Insert(Department department)
        {
            DepartmentModel departmentModel = new DepartmentModel
            {
                ManagerPositionId = department.ManagerPosition.ID,
                Name = department.Name,
                ParentId = department.Parent == null ? "" : department.Parent.ID,
                Remark = department.Remark,
                ID = department.ID
            };

            NHibernateHelper.CurrentSession.Save(departmentModel);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Department department)
        {
            DepartmentModel model = NHibernateHelper.CurrentSession.Get<DepartmentModel>(department.ID);
            model.Name = department.Name;
            model.Remark = department.Remark;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(Department department)
        {
            DepartmentModel model = NHibernateHelper.CurrentSession.Get<DepartmentModel>(department.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<Department> Select()
        {
            List<Department> deparments = new List<Department>();
            List<DepartmentModel> models = NHibernateHelper.CurrentSession.QueryOver<DepartmentModel>().List().ToList();
            if (models != null)
            {
                models.ForEach(x =>
                {
                    Department department = new Department(this._orgManager, x.ID, x.Name, this._orgManager.PositionManager.GetPositionById(x.ManagerPositionId), x.Remark);
                    deparments.Add(department);
                });
            }
            return deparments;
        }
    }
}
