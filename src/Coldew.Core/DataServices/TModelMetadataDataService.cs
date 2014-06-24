using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Coldew.Core.DataServices
{
    public class TModelMetadataDataService<TModel> : MetadataDataService 
        where TModel :TModelMetadataModel 
    {
        public TModelMetadataDataService(ColdewObject cobject)
            : base(cobject)
        {
            
        }

        public override void Create(Metadata metadata)
        {
            TModel model = Activator.CreateInstance<TModel>();
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());
            model.ID = metadata.ID;
            NHibernateHelper.CurrentSession.Save(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public override void Update(Metadata metadata)
        {
            TModel model = NHibernateHelper.CurrentSession.Get<TModel>(metadata.ID);
            model.PropertysJson = MetadataPropertyListHelper.ToPropertyModelJson(metadata.GetPropertys());

            NHibernateHelper.CurrentSession.Update(model);
        }

        public override void Delete(string id)
        {
            TModel model = NHibernateHelper.CurrentSession.Get<TModel>(id);

            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public override List<Metadata> LoadFromDB()
        {
            List<Metadata> metadatas = new List<Metadata>();

            IList<TModel> models = NHibernateHelper.CurrentSession.QueryOver<TModel>().List();
            foreach (TModel model in models)
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
