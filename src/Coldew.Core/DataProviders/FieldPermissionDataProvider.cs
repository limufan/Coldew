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
    public class FieldPermissionDataProvider
    {
        ColdewObject _cobject;
        public FieldPermissionDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(FieldPermission permission)
        {
            FieldPermissionModel model = new FieldPermissionModel();
            model.ObjectId = this._cobject.ID;
            model.FieldId = permission.Field.ID;
            model.MemberId = permission.Member.ID;
            model.Value = (int)permission.Value;
            model.ID = Guid.NewGuid().ToString();

            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<FieldPermission> Select()
        {
            List<FieldPermission> perms = new List<FieldPermission>();
            IList<FieldPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<FieldPermissionModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (FieldPermissionModel model in models)
            {
                FieldPermission permission = this.Create(model);
                if (permission != null)
                {
                    perms.Add(permission);
                }
            }
            return perms;
        }

        private FieldPermission Create(FieldPermissionModel model)
        {
            Member member = this._cobject.ColdewManager.OrgManager.GetMember(model.MemberId);
            if (member != null)
            {
                Field field = this._cobject.GetFieldById(model.FieldId);
                FieldPermission permission = new FieldPermission(model.ID, field, member, (FieldPermissionValue)model.Value);
                return permission;

            }
            return null;
        }
    }
}
