using GBlog.Framework.Interface;
using System.Data;
using GBlog.Model;
using System.Collections.Generic;
using GBlog.Framework.StaticObject;
using GBlog.Framework.Wrap;

namespace GBlog.Framework.Base
{
    public class BaseDAL<TModel> where TModel : BaseModel
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

        public virtual string Insert(TModel model)
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

        public string Delete(TModel model)
        {
            string sql = objSqlBuilder.InitDeleteSQL(model);
            List<IDbDataParameter> parameters = new List<IDbDataParameter>() { objSqlBuilder.InitPrimaryKeyParameter(model) };

            string strError = string.Empty;
            bool result = objDBWrap.ExecuteSQLNonQuery(sql, parameters, out strError);
            return result ? "true" : strError;
        }

        public string Update(TModel model)
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

        public TModel Select(TModel model)
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

        public DataTable GetDataTable(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            string[] sqlArray = objSqlBuilder.InitPapingSQL(model, pageIndex, pageSize, whereStr, orderByStr);
            string strError = string.Empty;
            dataCount = objDBWrap.GetScalarBySQL<int>(sqlArray[0], out strError);
            if (string.IsNullOrWhiteSpace(strError))
            {
                DataTable dt = objDBWrap.GetDataTableBySql(sqlArray[1], out strError);
                if (string.IsNullOrWhiteSpace(strError))
                {
                    return dt;
                }
            }
            return null;
        }

        public List<TModel> GetList(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            DataTable dt = GetDataTable(model, pageIndex, pageSize, whereStr, orderByStr, ref dataCount);

            return FillingStore.FillingModel(model, dt);
        }

    }
}
