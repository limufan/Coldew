using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Exceptions;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core.Search
{
    public class RecentlyDateFilterExpression : FilterExpression
    {
        public RecentlyDateFilterExpression(Field field, int? startDays, int? endDays)
        {
            this.StartDays = startDays;
            this.EndDays = endDays;
            this.Field = field;
        }

        public int? StartDays { private set; get; }

        public int? EndDays { private set; get; }

        public Field Field { private set; get; }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }

            MetadataValue value = metadata.GetValue(this.Field.Code);
            if (value != null)
            {
                if (!(value is DateMetadataValue))
                {
                    throw new ColdewException(string.Format("{0} 不是日期类型字段, 无法执行搜索", this.Field.Name));
                }

                DateMetadataValue dateValue = value as DateMetadataValue;
                DateTime? startDate = null;
                DateTime? endDate = null;
                if (this.StartDays.HasValue)
                {
                    startDate = DateTime.Now.AddDays(this.StartDays.Value);
                }
                if (this.EndDays.HasValue)
                {
                    endDate = DateTime.Now.AddDays(this.EndDays.Value);
                }
                DateRange range = new DateRange(startDate, endDate);
                return range.InRange(dateValue.Date);
            }

            return true;
        }
    }
}
