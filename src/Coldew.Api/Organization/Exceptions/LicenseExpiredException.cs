﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Coldew.Api.Organization.Exceptions
{
    [Serializable]
    public class LicenseExpiredException : LicenseException
    {
        public LicenseExpiredException()
        {
            this.ExceptionMessage = "License已过期!";
        }

        public LicenseExpiredException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            
        }
        
        public override string Message
        {
            get
            {
                if (this.StringResouceProvider != null)
                {
                    return this.StringResouceProvider.GetString("ex_licenseExpired");
                }
                return base.Message;
            }
        }
    }
}
