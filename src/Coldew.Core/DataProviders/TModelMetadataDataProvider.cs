using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class TModelMetadataDataProvider<TModel>
        where TModel :TModelMetadataModel 
    {
        public TModelMetadataDataProvider(ColdewObject cobject)
        {
            
        }

        public void Create(Metadata metadata)
        {
            TModel model = Activator.CreateInstance<TModel>();
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());
            model.ID = metadata.ID;
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(Metadata metadata)
        {
            TModel model = NHibernateHelper.CurrentSession.Get<TModel>(metadata.ID);
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());

            NHibernateHelper.CurrentSession.Update(model);
        }

        public void Delete(string id)
        {
            TModel model = NHibernateHelper.CurrentSession.Get<TModel>(id);

            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public IList<TModel> Select()
        {
            IList<TModel> models = NHibernateHelper.CurrentSession.QueryOver<TModel>().List();
            return models;
        }
    }
}
