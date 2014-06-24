using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Coldew.Core.DataServices
{
    public class MetadataDataService
    {
        protected ColdewObject _cobject;
        public MetadataDataService(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public virtual void Create(Metadata metadata)
        {
            MetadataModel model = new MetadataModel();
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());
            model.ObjectId = this._cobject.ID;
            model.ID = metadata.ID;
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public virtual void Update(Metadata metadata)
        {
            MetadataModel model = NHibernateHelper.CurrentSession.Get<MetadataModel>(metadata.ID);
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());

            NHibernateHelper.CurrentSession.Update(model);
        }

        public virtual void Delete(string id)
        {
            MetadataModel model = NHibernateHelper.CurrentSession.Get<MetadataModel>(id);

            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public virtual List<Metadata> LoadFromDB()
        {
            List<Metadata> metadatas = new List<Metadata>();

            IList<MetadataModel> models = NHibernateHelper.CurrentSession.QueryOver<MetadataModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            foreach (MetadataModel model in models)
            {
                JObject jobject = JsonConvert.DeserializeObject<JObject>(model.PropertysJson);
                List<MetadataProperty> propertys = MetadataPropertyListHelper.MapPropertys(jobject, this._cobject);
                Metadata metadata = new Metadata(model.ID, propertys, this._cobject);
                metadatas.Add(metadata);
            }
            return metadatas;
        }
    }
}
