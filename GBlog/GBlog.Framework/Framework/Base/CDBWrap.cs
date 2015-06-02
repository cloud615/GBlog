using GBlog.Framework.DBHelper;
using GBlog.Framework.Interface;
using GBlog.Framework.SqlBuilder;
using System.Configuration;

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
        /// 数据库SQL语句、参数生成对象
        /// </summary>
        public ISqlBuilder m_SqlBuilder { get; set; }
        /// <summary>
        /// 默认web.config中的数据连接字串
        /// </summary>
        public CDBWrap()
        {
            m_DbObj = GetDbObj(ConfigurationManager.AppSettings["dbType"], ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["dbConn"]].ConnectionString);
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
                    m_SqlBuilder = new MsSqlBuilder();
                    break;
                case "mysql.data.mysqlclient":
                    m_DbObj = new CMySqlDBWrap(strconn);
                    m_SqlBuilder = new MySqlBuilder();
                    break;
                case "system.data.odbc":
                    m_DbObj = new COdbcDBWrap(strconn);
                    m_SqlBuilder = new OdbcSqlBuilder();
                    break;
                case "system.data.oledb":
                    m_DbObj = new COleDBWrap(strconn);
                    m_SqlBuilder = new OleDBSqlBuilder();
                    break;
                case "system.data.oracleclient":
                    m_DbObj = new COracleDBWrap(strconn);
                    m_SqlBuilder = new OracleSqlBuilder();
                    break;
                default:
                    m_DbObj = new CSqlDBWrap(strconn);
                    m_SqlBuilder = new MsSqlBuilder();
                    break;
            }
            return m_DbObj;
        }

    }
}
