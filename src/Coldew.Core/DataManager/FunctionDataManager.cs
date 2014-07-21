using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class FunctionDataManager
    {
        internal FunctionDataProvider DataProvider { private set; get; }
        OrganizationManagement _orgManager;
        public FunctionDataManager(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
            this.DataProvider = new FunctionDataProvider(orgManager);
            orgManager.FunctionManager.Created += FunctionManager_Created;
            this.Load();
        }

        void FunctionManager_Created(FunctionManagement sender, Function args)
        {
            this.DataProvider.Insert(args);
        }

        void Load()
        {
            List<Function> functions = this.DataProvider.Select();
            this._orgManager.FunctionManager.AddFunction(functions);
        }
    }
}
