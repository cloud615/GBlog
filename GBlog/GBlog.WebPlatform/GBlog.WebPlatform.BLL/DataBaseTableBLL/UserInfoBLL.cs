using GBlog.Framework.Base;
using GBlog.Framework.Interface;
using GBlog.Model.DataBaseTableModel;
using GBlog.WebPlatform.DAL.DataBaseTableDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.WebPlatform.BLL.DataBaseTableBLL
{
    public class UserInfoBLL : BaseBLL<UserInfo, UserInfoDAL>, IBLL<UserInfo>
    {
        public UserInfoBLL()
            : base(new UserInfoDAL())
        {

        }
    }
}
