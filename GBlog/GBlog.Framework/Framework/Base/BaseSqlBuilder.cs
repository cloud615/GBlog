using GBlog.Framework.Interface;
using GBlog.Framework.StaticObject;
using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
            int primaryKeyValue = dbHelper.GetScalarBySQL<int>(sql, out strError);

            reflectionObject.PrimaryKeyProperty.SetValue(model, primaryKeyValue);
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

            string sql = string.Format("delete table {0} where {1}=@{1}", reflectionObject.TableName, reflectionObject.PrimaryKey);

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

        public string InitSelectSingleSQL(BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = string.Format("select top 1 * from {0} where {1}=@{1}", reflectionObject.TableName, reflectionObject.PrimaryKey);

            return sql;
        }

        public void SetProperties(DataRow dr, BaseModel model)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            // 读取查询结果，生成新的实体对象
            Type type = model.GetType();
            BaseModel modelNew = Activator.CreateInstance(type) as BaseModel;


            for (int i = 0; i < reflectionObject.Properties.Count; i++)
            {
                var property = reflectionObject.Properties[i];
                property.SetValue(modelNew, dr[property.Name]);
            }

            foreach (var property in reflectionObject.Properties)
            {
                // 如果更新对象的 属性值 为空，则 进行赋值操作（将读取的对象的属性赋予model）
                if (property.GetValue(model) == null)
                {
                    property.SetValue(model, property.GetValue(modelNew));
                }
            }
        }

        public string InitPapingSQL(BaseModel model, int pageIndex, int pageSize, string whereStr, string orderByStr, ref int dataCount)
        {
            ReflectioinObject reflectionObject = ReflectionStore.ReadModelMessage(model.GetType());

            string sql = "select top {0} * from {1} where {2} not in (select top {4} {2} from {1} where {5} {6} ) {6}";

            //sql = String.Format("Select * From ({1}) b Where {2} Not In(Select Top {0} {2} From ({1}) a)", startRowIndex - 1, sql, keyColumn);
            //sql = String.Format("Select Top {0} * From ({1}) b Where {2} Not In(Select Top {3} {2} From ({1}) a)", startRowIndex + maximumRows - 1, sql, keyColumn, startRowIndex - 1);

            return "";
        }
    }
}
