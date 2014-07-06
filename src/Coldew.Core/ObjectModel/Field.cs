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

        public string ID { internal set; get; }

        public string Code { internal set; get; }

        public string Name { internal set; get; }

        public string Tip { internal set; get; }

        public bool Required { internal set; get; }

        public bool IsSystem { internal set; get; }

        public int GridWidth { internal set; get; }

        public bool IsSummary { internal set; get; }

        public bool Unique { internal set; get; }

        public abstract string Type { get; }

        public abstract string TypeName { get; }

        public abstract MetadataValue CreateMetadataValue(JToken value);

        public ColdewObject ColdewObject { internal set; get; }

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
