using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace Coldew.Core
{
    internal class TypificationJsonSettings
    {
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple };
    }
}
