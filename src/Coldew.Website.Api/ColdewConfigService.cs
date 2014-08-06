using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Coldew.Api;
using Coldew.Core;
using Coldew.Data;

namespace Coldew.Website.Api
{
    public class ColdewConfigService : IColdewConfigService
    {
        ColdewManager _crmManager;

        public ColdewConfigManager ConfigManager { set; get; }

        public ColdewConfigService(ColdewManager crmManager)
        {
            this.ConfigManager = new ColdewConfigManager(crmManager);
            this.ConfigManager.Load();
            _crmManager = crmManager;
        }

        public EmailConfigInfo GetEmailConfig()
        {
            return this.ConfigManager.GetEmailConfig();
        }

        public void SetEmailConfig(string account, string address, string password, string server)
        {
            this.ConfigManager.SetEmailConfig(account, address, password, server);
        }

        public void TestEmailConfig(string account, string address, string password, string server)
        {
            SmtpMailSender sender = new SmtpMailSender(new MailAddress(address), server, account, password);
            List<string> to = new List<string>();
            to.Add(address);
            sender.Send(to, null, "test", "test", false, null);
        }
    }
}
