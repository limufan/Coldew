using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data.Organization;

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
            GroupModel model = new GroupModel
            {
                CreateTime = group.CreateTime,
                CreatorId = group.Creator.ID,
                GroupType = (int)group.GroupType,
                Name = group.Name,
                Remark = group.Remark,
                ID = group.ID
            };
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Group group)
        {
            DepartmentModel model = NHibernateHelper.CurrentSession.Get<DepartmentModel>(group.ID);
            model.Name = group.Name;
            model.Remark = group.Remark;

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
            List<Group> deparments = new List<Group>();
            List<DepartmentModel> models = NHibernateHelper.CurrentSession.QueryOver<DepartmentModel>().List().ToList();
            if (models != null)
            {
                models.ForEach(x =>
                {
                    Group group = new Group(this._orgManager, x.ID, x.Name, this._orgManager.PositionManager.GetPositionById(x.ManagerPositionId), x.Remark);
                    deparments.Add(group);
                });
            }
            return deparments;
        }
    }
}
