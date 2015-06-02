using GBlog.Model;
using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.Interface
{
    public interface ISqlBuilder
    {
        void SetPrimaryKey(IDBInterface dbHelper, BaseModel model);
        string InitInsertSQL(BaseModel model);
        string InitDeleteSQL(BaseModel model);
        string InitUpdateSQL(BaseModel model);
        List<IDbDataParameter> InitParameters(BaseModel model);
    }
}
