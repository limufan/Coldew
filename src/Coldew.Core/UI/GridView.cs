using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data;
using Coldew.Api;
using Coldew.Core;
using log4net.Util;
using Newtonsoft.Json;
using Coldew.Core.Search;
using Coldew.Core.UI;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class GridView
    {
        ReaderWriterLock _lock;
        GridViewManager _gridViewManager;
        public GridView(string id, string code, string name, User creator, bool isShared, bool isSystem, 
            int index, List<GridViewColumn> columns, MetadataFilter filter, Field orderField, ColdewObject cobject)
        {
            this.ID = id;
            this.Code = code;
            this.Name = name;
            this.Creator = creator;
            this.IsShared = isShared;
            this.IsSystem = isSystem;
            this.Index = index;
            this.Filter = filter;
            this._gridViewManager = cobject.GridViewManager;
            this.ColdewObject = cobject;
            this.Columns = columns;
            this.OrderField = orderField;
            if (this.Columns == null)
            {
                this.Columns = new List<GridViewColumn>();
            }
            this._lock = new ReaderWriterLock();
            this.ColdewObject.FieldDeleted += new TEventHandler<Core.ColdewObject, Field>(ColdewObject_FieldDeleted);
        }

        void ColdewObject_FieldDeleted(ColdewObject sender, Field field)
        {
            this.RemoveFieldColumn(field);
        }

        public string ID { private set; get; }

        public string Code { private set; get; }

        public string Name { private set; get; }

        public User Creator { private set; get; }

        public bool IsShared { private set; get; }

        public bool IsSystem { private set; get; }

        public int Index { private set; get; }

        public MetadataFilter Filter { private set; get; }

        public Field OrderField { private set; get; }

        public List<GridViewColumn> Columns { private set; get; }

        public List<GridFooter> Footer { internal set; get; }

        public ColdewObject ColdewObject { private set; get; }

        public event TEventHandler<GridView> Modified;

        public void Modify(string name, bool isShared, MetadataFilter filter, List<GridViewColumn> columns)
        {
            this._lock.AcquireWriterLock();
            try
            {
                this.Columns = columns;
                this.Filter = filter;
                this.IsShared = isShared;
                this.Name = name;

                if (this.Modified != null)
                {
                    this.Modified(this);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public void SetColumns(List<GridViewColumn> columns)
        {
            this._lock.AcquireWriterLock();
            try
            {
                this.Columns = columns;

                if (this.Modified != null)
                {
                    this.Modified(this);
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public event TEventHandler<GridView> Deleting;
        public event TEventHandler<GridView> Deleted;

        public void Delete()
        {
            if (this.Deleting != null)
            {
                this.Deleting(this);
            }

            if (this.Deleted != null)
            {
                this.Deleted(this);
            }
        }

        private void RemoveFieldColumn(Field field)
        {
            this._lock.AcquireWriterLock();
            try
            {
                List<GridViewColumn> columns = this.Columns.ToList();
                columns.RemoveAll(x => { 
                    if(x != null)
                    {
                        return x.Field == field;
                    }
                    return false;
                });
                this.SetColumns(columns);
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public bool HasPermission(User user)
        {
            if (this.IsShared)
            {
                return true;
            }
            if (this.Creator == user)
            {
                return true;
            }
            return false;
        }

        public JObject GetJObject(Metadata metadata, User opUser)
        {
            JObject jobject = new JObject();
            jobject.Add("id", metadata.ID);
            jobject.Add("summary", metadata.GetSummary());
            bool favorited = metadata.ColdewObject.FavoriteManager.IsFavorite(opUser, metadata);
            jobject.Add("favorited", favorited);
            MetadataPermissionValue permission = metadata.ColdewObject.MetadataPermission.GetValue(opUser, metadata);
            jobject.Add("canModify", permission.HasFlag(MetadataPermissionValue.Modify));
            jobject.Add("canDelete", permission.HasFlag(MetadataPermissionValue.Delete));
            foreach (GridViewColumn column in this.Columns)
            {
                MetadataValue value = column.GetValue(metadata);
                if (value != null)
                {
                    jobject.Add(value.Field.Code, value.ShowValue);
                }
            }
            return jobject;
        }
    }
}
