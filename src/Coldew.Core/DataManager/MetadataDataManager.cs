using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;

namespace Coldew.Core.DataManager
{
    public class MetadataDataManager
    {
        internal MetadataDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public MetadataDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new MetadataDataProvider(cobject);
            cobject.MetadataManager.Created += MetadataManager_Created;
            this.Load();
        }

        void MetadataManager_Created(MetadataManager sender, Metadata args)
        {
            this.DataProvider.Insert(args);
            this.BindColdewObjectEvent(args);
        }

        private void BindColdewObjectEvent(Metadata metadata)
        {
            metadata.Deleted += Metadata_Deleted;
            metadata.Changed += Metadata_Changed;
        }

        void Metadata_Changed(Metadata metadata, MetadataChangeInfo changeInfo)
        {
            this.DataProvider.Update(metadata);
        }

        void Metadata_Deleted(Metadata sender, Organization.User args)
        {
            this.DataProvider.Delete(sender.ID);
        }

        void Load()
        {
            List<Metadata> metadatas = this.DataProvider.Select();
            this._cobject.MetadataManager.AddMetadatas(metadatas);
            foreach (Metadata metadata in metadatas)
            {
                this.BindColdewObjectEvent(metadata);
            }
        }
    }
}
