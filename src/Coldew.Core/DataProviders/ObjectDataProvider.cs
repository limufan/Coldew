using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;

namespace Coldew.Core.DataProviders
{
    public class ObjectDataProvider
    {
        public void Insert(ColdewObject cobject)
        {
            ColdewObjectModel model = new ColdewObjectModel
            {
                ID = cobject.ID,
                Code = cobject.Code,
                Name = cobject.Name,
                IsSystem = cobject.IsSystem,
                Index = cobject.Index
            };
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(ColdewObject cobject)
        {
            ColdewObjectModel model = NHibernateHelper.CurrentSession.Get<ColdewObjectModel>(cobject.ID);
            model.NameFieldId = cobject.NameField.ID;
            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public IList<ColdewObjectModel> Select()
        {
            List<ColdewObject> objectList = new List<ColdewObject>();
            IList<ColdewObjectModel> models = NHibernateHelper.CurrentSession.QueryOver<ColdewObjectModel>().List();
            return models;
        }
    }
}
