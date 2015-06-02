using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.Interface
{
    public interface IDBInterface
    {

        /// <summary>
        /// 连接字符串
        /// </summary>
        string m_strConnectString { get; set; }

        /// <summary>
        /// 执行update insert等所影响行数； maxian
        /// </summary>
        int m_ResultRow { get; set; }

        #region 数据库状态处理

        /// <summary>
        ///  连接数据库，如果关闭则重新连接，如果已连接则不重新打开．
        /// </summary>
        /// <param name="strError">返回连接失败的原因</param>
        /// <returns>布尔值表示成功或失败</returns>
        bool OpenDB(out string strError);

        /// <summary>
        /// 关闭数据库,如果没有关闭则关闭．如果已经关闭不进行操作．
        /// </summary>
        /// <param name="strError">返回关闭连接失败的原因</param>
        /// <returns>布尔值表示成功或失败</returns>
        bool CloseDB(out string strError);

        /// <summary>
        /// 判断数据库连接是否已经成功打开
        /// </summary>
        /// <returns></returns>
        bool IsOpen();

        #endregion

        #region 获取主键最大值

        /// <summary>
        /// 得到数据库中某个表中最大的ID, 用于DB2, Oracle等没有自增列的数据库
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strError">异常信息</param>
        /// <returns>最大ID值</returns>
        int GetMaxID(string strTableName, out string strError);
        /// <summary>
        /// 得到数据库中某个表中最大的ID, 用于DB2, Oracle等没有自增列的数据库
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strIDColName">自增列名</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">异常信息</param>
        /// <returns>最大ID值</returns>
        int GetMaxID(string strTableName, string strIDColName, bool bCloseDB, out string strError);

        #endregion

        #region 事务状态处理
        /// <summary>
        /// 启动事务
        /// </summary>
        /// <param name="strError">启动事务失败的原因</param>
        /// <returns>成功或失败</returns>
        bool BeginTrans(out string strError);

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="strError">提交事务失败的原因</param>
        /// <returns>成功或失败</returns>
        bool CommitTrans(out string strError);

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="strError">回滚事务失败的原因</param>
        /// <returns>成功或失败</returns>
        bool RollbackTrans(out string strError);

        #endregion

        #region 返回Scalar的函数
        /// <summary>
        /// 根据一个select SQL返回一个Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        T GetScalarBySQL<T>(string strSQL, bool bCloseDB, out string strError);
        /// <summary>
        /// 根据一个select SQL返回一个Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        T GetScalarBySQL<T>(string strSQL, out string strError);
        /// <summary>
        /// 根据一个不带参数的存储过程返回一个Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strProcedureName">存储过程名</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        T GetScalarByProcedure<T>(string strProcedureName, out string strError);
        /// <summary>
        /// 根据带参数的存储过程得到Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">参数集合</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        T GetScalarByProcedure<T>(string strProcedureName, List<IDbDataParameter> listParam, out string strError);

        #endregion

        #region 返回DataReader的函数
        /// <summary>
        /// 根据SQL得到datareader
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        IDataReader GetDataReaderBySQL(string strSQL, out string strError);
        /// <summary>
        /// 根据SQL得到datareader
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="bCloseDB">是否关闭数据库连接</param>
        /// <param name="strError">异常信息</param>
        /// <returns></returns>
        IDataReader GetDataReaderBySQL(string strSQL, bool bCloseDB, out string strError);
        /// <summary>
        /// 根据不参数的存储过程得到datareader
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        IDataReader GetDataReaderByProcedure(string strProcedureName, out string strError);
        /// <summary>
        /// 根据带参数的存储过程得到datareader
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">参数</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        IDataReader GetDataReaderByProcedure(string strProcedureName, List<IDbDataParameter> listParam, out string strError);

        #endregion

        #region 返回DataSet的函数

        /// <summary>
        /// 根据查询语句返回DataSet对象
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="bCloseDB">是否关闭数据库true-关闭</param>
        /// <param name="strError">错误信息</param>
        /// <returns>结果集</returns>
        DataSet GetDataSetBySQL(string strSQL, bool bCloseDB, out string strError);
        /// <summary>
        /// 根据查询语句返回DataSet对象
        /// </summary>
        /// <param name="strSQL">Select语句</param>
        /// <param name="strError">查询失败的原因</param>
        /// <returns>返回DataSet,如果失败则返回null</returns>
        DataSet GetDataSetBySQL(string strSQL, out string strError);

        /// <summary>
        /// 得到DataSet有位置
        /// </summary>
        /// <param name="strSQL">sql脚本</param>
        /// <param name="startRecord">起始位置</param>
        /// <param name="maxRecords">结束位置</param>
        /// <param name="srcTable">表名</param>
        /// <param name="bCloseDB">是否关闭数据</param>
        /// <param name="strError">错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        DataSet GetDataSetBySQLPos(string strSQL, int startRecord, int maxRecords, string srcTable, bool bCloseDB, out string strError);
        /// <summary>
        /// 得到DataSet有位置
        /// </summary>
        /// <param name="strSQL">sql脚本</param>
        /// <param name="startRecord">起始位置</param>
        /// <param name="maxRecords">结束位置</param>
        /// <param name="srcTable">表名</param>
        /// <param name="strError">错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        DataSet GetDataSetBySQLPos(string strSQL, int startRecord, int maxRecords, string srcTable, out string strError);
        /// <summary>
        /// 根据存储过程得到dataset
        /// </summary>
        /// <param name="strProcedureName">存储过程名称</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        DataSet GetDataSetByProcedure(string strProcedureName, bool bCloseDB, out string strError);

        /// <summary>
        /// 根据无参数存储过程得到dataset
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError">输出参数,返回错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        DataSet GetDataSetByProcedure(string strProcedureName, out string strError);

        /// <summary>
        /// 根据带参数的存储过程得到dataset
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">查询参数</param>
        /// <param name="strError">输出参数,返回错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        DataSet GetDataSetByProcedure(string strProcedureName, List<IDbDataParameter> listParam, out string strError);

        #endregion

        #region 返回DataTable的函数
        /// <summary>
        /// 通过具体的sql语句得到一个DataTable
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="strTableName">得到的Table名称，一般情况写表名</param>
        /// <param name="bCloseDB">是否关闭数据库,true=关闭</param>
        /// <param name="strError">输出错误信息</param>
        /// <returns>返回指定的DataTable</returns>
        DataTable GetDataTableBySql(string strSQL, string strTableName, bool bCloseDB, out string strError, params IDbDataParameter[] parameters);
        /// <summary>
        /// 通过具体的sql语句得到一个DataTable
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="strTableName">得到的Table名称，一般情况写表名</param>
        /// <param name="strError">输出错误信息</param>
        /// <returns>返回指定的DataTable</returns>
        DataTable GetDataTableBySql(string strSQL, string strTableName, out string strError, params IDbDataParameter[] parameters);

        /// <summary>
        /// 通过具体的sql语句得到一个DataTable
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="strError">输出错误信息</param>
        /// <returns>返回指定的DataTable</returns>
        DataTable GetDataTableBySql(string strSQL, out string strError, params IDbDataParameter[] parameters);

        #endregion

        #region NonQuery函数
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">失败原因</param>
        /// <returns>成功或失败</returns>
        bool ExecuteSQLNonQuery(string strSQL, bool bCloseDB, out string strError);
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="strError">失败原因</param>
        /// <returns>成功或失败</returns>
        bool ExecuteSQLNonQuery(string strSQL, out string strError);

        /// <summary>
        /// 执行带参数SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="strError">失败原因</param>
        /// <param name="listParam">参数</param>
        /// <returns>成功或失败</returns>
        bool ExecuteSQLNonQuery(string strSQL, List<IDbDataParameter> listParam, out string strError);

        /// <summary>
        /// 执行没有参数,不返回记录集的存储过程
        /// </summary>
        /// <param name="strProcedureName">存储过程名</param>
        /// <param name="strError">失败原因</param>
        /// <returns>成功或失败</returns>
        bool ExecuteProcedureNonQuery(string strProcedureName, out string strError);


        /// <summary>
        /// 执行带参数不返回记录集的存储过程
        /// </summary>
        /// <param name="strProcedureName">存储过程名</param>
        /// <param name="strError">失败原因</param>
        /// <param name="listParam">参数</param>
        /// <returns>成功或失败</returns>
        bool ExecuteProcedureNonQuery(string strProcedureName, List<IDbDataParameter> listParam, out string strError);


        #endregion
    }
}
