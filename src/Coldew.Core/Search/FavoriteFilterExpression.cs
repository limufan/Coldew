using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;

namespace Coldew.Core.Search
{
    public class FavoriteFilterExpression : FilterExpression
    {
        ColdewObject _cobject;
        public FavoriteFilterExpression(ColdewObject cobject)
        {
            this._cobject = cobject;
        }

        public override bool IsTrue(User opUser, Metadata metadata)
        {
            return this._cobject.FavoriteManager.IsFavorite(opUser, metadata);
        }
    }
}
