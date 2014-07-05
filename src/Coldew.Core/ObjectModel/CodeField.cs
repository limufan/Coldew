using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Newtonsoft.Json.Linq;
using Coldew.Core.Organization;
using System.Text.RegularExpressions;
using Coldew.Api.Exceptions;

namespace Coldew.Core
{
    public class CodeField : Field
    {
        public CodeField()
        {

        }

        public override string TypeName
        {
            get { return "编码"; }
        }

        private string _format;
        public string Format
        {
            internal set
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

        public string GenerateCode(string lastCode)
        {
            string lastCodeYear = this.TrySubstring(lastCode, this.YearFormatPosition, this.YearLength);
            string lastCodeMonth = this.TrySubstring(lastCode, this.MonthStartPosition, 2);
            string lastCodeSerialNumber = this.TrySubstring(lastCode, this.SerialNumberStartPosition, this.SerialNumberLength);
            int serialNumber = 1;
            if (lastCodeYear == SystemTime.Now.ToString(this.YearFormat) && lastCodeMonth == SystemTime.Now.ToString(this.MonthFormat))
            {
                if(int.TryParse(lastCodeSerialNumber, out serialNumber))
                {
                    serialNumber++;
                }
            }

            string code = "";
            code = SystemTime.Now.ToString(this.Format);
            code = code.Replace(this.SerialNumberFormat, serialNumber.ToString().PadLeft(this.SerialNumberLength, '0'));
            return code;
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

        public void Modify(FieldModifyBaseInfo modifyInfo, string format)
        {
            FieldModifyArgs args = new FieldModifyArgs { Name = modifyInfo.Name, Required = modifyInfo.Required };

            this.OnModifying(args);

            FieldModel model = NHibernateHelper.CurrentSession.Get<FieldModel>(this.ID);
            model.name = modifyInfo.Name;
            model.required = modifyInfo.Required;
            model.Config = format;

            NHibernateHelper.CurrentSession.Update(model);
            NHibernateHelper.CurrentSession.Flush();

            this.Name = modifyInfo.Name;
            this.Required = modifyInfo.Required;
            this.Format = format;

            this.OnModifyed(args);
        }
    }
}

