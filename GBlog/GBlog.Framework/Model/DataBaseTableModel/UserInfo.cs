using GBlog.Model;
using GBlog.Model.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Model.DataBaseTableModel
{
    [SignModel(TableName="UserInfo",PrimaryKey="ID")]
    public class UserInfo : BaseModel
    {       
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
