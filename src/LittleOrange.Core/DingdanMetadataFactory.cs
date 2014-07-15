using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;

namespace LittleOrange.Core
{
    public class DingdanMetadataFactory : MetadataFactory
    {
        public DingdanMetadataFactory(MetadataManager metadataManager)
            :base(metadataManager)
        {
        }

        public override Metadata Create(Coldew.Data.MetadataModel model)
        {
            return base.Create(model);
        }

        public override Metadata Create(MetadataCreateInfo createInfo)
        {
            return base.Create(createInfo);
        }
    }
}
