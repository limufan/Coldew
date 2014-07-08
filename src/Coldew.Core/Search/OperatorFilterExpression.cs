using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Exceptions;

namespace Coldew.Core.Search
{
    public class OperatorFilterExpression : FilterExpression
    {
        public OperatorFilterExpression(Field field)
        {
            this.Field = field;
        }

        public Field Field { private set; get; }

        public override bool IsTrue(Organization.User opUser, Metadata metadata)
        {
            if (!this.Field.CanView(opUser))
            {
                return false;
            }
            MetadataValue value = metadata.GetValue(this.Field.Code);
            if (value != null)
            {
                if (!(value is UserMetadataValue) && !(value is UserListMetadataValue))
                {
                    throw new ColdewException(string.Format("{0} 不是用户类型字段, 无法执行搜索", this.Field.Name));
                }

                if (value is UserMetadataValue)
                {
                    UserMetadataValue userMetadataValue = value as UserMetadataValue;
                    return userMetadataValue.User == opUser;
                }
                else
                {
                    UserListMetadataValue userListMetadataValue = value as UserListMetadataValue;
                    return userListMetadataValue.Users.Contains(opUser);
                }
            }

            return true;
        }
    }
}
