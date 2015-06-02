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
        DataSet ds = new DataSet();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDAL()
        {
            CDBWrap obj = new CDBWrap();
            objDBWrap = obj.m_DbObj;
        }

        public virtual bool Insert(BaseModel model)
        {
            throw new System.NotImplementedException();            
            
            // 获取并设置主键

            // ORM : 调用 驱动执行操作

            // ADO ：拼接SQL，生成参数集合，执行sql
        }

        public bool Delete(BaseModel model)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(BaseModel model)
        {
            throw new System.NotImplementedException();
        }

        public BaseModel Select(string primaryKey)
        {
            throw new System.NotImplementedException();
        }

        public DataTable GetDataTable(int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            throw new System.NotImplementedException();
        }

        public List<BaseModel> GetList(int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
