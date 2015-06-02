using GBlog.Model;
using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.Interface
{
    public interface ISqlBuilder
    {
        void SetPrimaryKey(IDBInterface dbHelper, BaseModel model);
        void SetProperties(DataRow dr, BaseModel model);
        string InitInsertSQL(BaseModel model);
        string InitDeleteSQL(BaseModel model);
        string InitUpdateSQL(BaseModel model);
        string InitSelectSingleSQL(BaseModel model);
        string InitPapingSQL(BaseModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
        List<IDbDataParameter> InitParameters(BaseModel model);
        IDbDataParameter InitPrimaryKeyParameter(BaseModel model);
    }
}
