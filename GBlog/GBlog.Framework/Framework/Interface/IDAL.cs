using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Interface
{
    public interface IDAL
    {
        string Insert(BaseModel model);
        string Delete(BaseModel model);
        string Update(BaseModel model);
        BaseModel Select(string primaryKey);
        DataTable GetDataTable(int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
        List<BaseModel> GetList(int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
    }
}
