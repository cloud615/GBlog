using GBlog.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.StaticObject
{
    public class FillingStore
    {
        /// <summary>
        /// Data转换为List
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal static List<T> FillingModel<T>(T model, DataTable dt) where T : BaseModel
        {
            if (dt == null && dt.Rows.Count == 0)
            {
                return null;
            }
            Type type = model.GetType();
            PropertyInfo[] properties = type.GetProperties();

            List<T> list = new List<T>();

            object entity = null;
            foreach (DataRow dr in dt.Rows)
            {
                entity = Activator.CreateInstance(type);
                foreach (DataColumn dc in dt.Columns)
                {
                    foreach (var property in properties)
                    {
                        if (dc.ColumnName.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (property.CanWrite)
                                property.SetValue(entity, dr[dc.ColumnName]);
                        }
                    }
                }
                list.Add(entity as T);
            }

            return list;
        }
    }
}
