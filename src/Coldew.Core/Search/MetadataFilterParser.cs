using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Data;
using Newtonsoft.Json;

namespace Coldew.Core.Search
{
    public class MetadataFilterParser
    {
        ColdewObject _coldewObject;
        string _expressionJson;
        public MetadataFilterParser(string expressionJson, ColdewObject coldewObject)
        {
            this._expressionJson = expressionJson;
            this._coldewObject = coldewObject;
        }

        public MetadataFilter Parse()
        {
            MetadataFilterModel filterModel = JsonConvert.DeserializeObject<MetadataFilterModel>(this._expressionJson, TypificationJsonSettings.JsonSettings);
            MetadataFilter filter = this.Map(filterModel);
            return filter;
        }
        private MetadataFilter Map(MetadataFilterModel model)
        {
            List<FilterExpression> expressions = new List<FilterExpression>();
            foreach (dynamic expressionModel in model.expressions)
            {
                expressions.Add(this.Map(expressionModel));
            }
            return new MetadataFilter(expressions);
        }

        private FavoriteFilterExpression Map(FavoriteFilterExpressionModel model)
        {
            return new FavoriteFilterExpression(this._coldewObject);
        }

        public KeywordFilterExpression Map(KeywordFilterExpressionModel model)
        {
            return new KeywordFilterExpression(model.keyword);
        }

        public StringFilterExpression Map(StringFilterExpressionModel model)
        {
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            return new StringFilterExpression(field, model.keyword);
        }

        public DateFilterExpression Map(DateFilterExpressionModel model)
        {
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            return new DateFilterExpression(field, model.start, model.end);
        }

        public NumberFilterExpression Map(NumberFilterExpressionModel model)
        {
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            return new NumberFilterExpression(field, model.min, model.max);
        }

        public OperatorFilterExpression Map(OperatorFilterExpressionModel model)
        {
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            return new OperatorFilterExpression(field);
        }

        public RecentlyDateFilterExpression Map(RecentlyDateFilterExpressionModel model)
        {
            Field field = this._coldewObject.GetFieldById(model.fieldId);
            return new RecentlyDateFilterExpression(field, model.startDays, model.endDays);
        }
    }
}
