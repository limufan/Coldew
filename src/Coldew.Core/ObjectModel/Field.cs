using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json;
using Coldew.Core.Organization;
using Coldew.Data;
using Newtonsoft.Json.Linq;


namespace Coldew.Core
{
    public abstract class Field
    {
        internal Field()
        {
            
        }

        public string ID { set; get; }

        public string Code{ set;get; }

        public string Name { set; get; }

        public string Tip { set; get; }

        public bool Required { set; get; }

        public bool IsSystem { set; get; }

        public int GridWidth { set; get; }

        public bool IsSummary { set; get; }

        public bool Unique { set; get; }

        public string Type { set; get; }

        public abstract string TypeName { get; }

        public abstract MetadataValue CreateMetadataValue(JToken value);

        public ColdewObject ColdewObject { set; get; }

        public event TEventHandler<Field, FieldModifyArgs> Modifying;
        public event TEventHandler<Field, FieldModifyArgs> Modified;

        protected void OnModifying(FieldModifyArgs modifyInfo)
        {
            if (this.Modifying != null)
            {
                this.Modifying(this, modifyInfo);
            }
        }

        protected void OnModifyed(FieldModifyArgs modifyInfo)
        {
            if (this.Modified != null)
            {
                this.Modified(this, modifyInfo);
            }
        }

        public event TEventHandler<Field, User> Deleting;
        public event TEventHandler<Field, User> Deleted;

        public void Delete(User opUser)
        {
            if (this.Deleting != null)
            {
                this.Deleting(this, opUser);
            }

            FieldModel model = NHibernateHelper.CurrentSession.Get<FieldModel>(this.ID);

            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();

            if (this.Deleted != null)
            {
                this.Deleted(this, opUser);
            }
        }

        public bool CanView(User user)
        {
            return this.ColdewObject.FieldPermission.HasValue(user, FieldPermissionValue.View, this);
        }
    }
}
