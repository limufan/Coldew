using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Coldew.Core.Organization;
using Coldew.Core.Permission;

namespace Coldew.Core
{
    public class ColdewManager
    {
        public ColdewManager()
        {
            log4net.Config.XmlConfigurator.Configure();
            this.Logger = log4net.LogManager.GetLogger("logger");

            this.Init();
            this.Load();
        }

        public virtual void Init()
        {
            this.OrgManager = new OrganizationManagement();
            this.ObjectManager = this.CreateObjectManager();
        }

        protected virtual void Load()
        {
            
        }

        protected virtual ColdewObjectManager CreateObjectManager()
        {
            return new ColdewObjectManager(this);
        }

        public OrganizationManagement OrgManager { set; get; }

        public ColdewObjectManager ObjectManager { set; get; }

        public MailSender MailSender { set; get; }

        ILog _logger;
        public ILog Logger
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _logger = value;
            }
            get
            {
                return _logger;
            }
        }
    }
}
