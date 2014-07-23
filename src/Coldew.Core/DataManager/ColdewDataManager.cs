using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class ColdewDataManager
    {
        public ColdewManager ColdewManager { set; get; }
        public UserDataManager UserDataManager { set; get; }
        public DepartmentDataManager DepartmentDataManager { set; get; }
        public FunctionDataManager FunctionDataManager { set; get; }
        public PositionDataManager PositionDataManager { set; get; }
        public GroupDataManager GroupDataManager { set; get; }
        public ObjectDataManager ObjectDataManager { set; get; }
        public ColdewDataManager(ColdewManager coldewManager)
        {
            this.UserDataManager = new UserDataManager(coldewManager.OrgManager);
            this.PositionDataManager = new PositionDataManager(coldewManager.OrgManager);
            this.DepartmentDataManager = new DepartmentDataManager(coldewManager.OrgManager);
            this.FunctionDataManager = new FunctionDataManager(coldewManager.OrgManager);
            this.GroupDataManager = new GroupDataManager(coldewManager.OrgManager);
            this.ObjectDataManager = new ObjectDataManager(coldewManager);
        }
    }
}
