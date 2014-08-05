using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data.Organization;

namespace Coldew.Core.DataProviders
{
    public class FunctionDataProvider
    {
        OrganizationManagement _orgManager;
        public FunctionDataProvider(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
        }

        public void Insert(Function function)
        {
            string memberIds = string.Join(",", function.OwnerMembers.Select(x => x.ID));
            FunctionModel model = new FunctionModel
            {
                IconClass = function.IconClass,
                ID = function.ID,
                Name = function.Name,
                Sort = function.Sort,
                Url = function.Url,
                OwnerMemberIds = memberIds
            };
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(Function function)
        {
            DepartmentModel model = NHibernateHelper.CurrentSession.Get<DepartmentModel>(function.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            List<Function> functions = new List<Function>();
            IList<FunctionModel> funcModels = NHibernateHelper.CurrentSession.QueryOver<FunctionModel>().List();
            if (funcModels != null)
            {
                foreach (FunctionModel model in funcModels)
                {
                    List<Member> members = new List<Member>();
                    string[] memberIds = model.OwnerMemberIds.Split(',');
                    foreach (string memberId in memberIds)
                    {
                        Member member = this._orgManager.GetMember(memberId);
                        if (member != null)
                        {
                            members.Add(member);
                        }
                    }
                    Function function = new Function(model.ID, model.Name, model.Url, model.IconClass, model.Sort, members, this._orgManager);
                    functions.Add(function);
                }
            }
            this._orgManager.FunctionManager.AddFunction(functions);
        }
    }
}
