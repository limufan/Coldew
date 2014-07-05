using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Search;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.DataProviders
{
    public class GridViewDataProvider
    {
        ColdewObject _cobject;

        public GridViewDataProvider(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public void Insert(GridView gridview)
        {
            var columnModels = gridview.Columns.Select(x => new GridViewColumnModel { FieldId = x.Field.ID });
            string columnJson = JsonConvert.SerializeObject(columnModels);
            string footerJson = JsonConvert.SerializeObject(gridview.Footer);
            GridViewModel model = new GridViewModel
            {
                ID = Guid.NewGuid().ToString(),
                CreatorAccount = gridview.Creator.Account,
                IsSystem = gridview.IsSystem,
                ObjectId = gridview.ColdewObject.ID,
                Name = gridview.Name,
                ColumnsJson = columnJson,
                IsShared = gridview.IsShared,
                Code = gridview.Code,
                Index = gridview.Index,
                OrderFieldId = gridview.OrderField.ID,
                FooterJson = footerJson
            };
            if(gridview.Filter != null)
            {
                model.FilterJson = JsonConvert.SerializeObject(this.Map(gridview.Filter), TypificationJsonSettings.JsonSettings);
            }
            NHibernateHelper.CurrentSession.Save(model).ToString();
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Update(GridView view)
        {
            GridViewModel model = NHibernateHelper.CurrentSession.Get<GridViewModel>(view.ID);
            var columnModels = view.Columns.Select(x => new GridViewColumnModel { FieldId = x.Field.ID });
            model.ColumnsJson = JsonConvert.SerializeObject(columnModels);
            if (view.Filter != null)
            {
                model.FilterJson = JsonConvert.SerializeObject(this.Map(view.Filter), TypificationJsonSettings.JsonSettings);
            }
            model.Name = view.Name;
            model.IsShared = view.IsShared;
            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public void Delete(GridView view)
        {
            GridViewModel model = NHibernateHelper.CurrentSession.Get<GridViewModel>(view.ID);
            NHibernateHelper.CurrentSession.Delete(model);
            NHibernateHelper.CurrentSession.Flush();
        }

        public IList<GridViewModel> Select()
        {
            IList<GridViewModel> models = NHibernateHelper.CurrentSession.QueryOver<GridViewModel>().Where(x => x.ObjectId == this._cobject.ID).List();
            return models;
        }

        public MetadataFilterModel Map(MetadataFilter filter)
        {
            MetadataFilterModel model = new MetadataFilterModel();
            model.expressions = new List<FilterExpressionModel>();
            foreach (dynamic expression in filter.Expressions)
            {
                FilterExpressionModel expressionModel = this.Map(expression);
                model.expressions.Add(expressionModel);
            }

            return model;
        }

        public FavoriteFilterExpressionModel Map(FavoriteFilterExpression expression)
        {
            return new FavoriteFilterExpressionModel();
        }

        public KeywordFilterExpressionModel Map(KeywordFilterExpression expression)
        {
            return new KeywordFilterExpressionModel { keyword = expression.Keyword };
        }

        public StringFilterExpressionModel Map(StringFilterExpression expression)
        {
            return new StringFilterExpressionModel { fieldId = expression.Field.ID, keyword = expression.Keyword };
        }

        public DateFilterExpressionModel Map(DateFilterExpression expression)
        {
            return new DateFilterExpressionModel { fieldId = expression.Field.ID, end = expression.End, start = expression.Start };
        }

        public NumberFilterExpressionModel Map(NumberFilterExpression expression)
        {
            return new NumberFilterExpressionModel { fieldId = expression.Field.ID, max = expression.Max, min = expression.Min };
        }

        public OperatorFilterExpressionModel Map(OperatorFilterExpression expression)
        {
            return new OperatorFilterExpressionModel { fieldId = expression.Field.ID};
        }

        public RecentlyDateFilterExpressionModel Map(RecentlyDateFilterExpression expression)
        {
            return new RecentlyDateFilterExpressionModel { fieldId = expression.Field.ID, endDays = expression.EndDays, startDays = expression.StartDays };
        }
    }
}
