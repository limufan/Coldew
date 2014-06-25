using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.Organization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coldew.Api;
using Coldew.Data;
using Coldew.Api.Exceptions;
using Coldew.Core.DataServices;
using Coldew.Core.Permission;

namespace Coldew.Core
{
    public class MetadataChangingEventArgs
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public User Operator { set; get; }

        /// <summary>
        /// 修改信息
        /// </summary>
        public JObject ChangeInfo { set; get; }

        /// <summary>
        /// 修改以前的快照信息
        /// </summary>
        public JObject ChangingSnapshotInfo { set; get; }

        /// <summary>
        /// 修改的对象
        /// </summary>
        public Metadata Metadata { set; get; }
    }
}
