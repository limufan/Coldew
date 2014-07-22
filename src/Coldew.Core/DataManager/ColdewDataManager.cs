using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class ColdewDataManager
    {
        internal ObjectDataProvider ObjectDataProvider { private set; get; }
        ColdewManager _coldewManager;
        UserDataManager _userDataManager;
        DepartmentDataManager _departmentDataManager;
        FunctionDataManager _functionDataManager;
        PositionDataManager _positionDataManager;
        GroupDataManager _groupDataManager;
        ObjectDataManager _objectDataManager;
        public ColdewDataManager(ColdewManager coldewManager)
        {
            this._userDataManager = new UserDataManager(coldewManager.OrgManager);
            this._positionDataManager = new PositionDataManager(coldewManager.OrgManager);
            this._departmentDataManager = new DepartmentDataManager(coldewManager.OrgManager);
            this._functionDataManager = new FunctionDataManager(coldewManager.OrgManager);
            this._groupDataManager = new GroupDataManager(coldewManager.OrgManager);
            this._objectDataManager = new ObjectDataManager(coldewManager);
        }
    }
}
