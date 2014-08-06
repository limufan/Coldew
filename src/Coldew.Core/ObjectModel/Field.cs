using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Newtonsoft.Json;
using Coldew.Core.Organization;

using Newtonsoft.Json.Linq;


namespace Coldew.Core
{
    public abstract class Field
    {
        public Field(FieldNewInfo newInfo)
        {
            ClassPropertyHelper.ChangeProperty(newInfo, this);
        }

        public string ID { set; get; }

        public string Code { set; get; }

        public string Name { set; get; }

        public string Tip { set; get; }

        public bool Required { set; get; }

        public bool IsSystem { set; get; }

        public int GridWidth { set; get; }

        public bool IsSummary { set; get; }

        public bool Unique { set; get; }

        public abstract string Type { get; }

        public abstract string TypeName { get; }

        public abstract MetadataValue CreateMetadataValue(JToken value);

        public ColdewObject ColdewObject { set; get; }

        public event TEventHandler<Field, FieldChangeInfo> Changing;
        public event TEventHandler<Field, FieldChangeInfo> Changed;

        protected void OnChanging(FieldChangeInfo changeInfo)
        {
            if (this.Changing != null)
            {
                this.Changing(this, changeInfo);
            }
        }

        protected void OnChanged(FieldChangeInfo modifyInfo)
        {
            if (this.Changed != null)
            {
                this.Changed(this, modifyInfo);
            }
        }

        public void Change(FieldChangeInfo changeInfo)
        {
            this.OnChanging(changeInfo);

            ClassPropertyHelper.ChangeProperty(changeInfo, this);

            this.OnChanged(changeInfo);
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
