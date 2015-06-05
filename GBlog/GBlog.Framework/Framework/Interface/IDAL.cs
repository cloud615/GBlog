using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Interface
{
    public interface IDAL<T> where T:BaseModel
    {
        string Insert(T model);
        string Delete(T model);
        string Update(T model);
        T Select(T model);
        DataTable GetDataTable(T model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
        List<T> GetList(T model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
    }
}
