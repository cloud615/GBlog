using GBlog.Framework.Interface;
using System.Data;

namespace GBlog.Framework.Base
{
    public class BaseDAL
    {
        protected IDBInterface objDBWrap = null;
        DataSet ds = new DataSet();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDAL()
        {
            CDBWrap obj = new CDBWrap();
            objDBWrap = obj.m_DbObj;
        }
    }
}
