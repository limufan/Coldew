using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldew.Core.Organization
{
    [Serializable]
    public class DepartmentCreateInfo
    {
        /// <summary>
        /// 部门主管职位
        /// </summary>
        public virtual Position ManagerPosition { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        string _name;
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                _name = value;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
