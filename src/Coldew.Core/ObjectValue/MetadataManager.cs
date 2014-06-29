using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Api;
using System.Threading;
using Coldew.Api.Exceptions;
using Coldew.Core.DataServices;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class MetadataManager
    {
        protected Dictionary<string, Metadata> _metadataDicById;
        protected List<Metadata> _metadataList;
        OrganizationManagement _orgManger;
        protected ReaderWriterLock _lock;

        public MetadataManager(ColdewObject cobject, OrganizationManagement orgManger)
        {
            this._metadataDicById = new Dictionary<string, Metadata>();
            this._metadataList = new List<Metadata>();
            this._orgManger = orgManger;
            this.ColdewObject = cobject;
            this._lock = new ReaderWriterLock();
            this.ColdewObject.FieldDeleted += new TEventHandler<Core.ColdewObject, Field>(ColdewObject_FieldDeleted);
        }

        void ColdewObject_FieldDeleted(ColdewObject sender, Field field)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                foreach (Metadata metadata in this._metadataList)
                {
                    metadata.RemoveFieldProperty(field);
                }
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public ColdewObject ColdewObject { private set; get; }

        public string GenerateCode(string fieldCode)
        {
            Field field = this.ColdewObject.GetFieldByCode(fieldCode);
            if(field == null)
            {
                throw new ColdewException(string.Format("找不到{0}字段", fieldCode));
            }
            if(!(field is CodeField))
            {
                throw new ColdewException(string.Format("编号{0}字段不是编码字段", fieldCode));
            }
            CodeField codeField = field as CodeField;
            string lastCode = "";
            if (this._metadataList.Count > 0)
            {
                Metadata lastCreatedMetadata = this._metadataList[0];
                MetadataProperty property = lastCreatedMetadata.GetProperty(codeField.Code);
                CodeMetadataValue codeValue = property.Value as CodeMetadataValue;
                lastCode = codeValue.Code;
            }
            return codeField.GenerateCode(lastCode);
        }

        public event TEventHandler<MetadataManager, JObject> Creating;

        public event TEventHandler<MetadataManager, Metadata> Created;

        public Metadata Create(User creator, JObject jobject)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                if (this.Creating != null)
                {
                    this.Creating(this, jobject);
                }
                List<Field> requiredFields = this.ColdewObject.GetRequiredFields();
                foreach (Field field in requiredFields)
                {
                    if (jobject[field.Code] == null || string.IsNullOrEmpty(jobject[field.Code].ToString()))
                    {
                        throw new ColdewException(string.Format("{0}不能空", field.Name));
                    }
                }

                Metadata metadata = this.Create(Guid.NewGuid().ToString(), jobject);
                this.ValidateUnique(metadata);

                this.ColdewObject.DataService.Create(metadata);

                this._metadataDicById.Add(metadata.ID, metadata);
                this._metadataList.Insert(0, metadata);

                this.BindEvent(metadata);
                if (this.Created != null)
                {
                    this.Created(this, metadata);
                }
                return metadata;
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        private void ValidateUnique(Metadata metadata)
        {
            List<Field> uniqueFields = this.ColdewObject.GetUniqueFields();
            foreach (Field field in uniqueFields)
            {
                if (this._metadataList.Any(x =>
                {
                    if (metadata != x)
                    {
                        MetadataProperty property = x.GetProperty(field.Code);
                        MetadataProperty property1 = metadata.GetProperty(field.Code);
                        if (property != null && property1 != null)
                        {
                            return property.Value.Equals(property1.Value);
                        }
                    }
                    return false;
                }))
                {
                    throw new ColdewException(string.Format("{0}不能重复", field.Name));
                }
            }
        }

        protected virtual Metadata Create(string id, JObject jobject)
        {
            List<MetadataProperty> propertys = MetadataPropertyListHelper.MapPropertys(jobject, this.ColdewObject);
            Metadata metadata = new Metadata(id, propertys, this.ColdewObject);
            return metadata;
        }

        protected virtual void BindEvent(Metadata metadata)
        {
            metadata.PropertyChanging += new TEventHandler<MetadataChangingEventArgs>(Metadata_Changing);
            metadata.PropertyChanged += Metadata_PropertyChanged;
            metadata.Deleting += Metadata_Deleting;
            metadata.Deleted += new TEventHandler<Metadata, User>(Metadata_Deleted);
        }

        public event TEventHandler<MetadataManager, Metadata> MetadataDeleting;

        void Metadata_Deleting(Metadata metadata, User args)
        {
            if (this.MetadataDeleting != null)
            {
                this.MetadataDeleting(this, metadata);
            }
        }

        public event TEventHandler<MetadataManager, Metadata> MetadataDeleted;

        void Metadata_Deleted(Metadata metadata, User opUser)
        {
            this._lock.AcquireWriterLock(0);
            try
            {
                this._metadataDicById.Remove(metadata.ID);
                this._metadataList.Remove(metadata);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
            if (this.MetadataDeleted != null)
            {
                this.MetadataDeleted(this, metadata);
            }
        }

        public event TEventHandler<MetadataManager, MetadataChangingEventArgs> MetadataChanging;

        void Metadata_Changing(MetadataChangingEventArgs args)
        {
            if (MetadataChanging != null)
            {
                this.MetadataChanging(this, args);
            }
            this._lock.AcquireWriterLock(0);
            try
            {
                this.ValidateUnique(args.Metadata);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public event TEventHandler<MetadataManager, MetadataChangingEventArgs> MetadataChanged;

        void Metadata_PropertyChanged(MetadataChangingEventArgs args)
        {
            if (MetadataChanged != null)
            {
                this.MetadataChanged(this, args);
            }
        }

        public List<Metadata> GetList(User user, int skipCount, int takeCount, string orderBy, out int totalCount)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                var metadatasEnumer = this._metadataList.Where(x => x.CanPreview(user));
                var metadatas = metadatasEnumer.OrderBy(orderBy).ToList();
                totalCount = metadatas.Count;
                return metadatas.Skip(skipCount).Take(takeCount).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Metadata> GetList(User user, string orderBy)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                var metadatasEnumer = this._metadataList.Where(x => x.CanPreview(user));
                return metadatasEnumer.OrderBy(orderBy).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Metadata> GetRelatedList(ColdewObject cObject, string metadataId, string orderBy)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                var metadatasEnumer = this._metadataList.Where(x => {
                    MetadataProperty property = x.GetPropertyByObject(cObject);
                    if (property != null)
                    {
                        MetadataRelatedValue value = property.Value as MetadataRelatedValue;
                        return value.Metadata.ID == metadataId;
                    }
                    return false;
                });

                return metadatasEnumer.OrderBy(orderBy).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Metadata GetById(string id)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                if (this._metadataDicById.ContainsKey(id))
                {
                    return this._metadataDicById[id];
                }
                return null;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public Metadata GetByName(string name)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                return this._metadataList.Find(x => x.Name == name);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Metadata> Search(User user, List<MetadataSearcher> serachers)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                List<Metadata> metadatas = null;
                if (serachers == null || serachers.Count == 0)
                {
                    metadatas = this._metadataList.Where(x => x.CanPreview(user)).ToList();
                }
                else
                {
                    metadatas = this._metadataList.Where(x => x.CanPreview(user) && serachers.All(s =>s.Accord(user, x))).ToList();
                }
                return metadatas;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<Metadata> Search(User user, List<MetadataSearcher> serachers, string orderBy)
        {
            this._lock.AcquireReaderLock(0);
            try
            {
                List<Metadata> metadatas = null;
                if (serachers == null || serachers.Count == 0)
                {
                    metadatas = this._metadataList.Where(x => x.CanPreview(user)).OrderBy(orderBy).ToList();
                }
                else
                {
                    metadatas = this._metadataList.Where(x => x.CanPreview(user) && serachers.All(s =>s.Accord(user, x))).OrderBy(orderBy).ToList();
                }
                return metadatas;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        internal virtual void Load()
        {
            List<Metadata> metadatas = this.ColdewObject.DataService.LoadFromDB();
            foreach (Metadata metadata in metadatas)
            {
                this._metadataDicById.Add(metadata.ID, metadata);
                this._metadataList.Add(metadata);

                this.BindEvent(metadata);
            }
            this._metadataList = this._metadataList.ToList();
        }
    }
}
