using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.Base
{
    public class BaseSqlBuilder
    {
        protected readonly string _parameterSign;
        public BaseSqlBuilder(string parameterSign)
        {
            if (string.IsNullOrWhiteSpace(parameterSign))
            {
                throw new ArgumentNullException();
            }
            _parameterSign = parameterSign;
        }

        public void SetPrimaryKey(IDBInterface dbHelper, BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = string.Format("select max({0})+1 from {1} ", reflectionObject.PrimaryKey, reflectionObject.TableName);
            string strError = string.Empty;
            model.PrimaryKeyValue = dbHelper.GetScalarBySQL<int>(sql, out strError);

            reflectionObject.PrimaryKeyProperty.SetValue(model, model.PrimaryKeyValue);
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

    }
}
