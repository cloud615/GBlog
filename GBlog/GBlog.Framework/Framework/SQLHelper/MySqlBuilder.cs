using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.SQLHelper
{
    public class MySqlBuilder : ISQLHelper
    {
        public string GetNewPrimaryKeyValue(string tableName, string primaryKey)
        {
            string sql = string.Format("select max({0}) from {1} ", primaryKey, tableName);
            return "";
        }
        public string InitInsertSQL(BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = "insert into {0}({1}) values({2})";

            var valuesFormatFields = (from n in reflectionObject.Fields
                                      select "@" + n).ToList<string>();
            string values = string.Join(",", valuesFormatFields);

            sql = string.Format(sql, reflectionObject.TableName, string.Join(",", reflectionObject.Fields), values);

            return sql;
        }

        public string InitDeleteSQL(BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = "delete table {0} where {1}=@{1}";

            sql = string.Format(sql, reflectionObject.TableName, reflectionObject.PrimaryKey);

            return sql;
        }

        public string InitUpdateSQL(BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = "update {0} set {1} where {2}=@{2}";

            StringBuilder sb = new StringBuilder();
            foreach (var field in reflectionObject.Fields)
            {
                sb.AppendFormat("{0}=@{0}, ", field);
            }
            var temp = sb.ToString().TrimEnd(',', ' ');


            sql = string.Format(sql, reflectionObject.TableName, temp, reflectionObject.PrimaryKey);

            return sql;
        }

        public List<IDataParameter> InitParameters(BaseModel model, List<string> fields)
        {
            List<IDataParameter> list = new List<IDataParameter>();

            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            for (int i = 0; i < reflectionObject.Fields.Count; i++)
            {
                IDataParameter param = new MySqlParameter("@" + reflectionObject.Fields[i], 
                                                        reflectionObject.Properties[i].GetValue(model));
                list.Add(param);
            }
            return list;
        }
    }
}
