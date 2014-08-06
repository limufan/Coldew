using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.Organization;
using Coldew.Data.Organization;
using Newtonsoft.Json;

namespace Coldew.Data.DataProviders
{
    public class GroupDataProvider
    {
        OrganizationManagement _orgManager;
        public GroupDataProvider(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
        }

        public void Insert(Group group)
        {
            string members = string.Join(",", group.Members.Select(m => m.ID));
            GroupModel model = new GroupModel
            {
                CreateTime = group.CreateTime,
                CreatorId = group.Creator.ID,
                GroupType = (int)group.GroupType,
                Name = group.Name,
                Remark = group.Remark,
                Members = members,
                ID = group.ID
            };
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Group group)
        {
            string members = string.Join(",", group.Members.Select(m => m.ID));
            GroupModel model = NHibernateHelper.CurrentSession.Get<GroupModel>(group.ID);
            model.Name = group.Name;
            model.Remark = group.Remark;
            model.Members = members;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(Group group)
        {
            GroupModel model = NHibernateHelper.CurrentSession.Get<GroupModel>(group.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            List<Group> groups = new List<Group>();
            List<GroupModel> models = NHibernateHelper.CurrentSession.QueryOver<GroupModel>().List().ToList();
            if (models != null)
            {
                models.ForEach(x =>
                {
                    Group group = new Group(x.ID, x.Name, x.CreateTime, this._orgManager.UserManager.GetUserById(x.CreatorId), x.Remark, 
                        (GroupType)x.GroupType, this._orgManager);
                    groups.Add(group);
                });
            }
            this._orgManager.GroupManager.AddGroups(groups);
            this.LoadMembers(groups);
        }

        private void LoadMembers(List<Group> groups)
        {
            Dictionary<string, string> members = new Dictionary<string, string>();
            List<GroupModel> models = NHibernateHelper.CurrentSession.QueryOver<GroupModel>().List().ToList();
            foreach (GroupModel model in models)
            {
                members.Add(model.ID, model.Members);
            }
            foreach (Group group in groups)
            {
                string memberIds = members[group.ID];
                if (!string.IsNullOrEmpty(memberIds))
                {
                    string[] memberIdArray = memberIds.Split(',');
                    foreach (string memberId in memberIdArray)
                    {
                        Member member = this._orgManager.GetMember(memberId);
                        if (member != null)
                        {
                            group._Members.Add(member);
                        }
                    }
                }
            }
        }
    }
}
