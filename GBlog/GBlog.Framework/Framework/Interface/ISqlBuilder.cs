using GBlog.Model;
using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.Interface
{
    public interface ISqlBuilder
    {
        /// <summary>
        /// 从数据库获取主键新值，并为model的主键赋值
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="model"></param>
        void SetPrimaryKey(IDBInterface dbHelper, BaseModel model);
        /// <summary>
        /// 将DataRow中的值填充进 model ,
        /// 用于update操作前的 表单非必填字段的填充,
        /// 会根据参数model的主键，去数据库检索旧的数据进行填充
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="model"></param>
        void SetProperties(DataRow dr, BaseModel model);
        /// <summary>
        /// 生成insert语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string InitInsertSQL(BaseModel model);
        /// <summary>
        /// 生成delete语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string InitDeleteSQL(BaseModel model);
        /// <summary>
        /// 生成update语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string InitUpdateSQL(BaseModel model);
        /// <summary>
        /// 生成单挑数据的select语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string InitSelectSingleSQL(BaseModel model);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="model">实体对象，仅new即可</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页容量</param>
        /// <param name="whereStr">查询条件，开头不带and</param>
        /// <param name="orderByStr">排序条件，以order by开头</param>
        /// <returns>长度为2的数组，包含 统计数据量的sql和分页sql</returns>
        string[] InitPapingSQL(BaseModel model, int pageIndex, int pageSize, string whereStr, string orderByStr);
        /// <summary>
        /// 生成参数集合
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<IDbDataParameter> InitParameters(BaseModel model);
        /// <summary>
        /// 只生成主键对应参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IDbDataParameter InitPrimaryKeyParameter(BaseModel model);
    }
}
