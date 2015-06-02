using GBlog.Framework.Interface;
using System.Data;
using GBlog.Model;
using System.Collections.Generic;
using GBlog.Framework.StaticObject;

namespace GBlog.Framework.Base
{
    public class BaseDAL
    {
        protected IDBInterface objDBWrap = null;
        protected ISqlBuilder objSqlBuilder = null;
        DataSet ds = new DataSet();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDAL()
        {
            CDBWrap obj = new CDBWrap();
            objDBWrap = obj.m_DbObj;
            objSqlBuilder = obj.m_SqlBuilder;
        }

        public virtual string Insert(BaseModel model)
        {
            // A) ORM : 调用 驱动执行操作
            // B) ADO ：拼接SQL，生成参数集合，执行sql

            // 具体实现中，获取并设置主键
            objSqlBuilder.SetPrimaryKey(objDBWrap, model);

            // 获取sql、参数集合
            string sql = objSqlBuilder.InitInsertSQL(model);
            List<IDbDataParameter> parameters = objSqlBuilder.InitParameters(model);

            string strError = string.Empty;
            bool result = objDBWrap.ExecuteSQLNonQuery(sql, parameters, out strError);
            return result ? "true" : strError;
        }

        public string Delete(BaseModel model)
        {
            string sql = objSqlBuilder.InitDeleteSQL(model);
            List<IDbDataParameter> parameters = new List<IDbDataParameter>() { objSqlBuilder.InitPrimaryKeyParameter(model) };

            string strError = string.Empty;
            bool result = objDBWrap.ExecuteSQLNonQuery(sql, parameters, out strError);
            return result ? "true" : strError;
        }

        public string Update(BaseModel model)
        {
            // 检索并更新model对象
            model = Select(model);
            // 获取sql、参数集合
            string sql = objSqlBuilder.InitUpdateSQL(model);
            List<IDbDataParameter> parameters = objSqlBuilder.InitParameters(model);

            string strError = string.Empty;
            bool result = objDBWrap.ExecuteSQLNonQuery(sql, parameters, out strError);
            return result ? "true" : strError;
        }

        public BaseModel Select(BaseModel model)
        {
            // 具体实现中，根据主键读取数据
            string sql = objSqlBuilder.InitSelectSingleSQL(model);
            List<IDbDataParameter> parameters = new List<IDbDataParameter>() { objSqlBuilder.InitPrimaryKeyParameter(model) };

            string strError = string.Empty;
            DataTable dt = objDBWrap.GetDataTableBySql(sql, out strError, parameters.ToArray());
            if (dt != null && dt.Rows.Count > 0)
            {
                objSqlBuilder.SetProperties(dt.Rows[0], model);
                return model;
            }
            else
            {
                return null;
            }
        }

        public DataTable GetDataTable(BaseModel model,int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            throw new System.NotImplementedException();
        }

        public List<BaseModel> GetList(int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
