using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class GroupDataManager
    {
        public GroupDataProvider DataProvider { private set; get; }
        OrganizationManagement _orgManager;
        public GroupDataManager(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
            this.DataProvider = new GroupDataProvider(orgManager);
            orgManager.GroupManager.Created += GroupManager_Created;
            orgManager.GroupManager.Deleted += GroupManager_Deleted;
            this.Load();
        }

        void GroupManager_Created(GroupManagement manager, Group group)
        {
            this.DataProvider.Insert(group);
            this.BindEvent(group);
        }

        void GroupManager_Deleted(GroupManagement manager, DeleteEventArgs<Group> args)
        {
            this.DataProvider.Delete(args.DeleteObject);
        }

        private void BindEvent(Group group)
        {
            group.Changed += Group_Changed;
            group.RemovedMember += Group_RemovedMember;
            group.AddedMember += Group_AddedMember;
        }

        void Group_AddedMember(Group sender, ChangeEventArgs<GroupMemberInfo, Group> args)
        {
            this.DataProvider.Update(sender);
        }

        void Group_RemovedMember(Group group, ChangeEventArgs<GroupMemberInfo, Group> args)
        {
            this.DataProvider.Update(group);
        }

        void Group_Changed(Group group, GroupChangeInfo changeInfo)
        {
            this.DataProvider.Update(group);
        }

        void Load()
        {
            List<Group> groups = this.DataProvider.Select();
            this._orgManager.GroupManager.AddGroups(groups);
            foreach (Group group in groups)
            {
                this.BindEvent(group);
            }
        }

        public void LoadMembers()
        {
            this.DataProvider.LoadMembers(this._orgManager.GroupManager.Groups.ToList());
        }
    }
}
