using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class DepartmentDataManager
    {
        internal DepartmentDataProvider DataProvider { private set; get; }
        OrganizationManagement _orgManager;
        public DepartmentDataManager(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
            this.DataProvider = new DepartmentDataProvider(orgManager);
            orgManager.DepartmentManager.Created += DepartmentManager_Created;
            orgManager.DepartmentManager.Deleted += DepartmentManager_Deleted;
            this.Load();
        }

        void DepartmentManager_Deleted(DepartmentManagement department, DeleteEventArgs<Department> args)
        {
            this.DataProvider.Delete(args.DeleteObject);
        }

        void DepartmentManager_Created(DepartmentManagement sender, Department args)
        {
            this.DataProvider.Insert(args);
        }

        private void BindEvent(Department deparment)
        {
            deparment.Changed += Deparment_Changed;
        }

        void Deparment_Changed(Department sender, DepartmentChangeInfo args)
        {
            this.DataProvider.Update(sender);
        }

        void Load()
        {
            List<Department> departments = this.DataProvider.Select();
            this._orgManager.DepartmentManager.AddDepartment(departments);
            foreach (Department dept in departments)
            {
                this.BindEvent(dept);
            }
        }
    }
}
