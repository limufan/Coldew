using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Coldew.Data;
using Coldew.Api;
using log4net.Util;
using Coldew.Core;
using Newtonsoft.Json;
using Coldew.Api.Exceptions;
using Coldew.Core.Search;
using Coldew.Api.UI;
using Coldew.Core.UI;
using Coldew.Core.DataProviders;

namespace Coldew.Core
{
    public class GridViewManager
    {
        protected Dictionary<string, GridView> _gridViewDicById;
        protected List<GridView> _gridViews;
        public ColdewObject ColdewObject;
        protected ReaderWriterLock _lock;
        FavoriteFilterExpression _favoriteExpression;
        internal GridViewDataProvider DataProvider;

        public GridViewManager(ColdewObject coldewObject)
        {
            this.DataProvider = new GridViewDataProvider(coldewObject);
            this.ColdewObject = coldewObject;
            this._gridViewDicById = new Dictionary<string, GridView>();
            this._favoriteExpression = new FavoriteFilterExpression(this.ColdewObject);
            this._gridViews = new List<GridView>();
            this._lock = new ReaderWriterLock();
        }

        private int MaxIndex()
        {
            if (this._gridViewDicById.Count == 0)
            {
                return 1;
            }
            return this._gridViewDicById.Values.Max(x => x.Index) + 1;
        }

        public GridView Create(GridViewCreateInfo createInfo)
        {
            GridView view = new GridView(Guid.NewGuid().ToString(), createInfo.Code, createInfo.Name, createInfo.Creator,
                    createInfo.IsShared, createInfo.IsSystem,this.MaxIndex(), createInfo.Columns, createInfo.Searcher, createInfo.OrderField, 
                    this);
            view.Footer = createInfo.Footer;
            this.DataProvider.Insert(view);
            this.BindEvent(view);
            this.Index(view);
            return view;
        }

        private void Index(GridView view)
        {
            this._gridViewDicById.Add(view.ID, view);
            this._gridViews.Add(view);
        }

        private void BindEvent(GridView view)
        {
            view.Deleted += new TEventHandler<GridView>(View_Deleted);
        }

        void View_Deleted(GridView args)
        {
            this._lock.AcquireReaderLock();
            try
            {
                this._gridViewDicById.Remove(args.ID);
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public GridView GetGridView(string viewId)
        {
            this._lock.AcquireReaderLock();
            try
            {
                if (this._gridViewDicById.ContainsKey(viewId))
                {
                    return this._gridViewDicById[viewId];
                }
                return null;
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<GridView> GetGridViews(User user)
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._gridViewDicById.Values.Where(x => x.HasPermission(user)).OrderBy(x => x.Index).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        public List<GridView> GetSystemGridViews()
        {
            this._lock.AcquireReaderLock();
            try
            {
                return this._gridViewDicById.Values.OrderBy(x => x.Index).ToList();
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        protected virtual GridView Create(GridViewModel model)
        {
            GridViewColumnMapper columnMapper = new GridViewColumnMapper(this.ColdewObject.ObjectManager);
            User creator = this.ColdewObject.ColdewManager.OrgManager.UserManager.GetUserByAccount(model.CreatorAccount);
            List<GridViewColumnModel> columnModels = JsonConvert.DeserializeObject<List<GridViewColumnModel>>(model.ColumnsJson);
            List<GridViewColumn> columns = columnModels.Select(x =>
                {
                    return columnMapper.MapColumn(x);
                }).ToList();
            Field orderByField = this.ColdewObject.GetFieldById(model.OrderFieldId);
            MetadataFilter filter = null;
            if (!string.IsNullOrEmpty(model.FilterJson))
            {
                MetadataFilterParser parser = new MetadataFilterParser(model.FilterJson, this.ColdewObject); 
                filter = parser.Parse();
            }

            GridView view = new GridView(model.ID, model.Code, model.Name, creator, model.IsShared, model.IsSystem,
                       model.Index, columns, filter, orderByField, this);   

            if (!string.IsNullOrEmpty(model.FooterJson))
            {
                view.Footer = JsonConvert.DeserializeObject<List<GridFooter>>(model.FooterJson);
            }
            return view;
        }

        internal void Load()
        {
            IList<GridViewModel> models = this.DataProvider.Select();
            foreach (GridViewModel model in models)
            {
                GridView view = this.Create(model);
                this.BindEvent(view);
                this.Index(view);
            }
            this._gridViews = this._gridViews.OrderBy(x => x.Index).ToList();
        }
    }
}
