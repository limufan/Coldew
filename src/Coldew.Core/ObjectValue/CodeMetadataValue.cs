using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Coldew.Api;
using Coldew.Api.Exceptions;
using Newtonsoft.Json.Linq;

namespace Coldew.Core
{
    public class CodeMetadataValue : MetadataValue
    {
        public CodeMetadataValue(string value, Field field)
            :base(value, field)
        {
            CodeField codeField = field as CodeField;
            this.Year = this.TrySubstring(value, codeField.YearFormatPosition, codeField.YearLength);
            this.Month = this.TrySubstring(value, codeField.MonthStartPosition, 2);
            string codeSerialNumber = this.TrySubstring(value, codeField.SerialNumberStartPosition, codeField.SerialNumberLength);
            int serialNumber = 0;
            if (int.TryParse(codeSerialNumber, out serialNumber))
            {
                this.SerialNumber = serialNumber;
            }
        }

        public string Code
        {
            get
            {
                return this.Value;
            }
        }

        public string Year { private set; get; }

        public string Month { private set; get; }

        public int SerialNumber { private set; get; }

        public override JToken PersistenceValue
        {
            get { return this.Code; }
        }

        public override string ShowValue
        {
            get { return this.Code; }
        }

        public override dynamic OrderValue
        {
            get { return this.Code; }
        }

        public override JToken JTokenValue
        {
            get
            {
                return this.Code;
            }
        }

        private string TrySubstring(string str, int start, int length)
        {
            try
            {
                return str.Substring(start, length);
            }
            catch
            {
                return "";
            }
        }
    }
}
