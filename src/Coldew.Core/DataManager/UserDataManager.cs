using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class UserDataManager
    {
        internal UserDataProvider DataProvider { private set; get; }
        OrganizationManagement _orgManager;
        public UserDataManager(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
            this.DataProvider = new UserDataProvider(orgManager);
            orgManager.UserManager.Created += UserManager_Created;
            orgManager.UserManager.Deleted += UserManager_Deleted;
            this.Load();
        }

        void UserManager_Created(UserManagement manager, User user)
        {
            this.DataProvider.Insert(user);
        }

        void UserManager_Deleted(UserManagement manager, DeleteEventArgs<User> args)
        {
            this.DataProvider.Delete(args.DeleteObject);
        }

        private void BindEvent(User user)
        {
            user.Changed += User_Changed;
            user.Locked += User_Changed;
            user.Activated += User_Changed;
            user.ChangedSignInInfo += User_Changed;
            user.Logoffed += User_Changed;
        }

        void User_Changed(User sender, UserChangeInfo args)
        {
            this.DataProvider.Update(sender);
        }

        void Load()
        {
            List<User> users = this.DataProvider.Select();
            this._orgManager.UserManager.AddUser(users);
            foreach (User user in users)
            {
                this.BindEvent(user);
            }
        }
    }
}
