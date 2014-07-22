using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.Organization;
using Coldew.Data.Organization;

namespace Coldew.Core.DataProviders
{
    public class UserDataProvider
    {
        OrganizationManagement _orgManager;
        public UserDataProvider(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
        }

        public void Insert(User user)
        {
            UserModel userModel = new UserModel
            {
                Account = user.Account,
                Email = user.Email,
                Gender = (int)user.Gender,
                Name = user.Name,
                Password = Cryptography.MD5Encode(user.Password),
                Remark = user.Remark,
                Role = (int)user.Role,
                Status = (int)user.Status,
                MainPositionId = user.MainDepartment.ID
            };

            NHibernateHelper.CurrentSession.Save(userModel);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(User user)
        {
            UserModel model = NHibernateHelper.CurrentSession.Get<UserModel>(user.ID);
            model.Name = user.Name;
            model.Email = user.Email;
            model.Gender = (int)user.Gender;
            model.Status = (int)user.Status;
            model.Password = user.Password;
            model.LastLoginTime = user.LastLoginTime;
            model.LastLoginIp = user.LastLoginIp;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(User user)
        {
            UserModel model = NHibernateHelper.CurrentSession.Get<UserModel>(user.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public List<User> Select()
        {
            List<User> users = new List<User>();
            List<UserModel> models = NHibernateHelper.CurrentSession.QueryOver<UserModel>().List().ToList();
            if (models != null)
            {
                models.ForEach(x =>
                {
                    User user = new User(x.ID, x.Name, x.Account, x.Password, x.Email, (UserGender)x.Gender, (UserRole)x.Role, (UserStatus)x.Status, 
                        x.LastLoginTime, x.LastLoginIp, x.Remark, x.MainPositionId, this._orgManager);
                    users.Add(user);
                });
            }
            return users;
        }
    }
}
