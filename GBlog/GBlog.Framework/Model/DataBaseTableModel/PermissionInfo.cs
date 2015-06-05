using GBlog.Model;
using GBlog.Model.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Model.DataBaseTableModel
{
    [SignModel(TableName = "PermissionInfo", PrimaryKey = "PermissionID")]
    public class PermissionInfo : BaseModel
    {
        /// <summary>
        /// permissionid
        /// </summary>		
        private int _permissionid;
        public int PermissionID
        {
            get { return _permissionid; }
            set { _permissionid = value; }
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
        /// parentid
        /// </summary>		
        private int _parentid;
        public int ParentID
        {
            get { return _parentid; }
            set { _parentid = value; }
        }
        /// <summary>
        /// ctrlid
        /// </summary>		
        private string _ctrlid;
        public string CtrlID
        {
            get { return _ctrlid; }
            set { _ctrlid = value; }
        }
        /// <summary>
        /// controllername
        /// </summary>		
        private string _controllername;
        public string ControllerName
        {
            get { return _controllername; }
            set { _controllername = value; }
        }
        /// <summary>
        /// actionname
        /// </summary>		
        private string _actionname;
        public string ActionName
        {
            get { return _actionname; }
            set { _actionname = value; }
        }
        /// <summary>
        /// pageurl
        /// </summary>		
        private string _pageurl;
        public string PageURL
        {
            get { return _pageurl; }
            set { _pageurl = value; }
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
        /// <summary>
        /// remark2
        /// </summary>		
        private string _remark2;
        public string Remark2
        {
            get { return _remark2; }
            set { _remark2 = value; }
        }        
    }
}
