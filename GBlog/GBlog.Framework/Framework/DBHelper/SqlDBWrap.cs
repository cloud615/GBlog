using GBlog.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GBlog.Framework.DBHelper
{
    /// <summary>
    /// CSqlDBWrap 使用ADO.NET中SQL Server的一套对象操作数据库
    /// </summary>
    public class CSqlDBWrap : IDBInterface
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

        private string strCloseError;
        private SqlConnection m_objConn = null;
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
        private bool m_bTrans = false;
        private SqlTransaction m_objTrans = null;
        /// <summary>
        /// 析构
        /// </summary>
        public CSqlDBWrap()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strConnectString">连接字符串</param>
        public CSqlDBWrap(string strConnectString)
        {
            m_strConnectString = strConnectString;
            m_bTrans = false;
            m_objConn = new SqlConnection(m_strConnectString);
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
                if (m_objConn.ConnectionString == "")
                {
                    m_objConn.ConnectionString = m_strConnectString;
                }
                if (m_objConn.State == ConnectionState.Closed)
                    m_objConn.Open();
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
        /// oledb字符串转Sql连接字符串
        /// </summary>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static string TransConn(string strConn)
        {
            string strRet = "";
            string[] list = strConn.Split(';');
            string strDs = "";
            string Uid = "";
            string Pws = "";
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].ToLower().IndexOf("data source") != -1)
                {
                    strDs = list[i].Split('=')[1].Trim();
                }
                else if (list[i].ToLower().IndexOf("user id") != -1)
                {
                    Uid = list[i].Split('=')[1].Trim();
                }
                else if (list[i].ToLower().IndexOf("password") != -1)
                {
                    Pws = list[i].Split('=')[1].Trim();
                }
            }
            strRet = "Data Source={0};User Id={1};Password={2};";
            strRet = string.Format(strRet, strDs, Uid, Pws);
            return strRet;
        }

        /// <summary>
        /// 得到数据库中某个表中最大的ID, 用于DB2, Sql等没有自增列的数据库
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
        /// 得到数据库中某个表中最大的ID, 用于DB2, Sql等没有自增列的数据库
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

        #region 事务函数
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
        /// <param name="strSQL"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
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
                return default(T);
            }
            return obj;
        }
        /// <summary>
        /// 根据一个不带参数的存储过程返回一个Scalar
        /// </summary>
        /// <param name="strProcedureName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
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
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="strError"></param>
        /// <param name="parameters">参数</param>
        /// <returns>DataReader</returns>
        private T GetScalarByProcedure<T>(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            string strTemp = strProcedureName;
            if (strTemp.ToLower().IndexOf("select") != -1
                || strTemp.ToLower().IndexOf("insert") != -1
                || strTemp.ToLower().IndexOf("update") != -1)
            {
                strError = "不是一个存储过程名";
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                if (parameters != null && parameters.Length > 0)
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        objCommand.Parameters.Add(CheckParameter(parameter));
                    }
                }

                obj = (T)objCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                strError = ex.Message;
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

            SqlDataReader objDataReader = null;

            try
            {
                using (SqlCommand objCommand = new SqlCommand())
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

            SqlDataReader objDataReader = null;
            try
            {
                using (SqlCommand objCommand = new SqlCommand())
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
                        foreach (SqlParameter parameter in parameters)
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
            SqlCommand objCommand = null;
            SqlDataAdapter objDataAdapter = null;
            try
            {
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                objDataAdapter = new SqlDataAdapter(objCommand);
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                SqlDataAdapter objDataAdapter = null;
                objDataAdapter = new SqlDataAdapter(objCommand);
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
        /// 根据存储过程得到dataset
        /// </summary>
        /// <param name="strSql">SQL或存储过程名称</param>
        /// <param name="bCloseDB">是否关闭数据库</param>
        /// <param name="strError">错误信息</param>
        /// <returns></returns>
        public DataSet GetDataSetByProcedure(string strSql, bool bCloseDB, out string strError)
        {
            DataSet objDataSet = GetDataSetByProcedure(strSql, out strError);
            if (bCloseDB)
            {
                this.CloseDB(out strCloseError);
            }
            return objDataSet;
        }
        /// <summary>
        /// 根据无参数存储过程得到dataset
        /// </summary>
        /// <param name="strSql">SQL或存储过程名字</param>
        /// <param name="strError">输出参数,返回错误信息</param>
        /// <returns></returns>
        public DataSet GetDataSetByProcedure(string strSql, out string strError)
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();

                #region
                strSql = strSql.Replace("(", " ");
                strSql = strSql.Replace(")", "");
                string[] sqllist = strSql.Split(' ');
                string strProcedureName = sqllist[0];

                string strParam = strSql.TrimStart(strProcedureName.ToCharArray());
                string[] strParamlist = strParam.Split(',');
                for (int i = 0; i < strParamlist.Length; i++)
                {
                    string name = "p" + i;
                    //SqlParameter p1 = new SqlParameter(name,SqlDbType.VarChar);
                    SqlParameter p1 = new SqlParameter();
                    p1.Direction = System.Data.ParameterDirection.Input;
                    p1.Value = strParamlist[i].Trim().Trim('\'');
                    objCommand.Parameters.Add(p1);
                }
                #endregion
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSql;

                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                SqlDataAdapter objDataAdapter = null;
                objDataAdapter = new SqlDataAdapter(objCommand);
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
            string strTemp = strProcedureName;
            if (strTemp.ToLower().IndexOf("select") != -1
                || strTemp.ToLower().IndexOf("insert") != -1
                || strTemp.ToLower().IndexOf("update") != -1)
            {
                strError = "不是一个存储过程名";
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                foreach (SqlParameter parameter in parameters)
                {
                    objCommand.Parameters.Add(CheckParameter(parameter));
                }

                SqlDataAdapter objDataAdapter = null;
                objDataAdapter = new SqlDataAdapter(objCommand);
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
                SqlDataAdapter odda = new SqlDataAdapter(strSQL, m_strConnectString);
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
                SqlDataAdapter odda = new SqlDataAdapter(strSQL, m_strConnectString);
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
            strError = "";
            if (!this.IsOpen())
            {
                if (!OpenDB(out strError))
                {
                    return false;
                }
            }

            bool bResult = false;
            SqlCommand objCommand = null;
            try
            {
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                ResultRow = objCommand.ExecuteNonQuery();
                bResult = true;

            }
            catch (Exception e)
            {
                bResult = false;
                strError = e.Message;
            }
            finally
            {
                objCommand.Dispose();
            }
            return bResult;
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
        /// <param name="parameters">参数</param>
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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strSQL;
                objCommand.CommandType = CommandType.Text;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (SqlParameter parameter in parameters)
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
        /// <param name="strProcedureName"></param>
        /// <param name="listParam"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public bool ExecuteProcedureNonQuery(string strProcedureName, List<IDbDataParameter> listParam, out string strError)
        {
            return ExecuteProcedureNonQuery(strProcedureName, out strError, listParam.ToArray());
        }

        /// <summary>
        /// 执行带参数不返回记录集的存储过程
        /// </summary>
        /// <param name="strProcedureName"></param>
        /// <param name="strError"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private bool ExecuteProcedureNonQuery(string strProcedureName, out string strError, params IDbDataParameter[] parameters)
        {
            string strTemp = strProcedureName;
            if (strTemp.ToLower().IndexOf("select") != -1
                || strTemp.ToLower().IndexOf("insert") != -1
                || strTemp.ToLower().IndexOf("update") != -1)
            {
                strError = "不是一个存储过程名";
                return false;
            }

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
                SqlCommand objCommand = null;
                objCommand = new SqlCommand();
                objCommand.Connection = m_objConn;
                objCommand.CommandText = strProcedureName;
                objCommand.CommandType = CommandType.StoredProcedure;
                if (m_bTrans)
                {
                    objCommand.Transaction = m_objTrans;
                }

                if (parameters != null && parameters.Length > 0)
                {
                    foreach (SqlParameter parameter in parameters)
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

        #endregion

        #region 辅助函数
        /// <summary>
        /// Sets the specified SqlParameter's Value property to DBNull.Value if it is null.
        /// </summary>
        /// <param name="parameter">The SqlParameter that should be checked for nulls.</param>
        /// <returns>The SqlParameter with a potentially updated Value property.</returns>
        private SqlParameter CheckParameter(SqlParameter parameter)
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
