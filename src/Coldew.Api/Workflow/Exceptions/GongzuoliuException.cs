﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Coldew.Api.Workflow.Exceptions
{
    [Serializable]
    public class GongzuoliuException : ApplicationException
    {
        public GongzuoliuException()
        {
            
        }

        public GongzuoliuException(string message)
        {
            this.ExceptionMessage = message;
        }

        public GongzuoliuException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            this.ExceptionMessage = info.GetString("ExceptionMessage");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExceptionMessage", this.Message);
        }

        protected string ExceptionMessage { set; get; }

        public override string Message
        {
            get
            {
                return this.ExceptionMessage;
            }
        }
    }
}
