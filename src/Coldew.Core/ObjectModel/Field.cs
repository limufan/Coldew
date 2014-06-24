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
        public Field(FieldNewInfo info)
        {
            this.ID = info.ID;
            this.Tip = info.Tip;
            this.Name = info.Name;
            this.Required = info.Required;
            this.IsSystem = info.IsSystem;
            this.Code = info.Code;
            this.ColdewObject = info.ColdewObject;
            this.Type = info.Type;
            this.Unique = info.Unique;
            this.IsSummary = info.IsSummary;
            this.GridWidth = info.GridWidth;
        }

        public int ID { set; get; }

        public string Code{ private set;get; }

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

        public ColdewObject ColdewObject { private set; get; }

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

        public virtual FieldInfo Map()
        {
            FieldInfo info = new FieldInfo();
            this.Fill(info, null);
            return info;
        }

        public abstract FieldInfo Map(User user);

        protected void Fill(FieldInfo info, User user)
        {
            info.Code = this.Code;
            info.ID = this.ID;
            info.Tip = this.Tip;
            info.Type = this.Type;
            info.Name = this.Name;
            info.Required = this.Required;
            info.Type = this.Type;
            info.TypeName = this.TypeName;
            info.Unique = this.Unique;
            if (user != null)
            {
                info.PermissionValue = this.ColdewObject.FieldPermission.GetPermission(user, this);
            }
        }
    }
}
