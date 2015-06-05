using GBlog.Model;
using GBlog.Model.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Model.DataBaseTableModel
{
    [SignModel(TableName = "UserInfo", PrimaryKey = "UserID")]
    public class UserInfo : BaseModel
    {  
        /// <summary>
        /// userid
        /// </summary>		
        private int _userid;
        public int UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }
        /// <summary>
        /// username
        /// </summary>		
        private string _username;
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// pwd
        /// </summary>		
        private string _pwd;
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
        /// <summary>
        /// roleitems
        /// </summary>		
        private string _roleitems;
        public string RoleItems
        {
            get { return _roleitems; }
            set { _roleitems = value; }
        }
        /// <summary>
        /// utype
        /// </summary>		
        private int _utype;
        public int UType
        {
            get { return _utype; }
            set { _utype = value; }
        }
        /// <summary>
        /// email
        /// </summary>		
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        /// <summary>
        /// personname
        /// </summary>		
        private string _personname;
        public string PersonName
        {
            get { return _personname; }
            set { _personname = value; }
        }

    }
}
