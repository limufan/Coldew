using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Data
{
    public class GridViewModel
    {
        public virtual string ID { set; get; }

        public virtual string Code { set; get; }

        public virtual string Name { set; get; }

        public virtual string ObjectId { set; get; }

        public virtual string CreatorAccount { set; get; }

        public virtual bool IsShared { set; get; }

        public virtual bool IsSystem { set; get; }

        public virtual int Index { set; get; }

        public virtual string ColumnsJson { set; get; }

        public virtual string FilterJson { set; get; }

        public virtual string OrderFieldId { set; get; }

        public virtual string FooterJson { set; get; }
    }

    public class GridViewColumnModel
    {
        public string fieldId;
    }

    public class MetadataFilterModel
    {
        public List<FilterExpressionModel> expressions;
    }

    public class FilterExpressionModel
    {

    }

    public class FavoriteFilterExpressionModel : FilterExpressionModel
    {

    }

    public class KeywordFilterExpressionModel : FilterExpressionModel
    {
        public string keyword;
    }

    public class StringFilterExpressionModel : FilterExpressionModel
    {
        public string fieldId;

        public string keyword;
    }

    public class DateFilterExpressionModel : FilterExpressionModel
    {
        public string fieldId;

        public DateTime? start;

        public DateTime? end;
    }

    public class NumberFilterExpressionModel : FilterExpressionModel
    {
        public string fieldId;

        public decimal? min;

        public decimal? max;
    }

    public class OperatorFilterExpressionModel : FilterExpressionModel
    {
        public string fieldId;
    }

    public class RecentlyDateFilterExpressionModel : FilterExpressionModel
    {
        public string fieldId;
        
        public int? startDays;

        public int? endDays;
    }
}
