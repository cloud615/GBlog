using GBlog.Framework.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.DBHelper
{
    /// <summary>
    /// MySql操作类
    /// </summary>
    public class CMySqlDBWrap : IDBInterface
    {
        private int ResultRow;

        /// <summary>
        /// 执行update insert等所影响行数
        /// </summary>
        public int m_ResultRow
        {
            get
            {
                return ResultRow;
            }
            set
            {
                ResultRow = value;
            }
        }

        private string _strConnectString = "";
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string m_strConnectString
        {
            get
            {
                return _strConnectString;
            }
            set
            {
                _strConnectString = value;
            }
        }
        /// <summary>
        /// 关闭异常
        /// </summary>
        private string strCloseError;
        /// <summary>
        /// 是否开启事务
        /// </summary>
        private bool m_bTrans = false;
        /// <summary>
        /// 数据库链接对象
        /// </summary>
        private MySqlConnection m_objConn = null;
        /// <summary>
        /// 事务对象
        /// </summary>
        private MySqlTransaction m_objTrans = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CMySqlDBWrap()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strConnectString">连接字符串</param>
        public CMySqlDBWrap(string strConnectString)
        {
            // 系统参数替换
            m_strConnectString = strConnectString;
            m_bTrans = false;
            m_objConn = new MySqlConnection(m_strConnectString);
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~CMySqlDBWrap()
        {
            string strError;
            if (IsOpen())
                CloseDB(out strError);
        }

        #region 数据库状态处理

        /// <summary>
        ///  连接数据库，如果关闭则重新连接，如果已连接则不重新打开．
        /// </summary>
        /// <param name="strError">返回连接失败的原因</param>
        /// <returns>布尔值表示成功或失败</returns>
        public bool OpenDB(out string strError)
        {
            strError = "";
            bool bResult = false;
            try
            {
                if (string.IsNullOrEmpty(m_objConn.ConnectionString))
                {
                    m_objConn.ConnectionString = m_strConnectString;
                }
                if (m_objConn.State == ConnectionState.Closed)
                {
                    m_objConn.Open();
                }
                bResult = true;
            }
            catch (Exception e)
            {
                strError = e.Message;
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 关闭数据库,如果没有关闭则关闭．如果已经关闭不进行操作．
        /// </summary>
        /// <param name="strError">返回关闭连接失败的原因</param>
        /// <returns>布尔值表示成功或失败</returns>
        public bool CloseDB(out string strError)
        {
            strError = "";
            bool bResult = false;
            try
            {
                if (m_objConn.State != ConnectionState.Closed)
                {
                    m_objConn.Close();
                    m_objConn.Dispose();
                }
                bResult = true;
            }
            catch (Exception e)
            {
                strError = e.Message;
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 判断数据库连接是否已经成功打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            bool bResult = false;
            try
            {
                if (null == m_objConn)
                    bResult = false;
                else
                    bResult = !(m_objConn.State == ConnectionState.Closed);
            }
            catch (Exception e)
            {
                string s = e.Message;
                bResult = false;
            }
            return bResult;
        }

        #endregion

        #region 获取主键最大值

        /// <summary>
        /// 得到数据库中某个表中最大的ID, 用于DB2, Oracle等没有自增列的数据库
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strError">异常信息</param>
        /// <returns>最大ID值</returns>
        public int GetMaxID(string strTableName, out string strError)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return -1;
                }
            }

            int nResult = 0;
            string strSQL = "select max(ID) from " + strTableName;
            DataSet ds = null;
            DataTable dt = null;
            ds = GetDataSetBySQL(strSQL, true, out strError);
            dt = ds.Tables[0];
            if (null == dt)
                nResult = -1;
            else
            {
                if (dt.Rows[0][0] == System.DBNull.Value)
                {
                    nResult = 0;
                }
                else
                {
                    nResult = System.Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            return nResult;
        }
        /// <summary>
        /// 得到数据库中某个表中最大的ID, 用于DB2, Oracle等没有自增列的数据库
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="strIDColName">自增列名</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">异常信息</param>
        /// <returns>最大ID值</returns>
        public int GetMaxID(string strTableName, string strIDColName, bool bCloseDB, out string strError)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return -1;
                }
            }

            int nResult = 0;
            string strSQL = "select max(" + strIDColName + ") from " + strTableName;
            DataSet ds = null;
            DataTable dt = null;
            ds = GetDataSetBySQL(strSQL, bCloseDB, out strError);
            dt = ds.Tables[0];
            if (null == dt)
                nResult = -1;
            else
            {
                try
                {
                    if (dt.Rows[0][0] == System.DBNull.Value)
                    {
                        nResult = 0;
                    }
                    else
                    {
                        nResult = System.Convert.ToInt32(dt.Rows[0][0]);
                    }
                }
                catch
                {
                    nResult = -1;
                }
            }
            return nResult;
        }

        #endregion

        #region 事务状态处理
        /// <summary>
        /// 启动事务
        /// </summary>
        /// <param name="strError">启动事务失败的原因</param>
        /// <returns>成功或失败</returns>
        public bool BeginTrans(out string strError)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return false;
                }
            }

            bool bResult = false;
            try
            {
                m_objTrans = m_objConn.BeginTransaction();
                m_bTrans = true;
                bResult = true;
            }
            catch (Exception e)
            {
                strError = e.Message;
                bResult = false;
            }

            return bResult;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool CommitTrans(out string strError)
        {
            strError = "";
            if (!m_bTrans)
            {
                strError = "没有开始事务!";
                return false;
            }

            bool bResult = false;
            try
            {
                m_objTrans.Commit();
                bResult = true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                bResult = false;
            }

            return bResult;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public bool RollbackTrans(out string strError)
        {
            strError = "";
            if (!m_bTrans)
            {
                strError = "没有开始事务!";
                return false;
            }

            bool bResult = false;
            try
            {
                m_objTrans.Rollback();
                bResult = true;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                bResult = false;
            }

            return bResult;
        }

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
        public T GetScalarBySQL<T>(string strSQL, bool bCloseDB, out string strError)
        {
            T objDataSet = GetScalarBySQL<T>(strSQL, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }
        /// <summary>
        /// 根据一个select SQL返回一个Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        public T GetScalarBySQL<T>(string strSQL, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return default(T);
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return default(T);
                }
            }

            T obj = default(T);
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                obj = (T)objCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                strError = e.Message;
                obj = default(T);
            }
            return obj;
        }
        /// <summary>
        /// 根据一个不带参数的存储过程返回一个Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strProcedureName">存储过程名</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        public T GetScalarByProcedure<T>(string strProcedureName, out string strError)
        {
            return GetScalarByProcedure<T>(strProcedureName, out strError, null);
        }

        /// <summary>
        /// 根据带参数的存储过程得到Scalar
        /// </summary>
        /// <param name="strError"></param>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">参数</param>
        /// <returns>DataReader</returns>
        public T GetScalarByProcedure<T>(string strProcedureName, List<IDbDataParameter> listParam, out string strError)
        {
            return GetScalarByProcedure<T>(strProcedureName, out strError, listParam.ToArray());
        }

        /// <summary>
        /// 根据带参数的存储过程得到Scalar
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="strError">异常信息</param>
        /// <returns>返回第一行第一列的结果</returns>
        private T GetScalarByProcedure<T>(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return default(T);
                }
            }

            T obj = default(T);
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        objCommand.Parameters.Add(CheckParameter(parameter));
                    }
                }

                obj = (T)objCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                strError = e.Message;
                obj = default(T);
            }
            return obj;
        }

        #endregion

        #region 返回DataReader的函数

        /// <summary>
        /// 根据SQL得到datareader
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        public IDataReader GetDataReaderBySQL(string strSQL, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return null;
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }

            MySqlDataReader objDataReader = null;

            try
            {
                using (MySqlCommand objCommand = new MySqlCommand())
                {
                    objCommand.Connection = m_objConn;
                    objCommand.CommandText = strSQL;
                    objCommand.CommandType = CommandType.Text;
                    if (m_bTrans)
                    {
                        objCommand.Transaction = m_objTrans;
                    }

                    objDataReader = objCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return null;
            }

            return objDataReader;
        }
        /// <summary>
        /// 根据SQL得到datareader
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="bCloseDB">是否关闭数据库连接</param>
        /// <param name="strError">异常信息</param>
        /// <returns></returns>
        public IDataReader GetDataReaderBySQL(string strSQL, bool bCloseDB, out string strError)
        {
            IDataReader objDataReader = GetDataReaderBySQL(strSQL, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataReader;
        }
        /// <summary>
        /// 根据不参数的存储过程得到datareader
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        public IDataReader GetDataReaderByProcedure(string strProcedureName, out string strError)
        {

            return GetDataReaderByProcedure(strProcedureName, out strError, null);
        }
        /// <summary>
        /// 根据带参数的存储过程得到datareader
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">参数</param>
        /// <param name="strError">异常信息</param>
        /// <returns>IDataReader</returns>
        public IDataReader GetDataReaderByProcedure(string strProcedureName, List<IDbDataParameter> listParam, out string strError)
        {
            return GetDataReaderByProcedure(strProcedureName, out strError, listParam.ToArray());
        }
        /// <summary>
        /// 根据带参数的存储过程得到datareader
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError">异常信息</param>
        /// <param name="parameters">参数</param>
        /// <returns>IDataReader</returns>
        private IDataReader GetDataReaderByProcedure(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }
            
            MySqlDataReader objDataReader = null;
            try
            {
                using (MySqlCommand objCommand = new MySqlCommand())
                {
                    objCommand.Connection = m_objConn;
                    objCommand.CommandText = strProcedureName;
                    objCommand.CommandType = CommandType.StoredProcedure;
                    if (m_bTrans)
                    {
                        objCommand.Transaction = m_objTrans;
                    }
                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach (MySqlParameter parameter in parameters)
                        {
                            objCommand.Parameters.Add(CheckParameter(parameter));
                        }
                    }

                    objDataReader = objCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }
            return objDataReader;
        }

        #endregion

        #region 返回DataSet的函数

        /// <summary>
        /// 根据查询语句返回DataSet对象
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="bCloseDB">是否关闭数据库true-关闭</param>
        /// <param name="strError">错误信息</param>
        /// <returns>结果集</returns>
        public DataSet GetDataSetBySQL(string strSQL, bool bCloseDB, out string strError)
        {
            strError = "";
            DataSet objDataSet = GetDataSetBySQL(strSQL, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }
        /// <summary>
        /// 根据查询语句返回DataSet对象
        /// </summary>
        /// <param name="strSQL">Select语句</param>
        /// <param name="strError">查询失败的原因</param>
        /// <returns>返回DataSet,如果失败则返回null</returns>
        public DataSet GetDataSetBySQL(string strSQL, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return null;
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }

            DataSet objDataSet = null;
            MySqlCommand objCommand = null;
            MySqlDataAdapter objDataAdapter = null;
            try
            {
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                objDataAdapter = new MySqlDataAdapter(objCommand);
                objDataAdapter.InsertCommand = objCommand;
                objDataAdapter.SelectCommand = objCommand;
                objDataAdapter.UpdateCommand = objCommand;
                objDataAdapter.DeleteCommand = objCommand;

                objDataSet = new DataSet();
                objDataAdapter.Fill(objDataSet);

            }
            catch (Exception e)
            {
                strError = e.Message;
                return null;
            }
            finally
            {
                objCommand.Dispose();
                objDataAdapter.Dispose();
            }
            return objDataSet;
        }

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
        public DataSet GetDataSetBySQLPos(string strSQL, int startRecord, int maxRecords, string srcTable, bool bCloseDB, out string strError)
        {
            DataSet objDataSet = GetDataSetBySQLPos(strSQL, startRecord, maxRecords, srcTable, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }
        /// <summary>
        /// 得到DataSet有位置
        /// </summary>
        /// <param name="strSQL">sql脚本</param>
        /// <param name="startRecord">起始位置</param>
        /// <param name="maxRecords">结束位置</param>
        /// <param name="srcTable">表名</param>
        /// <param name="strError">错误信息</param>
        /// <returns>返回指定的DataSet</returns>
        public DataSet GetDataSetBySQLPos(string strSQL, int startRecord, int maxRecords, string srcTable, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return null;
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }

            DataSet objDataSet = null;
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;

                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                MySqlDataAdapter objDataAdapter = null;
                objDataAdapter = new MySqlDataAdapter(objCommand);
                objDataSet = new DataSet();
                objDataAdapter.Fill(objDataSet, startRecord, maxRecords, srcTable);
            }
            catch (Exception e)
            {
                strError = e.Message;
                return null;
            }
            return objDataSet;
        }
        /// <summary>
        /// 根据SQL得到存储过程
        /// </summary>
        /// <param name="strProcedureName">SQL或存储过程名称</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">错误信息</param>
        /// <returns></returns>
        public DataSet GetDataSetByProcedure(string strProcedureName, bool bCloseDB, out string strError)
        {
            DataSet objDataSet = null;

            //// oracle数据库通过sql得到数据集
            //if (this.m_strConnectString.ToLower().IndexOf("oraMySql") != -1 || this.m_strConnectString.ToLower().IndexOf("msdaora") != -1)
            //{
            //    string strConn = COracleDBWrap.TransConn(this.m_strConnectString);
            //    COracleDBWrap ora = new COracleDBWrap(strConn);
            //    objDataSet = ora.GetDataSetByProcedure(strProcedureName, bCloseDB, out strError);
            //}

            objDataSet = GetDataSetByProcedure(strProcedureName, out strError);

            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }

        /// <summary>
        /// 根据无参数存储过程得到dataset
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError">输出参数,返回错误信息</param>
        /// <returns></returns>
        public DataSet GetDataSetByProcedure(string strProcedureName, out string strError)
        {
            return GetDataSetByProcedure(strProcedureName, out strError, null);
        }

        /// <summary>
        /// 根据带参数的存储过程得到dataset
        /// </summary>
        /// <param name="strError"></param>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="listParam">参数</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSetByProcedure(string strProcedureName, List<IDbDataParameter> listParam, out string strError)
        {
            return GetDataSetByProcedure(strProcedureName, out strError, listParam.ToArray());
        }

        /// <summary>
        /// 根据带参数的存储过程得到dataset
        /// </summary>
        /// <param name="strError"></param>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataSet</returns>
        private DataSet GetDataSetByProcedure(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }

            DataSet objDataSet = null;
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;

                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        objCommand.Parameters.Add(CheckParameter(parameter));
                    }
                }
                MySqlDataAdapter objDataAdapter = null;
                objDataAdapter = new MySqlDataAdapter(objCommand);
                objDataSet = new DataSet();
                objDataAdapter.Fill(objDataSet);
            }
            catch (Exception e)
            {
                strError = e.Message;
                return null;
            }
            return objDataSet;
        }

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
        public DataTable GetDataTableBySql(string strSQL, string strTableName, bool bCloseDB, out string strError)
        {
            DataTable objDataSet = GetDataTableBySql(strSQL, strTableName, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }
        /// <summary>
        /// 通过具体的sql语句得到一个DataTable
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="strTableName">得到的Table名称，一般情况写表名</param>
        /// <param name="strError">输出错误信息</param>
        /// <returns></returns>
        public DataTable GetDataTableBySql(string strSQL, string strTableName, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return null;
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }
            DataTable objDataTable = null;
            try
            {
                MySqlDataAdapter odda = new MySqlDataAdapter(strSQL, m_strConnectString);
                objDataTable = new DataTable(strTableName);
                odda.Fill(objDataTable);
            }
            catch (Exception e)
            {
                strError = e.Message;
            }
            return objDataTable;
        }

        /// <summary>
        /// 通过具体的sql语句得到一个DataTable
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="strError">输出错误信息</param>
        /// <returns></returns>
        public DataTable GetDataTableBySql(string strSQL, out string strError)
        {
            string strTemp = strSQL;
            if (strTemp.ToLower().IndexOf("select") == -1)
            {
                strError = "不是一个select SQL语句";
                return null;
            }

            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return null;
                }
            }
            DataTable objDataTable = null;
            try
            {
                MySqlDataAdapter odda = new MySqlDataAdapter(strSQL, m_strConnectString);
                objDataTable = new DataTable();
                odda.Fill(objDataTable);
            }
            catch (Exception e)
            {
                strError = e.Message;
            }
            return objDataTable;
        }

        #endregion

        #region NonQuery函数
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">失败原因</param>
        /// <returns>成功或失败</returns>
        public bool ExecuteSQLNonQuery(string strSQL, bool bCloseDB, out string strError)
        {
            bool bResult = ExecuteSQLNonQuery(strSQL, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out this.strCloseError);
            }
            return bResult;
        }
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="strError">失败原因</param>
        /// <returns>成功或失败</returns>
        public bool ExecuteSQLNonQuery(string strSQL, out string strError)
        {
            return ExecuteSQLNonQuery(strSQL, out strError, null);
        }
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="strError">失败原因</param>
        /// <param name="listParam">参数</param>
        /// <returns>成功或失败</returns>
        public bool ExecuteSQLNonQuery(string strSQL, List<IDbDataParameter> listParam, out string strError)
        {
            return ExecuteSQLNonQuery(strSQL, out strError, listParam.ToArray());
        }
        /// <summary>
        /// 执行SQL语句，并返回是否成功
        /// </summary>
        /// <param name="strSQL">Insert,delete,update语句</param>
        /// <param name="strError">失败原因</param>
        /// <param name="parameters">失败原因</param>
        /// <returns>成功或失败</returns>
        private bool ExecuteSQLNonQuery(string strSQL, out string strError, params IDbDataParameter[] parameters)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return false;
                }
            }

            bool bResult = false;
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        objCommand.Parameters.Add(CheckParameter(parameter));
                    }
                }
                ResultRow = objCommand.ExecuteNonQuery();
                bResult = true;
            }
            catch (Exception e)
            {
                bResult = false;
                strError = e.Message;
            }
            return bResult;
        }

        /// <summary>
        /// 执行没有参数,不返回记录集的存储过程
        /// </summary>
        /// <param name="strProcedureName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool ExecuteProcedureNonQuery(string strProcedureName, out string strError)
        {
            return ExecuteProcedureNonQuery(strProcedureName, out strError, null);
        }
        /// <summary>
        /// 执行带参数不返回记录集的存储过程
        /// </summary>
        /// <param name="strError"></param>
        /// <param name="strProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private bool ExecuteProcedureNonQuery(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return false;
                }
            }

            bool bResult = false;
            try
            {
                MySqlCommand objCommand = null;
                objCommand = new MySqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        objCommand.Parameters.Add(CheckParameter(parameter));
                    }
                }
                ResultRow = objCommand.ExecuteNonQuery();
                bResult = true;
            }
            catch (Exception e)
            {
                strError = e.Message;
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 执行带参数不返回记录集的存储过程
        /// </summary>
        /// <param name="strError"></param>
        /// <param name="strProcedureName"></param>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public bool ExecuteProcedureNonQuery(string strProcedureName, List<IDbDataParameter> listParam, out string strError)
        {
            return ExecuteProcedureNonQuery(strProcedureName, out strError, listParam.ToArray());
        }

        #endregion

        #region 辅助函数
        /// <summary>
        /// Sets the specified SqlParameter's Value property to DBNull.Value if it is null.
        /// </summary>
        /// <param name="parameter">The SqlParameter that should be checked for nulls.</param>
        /// <returns>The SqlParameter with a potentially updated Value property.</returns>
        private MySqlParameter CheckParameter(MySqlParameter parameter)
        {
            if (parameter.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            return parameter;
        }

        #endregion

    }
}
