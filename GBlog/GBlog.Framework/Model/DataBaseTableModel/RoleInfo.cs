using GBlog.Model;
using GBlog.Model.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Model.DataBaseTableModel
{
    [SignModel(TableName = "RoleInfo", PrimaryKey = "RoleID")]
    public class RoleInfo : BaseModel
    {
        /// <summary>
        /// roleid
        /// </summary>		
        private int _roleid;
        public int RoleID
        {
            get { return _roleid; }
            set { _roleid = value; }
        }
        /// <summary>
        /// name
        /// </summary>		
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// permissionitems
        /// </summary>		
        private string _permissionitems;
        public string PermissionItems
        {
            get { return _permissionitems; }
            set { _permissionitems = value; }
        }
        /// <summary>
        /// remark
        /// </summary>		
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }        
    }
}
