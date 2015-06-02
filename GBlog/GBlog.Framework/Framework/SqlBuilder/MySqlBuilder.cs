using GBlog.Framework.Base;
using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace GBlog.Framework.SqlBuilder
{
    public class MySqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        public MySqlBuilder()
            : base("@")
        {
        }

        public List<IDbDataParameter> InitParameters(BaseModel model)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            for (int i = 0; i < reflectionObject.Fields.Count; i++)
            {
                IDbDataParameter param = new MySqlParameter(_parameterSign + reflectionObject.Fields[i],
                                                        reflectionObject.Properties[i].GetValue(model));
                list.Add(param);
            }
            return list;
        }
    }
}
