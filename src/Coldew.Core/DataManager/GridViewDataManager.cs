using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.UI;

namespace Coldew.Core.DataManager
{
    public class GridViewDataManager
    {
        internal GridViewDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public GridViewDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new GridViewDataProvider(cobject);
            cobject.GridViewManager.Created += GridViewManager_Created;
            this.Load();
        }

        void GridViewManager_Created(GridViewManager sender, CreatedEventArgs<GridViewCreateInfo, GridView> args)
        {
            this.DataProvider.Insert(args.CreatedObject);
            this.BindColdewObjectEvent(args.CreatedObject);
        }

        private void BindColdewObjectEvent(GridView view)
        {
            view.Deleted += View_Deleted;
            view.Modified += View_Modified;
        }

        void View_Modified(GridView args)
        {
            this.DataProvider.Update(args);
        }

        void View_Deleted(GridView args)
        {
            this.DataProvider.Delete(args);
        }

        void Load()
        {
            List<GridView> views = this.DataProvider.Select();
            this._cobject.GridViewManager.AddGridViews(views);
            foreach (GridView view in views)
            {
                this.BindColdewObjectEvent(view);
            }
        }
    }
}
