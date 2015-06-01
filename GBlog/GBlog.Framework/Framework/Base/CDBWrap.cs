using GBlog.Framework.DBHelper;
using GBlog.Framework.Interface;

namespace GBlog.Framework.Base
{
    public class CDBWrap
    {
        /// <summary>
        /// 异常信息
        /// </summary>
        public string strError = "";
        /// <summary>
        /// 数据库操作接口对象
        /// </summary>
        public IDBInterface m_DbObj;
        /// <summary>
        /// 默认web.config中的数据连接字串
        /// </summary>
        public CDBWrap()
        {
        }

        /// <summary>
        /// 获取接口对象
        /// </summary>
        /// <param name="strtype"></param>
        /// <param name="strconn"></param>
        /// <returns></returns>
        public IDBInterface GetDbObj(string strtype, string strconn)
        {
            switch (strtype.ToLower())
            {
                case "system.data.sqlclient":
                    m_DbObj = new CSqlDBWrap(strconn);
                    break;
                case "mysql.data.mysqlclient":
                    m_DbObj = new CMySqlDBWrap(strconn);
                    break;
                case "system.data.odbc":
                    m_DbObj = new COdbcDBWrap(strconn);
                    break;
                case "system.data.oledb":
                    m_DbObj = new COleDBWrap(strconn);
                    break;
                case "system.data.oracleclient":
                    m_DbObj = new COracleDBWrap(strconn);
                    break;
                default:
                    m_DbObj = new CSqlDBWrap(strconn);
                    break;

            }
            return m_DbObj;
        }
    }
}
