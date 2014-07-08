using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class MetadataFieldMember : MetadataMember
    {
        Field _field;
        public MetadataFieldMember(Field field)
        {
            this._field = field;
        }

        public override bool Contains(Metadata metadata, User user)
        {
            MetadataValue value = metadata.GetValue(this._field.Code);
            if (value != null)
            {
                if (value is UserMetadataValue)
                {
                    UserMetadataValue userValue = value as UserMetadataValue;
                    return userValue.User == user;
                }
                else if (value is UserListMetadataValue)
                {
                    UserListMetadataValue userListValue = value as UserListMetadataValue;
                    return userListValue.Users.Contains(user);
                }
            }
            return false;
        }

        public override string Serialize()
        {
            return "field:" + this._field.Code;
        }
    }
}
