using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.Organization;
using Coldew.Data.Organization;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
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
            List<GroupMemberModel> memberModels = this.GetMemberModels(group);
            GroupModel model = new GroupModel
            {
                CreateTime = group.CreateTime,
                CreatorId = group.Creator.ID,
                GroupType = (int)group.GroupType,
                Name = group.Name,
                Remark = group.Remark,
                MembersJson = JsonConvert.SerializeObject(memberModels),
                ID = group.ID
            };
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        private List<GroupMemberModel> GetMemberModels(Group group)
        {
            List<GroupMemberModel> models = new List<GroupMemberModel>();
            List<User> users = group.Users.ToList();
            foreach (User user in users)
            {
                GroupMemberModel model = new GroupMemberModel
                {
                    GroupId = group.ID,
                    MemberId = user.ID,
                    MemberType = (int)MemberType.Group
                };
                models.Add(model);
            }
            return models;
        }

        public void Update(Group group)
        {
            List<GroupMemberModel> memberModels = this.GetMemberModels(group);
            GroupModel model = NHibernateHelper.CurrentSession.Get<GroupModel>(group.ID);
            model.Name = group.Name;
            model.Remark = group.Remark;
            model.MembersJson = JsonConvert.SerializeObject(memberModels);

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(Group group)
        {
            GroupModel model = NHibernateHelper.CurrentSession.Get<GroupModel>(group.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<Group> Select()
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
            models.ForEach(groupModel =>
            {
                List<GroupMemberModel> memberModels = JsonConvert.DeserializeObject<List<GroupMemberModel>>(groupModel.MembersJson);
                Group group = groups.Find(g => g.ID == groupModel.ID);
                memberModels.ForEach(memberModel =>
                    {
                        switch ((MemberType)memberModel.MemberType)
                        {
                            case MemberType.User:
                                User user = this._orgManager.UserManager.GetUserById(memberModel.MemberId);
                                if (user != null)
                                {
                                    group._GroupUsers.Add(user);
                                }
                                break;
                            case MemberType.Department:
                                Department department = this._orgManager.DepartmentManager.GetDepartmentById(memberModel.MemberId);
                                if (department != null)
                                {
                                    group._Departments.Add(department);
                                }
                                break;
                            case MemberType.Position:
                                Position position = this._orgManager.PositionManager.GetPositionById(memberModel.MemberId);
                                if (position != null)
                                {
                                    group._Positions.Add(position);
                                }
                                break;
                            case MemberType.Group:
                                Group groupMember = this._orgManager.GroupManager.GetGroupById(memberModel.MemberId);
                                if (group != null)
                                {
                                    group._Groups.Add(groupMember);
                                }
                                break;
                        }
                    });
            });
            return groups;
        }
    }
}
