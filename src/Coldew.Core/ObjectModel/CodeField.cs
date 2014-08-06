using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;

using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;
using System.Text.RegularExpressions;
using Coldew.Api.Exceptions;

namespace Coldew.Core
{
    public class CodeField : Field
    {
        public CodeField(CodeFieldNewInfo newInfo)
            : base(newInfo)
        {

        }

        public override string Type
        {
            get { return FieldType.Code; }
        }

        public override string TypeName
        {
            get { return "编码"; }
        }

        private string _format;
        public string Format
        {
            set
            {
                this._format = value;
                this.Parse();
            }
            get
            {
                return this._format;
            }
        }

        private void Parse()
        {
            string format = this._format;
            if (format.IndexOf("yyyy") > -1)
            {
                this.YearFormat = "yyyy";
                this.YearFormatPosition = format.IndexOf("yyyy");
                this.YearLength = 4;
            }
            else if (format.IndexOf("yy") > -1)
            {
                this.YearFormat = "yy";
                this.YearFormatPosition = format.IndexOf("yy");
                this.YearLength = 2;
            }

            if (format.IndexOf("MM") > -1)
            {
                this.MonthFormat = "MM";
                this.MonthStartPosition = format.IndexOf("MM");
            }

            Regex snLengthRegex = new Regex(@"SN\{(\d+)\}");
            if (snLengthRegex.IsMatch(format))
            {
                Match match = snLengthRegex.Match(format);
                int length;
                if (!int.TryParse(match.Groups[1].Value, out length))
                {
                    throw new ColdewException(string.Format("流水号长度{0}字符不正确", match.Groups[1].Value));
                }
                this.SerialNumberLength = length;
                this.SerialNumberFormat = match.Groups[0].Value;
                this.SerialNumberStartPosition = match.Index;
            }
        }

        public int YearFormatPosition { private set; get; }

        public string YearFormat { private set; get; }

        public int YearLength { private set; get; }

        public string MonthFormat { private set; get; }

        public int MonthStartPosition { private set; get; }

        public string SerialNumberFormat { private set; get; }

        public int SerialNumberStartPosition { private set; get; }

        public int SerialNumberLength { private set; get; }

        public CodeMetadataValue GenerateCode()
        {
            List<Metadata> metadatas = this.ColdewObject.MetadataManager.GetList();
            metadatas = metadatas.Where(x =>
                {
                    CodeMetadataValue value = x.GetValue(this.Code) as CodeMetadataValue;
                    return value.Year == SystemTime.Now.ToString(this.YearFormat) && value.Month == SystemTime.Now.ToString(this.MonthFormat);
                })
                .ToList();
            int maxSerialNumber = 1;
            if (metadatas.Count > 0)
            {
                maxSerialNumber = metadatas.Max(x =>
                    {
                        CodeMetadataValue value = x.GetValue(this.Code) as CodeMetadataValue;
                        return value.SerialNumber;
                    }) + 1;
            }

            string code = "";
            code = SystemTime.Now.ToString(this.Format);
            code = code.Replace(this.SerialNumberFormat, maxSerialNumber.ToString().PadLeft(this.SerialNumberLength, '0'));
            return new CodeMetadataValue(code, this);
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

        public override MetadataValue CreateMetadataValue(JToken value)
        {
            return new CodeMetadataValue(value.ToString(), this);
        }
    }
}

