
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Exceptions;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class DateFilterExpression : FilterExpression
    {
        DateRange _range;
        public DateFilterExpression(Field field, DateTime? start, DateTime? end)
        {
            this._range = new DateRange(start, end);
            this.Field = field;
            this.Start = start;
            this.End = end;
        }

        public Field Field { private set; get; }

        public DateTime? Start { private set; get; }

        public DateTime? End { private set; get; }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }
            MetadataProperty property = metadata.GetProperty(this.Field.Code);
            if (property != null)
            {
                if (!(property.Value is DateMetadataValue))
                {
                    throw new ColdewException(string.Format("{0} 不是日期类型字段, 无法执行搜索", this.Field.Name));
                }

                DateMetadataValue value = property.Value as DateMetadataValue;
                return this._range.InRange(value.Date);
            }

            return true;
        }
    }
}
