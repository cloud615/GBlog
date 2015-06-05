using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Interface
{
    public interface IBLL<TModel>
    {        
        string Insert(TModel model);
        string Delete(TModel model);
        string Update(TModel model);
        TModel Select(TModel model);
        DataTable GetDataTable(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
        List<TModel> GetList(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount);
    }
}
