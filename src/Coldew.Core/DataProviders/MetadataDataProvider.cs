
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class MetadataDataProvider
    {
        ColdewObjectManager _objectManager;
        public MetadataDataProvider(ColdewObjectManager objectManager)
        {
            this._objectManager = objectManager;
        }

        public virtual void Insert(Metadata metadata)
        {
            MetadataModel model = new MetadataModel();
            model.PropertysJson = this.GetPersistenceJson(metadata);
            model.ObjectId = metadata.ColdewObject.ID;
            model.ID = metadata.ID;
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public virtual void Update(Metadata metadata)
        {
            MetadataModel model = NHibernateHelper.CurrentSession.Get<MetadataModel>(metadata.ID);
            model.PropertysJson = this.GetPersistenceJson(metadata);

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public virtual void Delete(Metadata metadata)
        {
            MetadataModel model = NHibernateHelper.CurrentSession.Get<MetadataModel>(metadata.ID);

            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public virtual void Load()
        {
            IList<MetadataModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataModel>().List();
            foreach (MetadataModel model in models)
            {
                ColdewObject cobject = this._objectManager.GetObjectById(model.ObjectId);
                Metadata metadata = cobject.MetadataManager.MetadataFactory.Create(model);
                cobject.MetadataManager.AddMetadata(metadata);
            }
        }

        public string GetPersistenceJson(Metadata metadata)
        {
            List<MetadataValue> values = metadata.GetValue();

            JObject jobject = new JObject();
            foreach (MetadataValue value in values)
            {
                jobject.Add(value.Field.ID, value.PersistenceValue);
            }
            return JsonConvert.SerializeObject(jobject);
        }
    }
}
