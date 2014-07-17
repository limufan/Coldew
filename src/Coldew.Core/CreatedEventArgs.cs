using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core
{
    public class CreatedEventArgs<CreateInfoT, CreatedObjectT>: OperationEventArgs
    {
        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoT CreateInfo { set; get; }

        /// <summary>
        /// 创建以后的对象
        /// </summary>
        public CreatedObjectT CreatedObject { get; set; }
    }
}
