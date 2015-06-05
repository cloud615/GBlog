using GBlog.Framework.Interface;
using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Base
{
    public class BaseBLL<TModel, TDAL>
        where TModel : BaseModel
        where TDAL : IDAL<TModel>
    {
        protected TDAL objDAL = default(TDAL);

        public BaseBLL(TDAL argObjDAL)
        {
            objDAL = argObjDAL;
        }

        public virtual string Insert(TModel model)
        {
            return objDAL.Insert(model);
        }
        public virtual string Delete(TModel model)
        {
            return objDAL.Delete(model);
        }
        public virtual string Update(TModel model)
        {
            return objDAL.Update(model);
        }
        public virtual TModel Select(TModel model)
        {
            return objDAL.Select(model);
        }
        public virtual DataTable GetDataTable(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            return objDAL.GetDataTable(model, pageIndex, pageSize, whereStr, orderByStr, ref dataCount);
        }
        public virtual List<TModel> GetList(TModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            return objDAL.GetList(model, pageIndex, pageSize, whereStr, orderByStr, ref dataCount);
        }

    }
}
