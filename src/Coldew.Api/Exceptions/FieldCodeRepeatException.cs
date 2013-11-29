﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Coldew.Api.Exceptions
{
    [Serializable]
    public class FieldCodeRepeatException : ColdewException
    {
        public FieldCodeRepeatException()
        {
            this.ExceptionMessage = "code重复!";
        }

        public FieldCodeRepeatException(string message)
        {
            this.ExceptionMessage = message;
        }

        public FieldCodeRepeatException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            this.ExceptionMessage = info.GetString("ExceptionMessage");
        }
    }
}
