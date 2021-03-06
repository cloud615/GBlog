﻿using GBlog.Framework.Base;
using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;

namespace GBlog.Framework.SqlBuilder
{
    public class OracleSqlBuilder : BaseSqlBuilder, ISqlBuilder
    {
        public OracleSqlBuilder()
            : base(":")
        {
        }

        public List<IDbDataParameter> InitParameters(BaseModel model)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();

            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            for (int i = 0; i < reflectionObject.Fields.Count; i++)
            {
                IDbDataParameter param = new OracleParameter(_parameterSign + reflectionObject.Fields[i],
                                                        reflectionObject.Properties[i].GetValue(model));
                list.Add(param);
            }
            return list;
        }
        public IDbDataParameter InitPrimaryKeyParameter(BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());
            IDbDataParameter param = new OracleParameter(_parameterSign + reflectionObject.PrimaryKey,
                                                        reflectionObject.PrimaryKeyProperty.GetValue(model));

            return param;
        }
    }
}
