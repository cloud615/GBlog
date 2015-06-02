using GBlog.Framework.Base;
using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace GBlog.Framework.SqlBuilder
{
    public class OdbcSqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        public OdbcSqlBuilder()
            : base(":")
        {
        }

        public List<IDbDataParameter> InitParameters(BaseModel model)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            for (int i = 0; i < reflectionObject.Fields.Count; i++)
            {
                IDbDataParameter param = new OdbcParameter(_parameterSign + reflectionObject.Fields[i],
                                                        reflectionObject.Properties[i].GetValue(model));
                list.Add(param);
            }
            return list;
        }
    }
}
