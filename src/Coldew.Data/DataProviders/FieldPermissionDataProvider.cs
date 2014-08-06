using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Core.Permission;
using Coldew.Data;

namespace Coldew.Data.DataProviders
{
    public class FieldPermissionDataProvider
    {
        ColdewObjectManager _objectManager;
        public FieldPermissionDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public void Insert(FieldPermission permission)
        {
            FieldPermissionModel model = new FieldPermissionModel();
            model.ObjectId = permission.Field.ColdewObject.ID;
            model.FieldId = permission.Field.ID;
            model.MemberId = permission.Member.ID;
            model.Value = (int)permission.Value;
            model.ID = Guid.NewGuid().ToString();

            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Load()
        {
            IList<FieldPermissionModel> models = NHibernateHelper.CurrentSession.QueryOver<FieldPermissionModel>().List();
            foreach (FieldPermissionModel model in models)
            {
                FieldPermission permission = this.Create(model);
            }
        }

        private FieldPermission Create(FieldPermissionModel model)
        {
            ColdewObject cobject = this._objectManager.GetObjectById(model.ObjectId);
            Member member = cobject.ColdewManager.OrgManager.GetMember(model.MemberId);
            if (member != null)
            {
                Field field = cobject.GetFieldById(model.FieldId);
                FieldPermission permission = new FieldPermission(model.ID, field, member, (FieldPermissionValue)model.Value);
                cobject.FieldPermission.AddPermission(permission);
                return permission;

            }
            return null;
        }
    }
}
