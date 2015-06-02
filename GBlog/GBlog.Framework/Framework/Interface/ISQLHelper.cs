using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Interface
{
    interface ISQLHelper
    {
        string GetNewPrimaryKeyValue(string tableName,string primaryKey);
        string InitInsertSQL(BaseModel model);
        string InitDeleteSQL(BaseModel model);
        string InitUpdateSQL(BaseModel model);
        List<IDataParameter> InitParameters(BaseModel model, List<string> fields);
    }
}
