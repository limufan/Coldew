using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public abstract class SearchExpression
    {
        public SearchExpression(Field field)
        {
            this.Field = field;
        }

        public Field Field { set; get; }

        public bool Compare(User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }

            return this._Compare(opUser, metadata);
        }

        protected abstract bool _Compare(User opUser, Metadata metadata);
    }
}
