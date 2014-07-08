
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Exceptions;
using Coldew.Api;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class NumberFilterExpression : FilterExpression
    {
        NumberRange _range;
        public NumberFilterExpression(Field field, decimal? min, decimal? max)
        {
            this._range = new NumberRange(min, max);
            this.Field = field;
            this.Min = min;
            this.Max = max;
        }

        public Field Field { private set; get; }

        public decimal? Min { private set; get; }

        public decimal? Max { private set; get; }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }
            MetadataValue value = metadata.GetValue(this.Field.Code);
            if (value != null)
            {
                if (!(value is NumberMetadataValue))
                {
                    throw new ColdewException(string.Format("{0} 不是数字类型字段, 无法执行搜索", this.Field.Name));
                }

                NumberMetadataValue numberValue = value as NumberMetadataValue;
                return this._range.InRange(numberValue.Number);
            }

            return true;
        }
    }
}
