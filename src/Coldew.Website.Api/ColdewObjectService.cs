using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.Organization;
using Coldew.Website.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
{
    public class ColdewObjectService : IColdewObjectService
    {
        ColdewManager _coldewManager;

        public ColdewObjectService(ColdewManager crmManager)
        {
            this._coldewManager = crmManager;
        }

        public ColdewObjectWebModel GetObjectById(string userAccount, string objectId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            if (cobject != null)
            {
                return new ColdewObjectWebModel(cobject, user);
            }
            return null;
        }

        public ColdewObjectWebModel GetObjectByCode(string userAccount, string objectCode)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectByCode(objectCode);
            if (cobject != null)
            {
                return new ColdewObjectWebModel(cobject, user);
            }
            return null;
        }

        public List<ColdewObjectWebModel> GetObjects(string userAccount)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            List<ColdewObject> objects = this._coldewManager.ObjectManager.GetObjects();
            return objects.Where(x =>
            {
                return x.ObjectPermission.HasValue(user, ObjectPermissionValue.View);
            }).Select(x => new ColdewObjectWebModel(x, user)).ToList();
        }

        public MetadtaGridPageModel GetGridPageModel(string userAccount, string objectId, string viewId)
        {
            User user = this._coldewManager.OrgManager.UserManager.GetUserByAccount(userAccount);
            ColdewObject cobject = this._coldewManager.ObjectManager.GetObjectById(objectId);
            if (string.IsNullOrEmpty(viewId))
            {
                viewId = cobject.GridViewManager.GetGridViews(user)[0].ID;
            }
            GridView view = cobject.GridViewManager.GetGridView(viewId);
            return this.MapPageModel(user, cobject, view);
        }
        
        private MetadtaGridPageModel MapPageModel(User opUser, ColdewObject cobject, GridView view)
        {
            MetadtaGridPageModel model = new MetadtaGridPageModel();
            model.objectId = cobject.ID;
            model.nameField = cobject.NameField.Code;
            model.permission = cobject.ObjectPermission.GetPermission(opUser);
            model.columns = view.Columns.Select(x => new DataGridColumnModel(x)).ToList();
            model.fields = cobject.GetFields().Select(x => FieldWebModel.Map(x, opUser)).ToList();
            model.menus = cobject.GridViewManager.GetGridViews(opUser).Select(x => new LeftMenuModel(x)).ToList();
            model.title = view.Name;
            model.viewId = view.ID;
            return model;
        }
    }
}
