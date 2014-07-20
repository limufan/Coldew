using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core.Organization;
using Coldew.Core.Permission;
using Coldew.Data;

namespace Coldew.Core.DataProviders
{
    public class ObjectPermissionDataProvider
    {
        ColdewObject _cobject;
        public ObjectPermissionDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(ObjectPermission permission)
        {
            ObjectPermissionModel model = new ObjectPermissionModel();
            model.ObjectId = this._cobject.ID;
            model.MemberId = permission.Member.ID;
            model.Value = (int)permission.Value;
            model.ID = permission.ID;
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<ObjectPermission> Select()
        {
            List<ObjectPermission> perms = new List<ObjectPermission>();
            IList<ObjectPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<ObjectPermissionModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (ObjectPermissionModel model in models)
            {
                ObjectPermission permission = this.Create(model);
                if (permission != null)
                {
                    perms.Add(permission);
                }
            }
            return perms;
        }

        private ObjectPermission Create(ObjectPermissionModel model)
        {
            Member member = this._cobject.ColdewManager.OrgManager.GetMember(model.MemberId);
            if (member != null)
            {
                ObjectPermission permission = new ObjectPermission(model.ID, member, model.ObjectId, (ObjectPermissionValue)model.Value);
                return permission;
            }
            return null;
        }
    }
}
