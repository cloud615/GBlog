using GBlog.Model.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Framework.StaticObject
{
    public class ReflectionStore
    {
        /// <summary>
        /// 记录已通过反射获取的类的属性对象
        /// key：class的FullName
        /// value：属性集合
        /// </summary>
        private static Dictionary<string, ReflectioinObject> ReflectObject = new Dictionary<string, ReflectioinObject>();
        /// <summary>
        /// 读取对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ReflectioinObject ReadModelMessage(Type type)
        {
            string className = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
            if (!ReflectObject.Keys.Contains<string>(className))
            {
                InitReflectionObject(type, className);
            }

            return ReflectObject[className];
        }

        private static void InitReflectionObject(Type type, string className)
        {
            // 属性对象
            PropertyInfo[] properties = type.GetProperties();

            // 主键名
            string primaryKey = string.Empty;
            string tableName = string.Empty;
            SignModelAttribute signModelAttr = type.GetCustomAttribute<SignModelAttribute>();
            if (signModelAttr != null)
            {
                primaryKey = signModelAttr.PrimaryKey;
                tableName = signModelAttr.TableName;
            }

            // 属性名
            List<string> fields = new List<string>();
            foreach (var property in properties)
            {
                fields.Add(property.Name);
            }
            ReflectioinObject reflectObj = new ReflectioinObject();
            reflectObj.TableName = tableName;
            reflectObj.PrimaryKey = primaryKey;
            reflectObj.Fields = fields;
            reflectObj.Properties = properties.ToList();

            ReflectObject.Add(className, reflectObj);
        }

        #region 对象信息存储

        ///// <summary>
        ///// 记录已通过反射获取的类的属性对象
        ///// key：class的FullName
        ///// value：属性集合
        ///// </summary>
        //private static Dictionary<string, List<PropertyInfo>> ModelProperties = new Dictionary<string, List<PropertyInfo>>();
        ///// <summary>
        ///// 记录已通过反射获取的类的属性名
        ///// Key：class的FullName
        ///// Value：字段名称集合
        ///// </summary>
        //private static Dictionary<string, List<string>> ModelFields = new Dictionary<string, List<string>>();
        ///// <summary>
        ///// 记录已通过反射获取的类的主键名
        ///// Key：class的FullName
        ///// Value：主键名称
        ///// </summary>
        //private static Dictionary<string, string> ModelPrimaryKey = new Dictionary<string, string>();
        ///// <summary>
        ///// 记录已通过反射获取的类的表名
        ///// Key：class的FullName
        ///// Value：表名
        ///// </summary>
        //private static Dictionary<string, string> ModelTableName = new Dictionary<string, string>();

        //public static List<string> GetFields(Type type)
        //{
        //    string className = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //    if (!ModelFields.Keys.Contains<string>(className))
        //    {
        //        InitReflectionObject(type, className);
        //    }

        //    return ModelFields[className];
        //}
        //public static List<PropertyInfo> GetProperty(Type type)
        //{
        //    string className = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //    if (!ModelProperties.Keys.Contains<string>(className))
        //    {
        //        InitReflectionObject(type, className);
        //    }

        //    return ModelProperties[className];
        //}
        //public static string GetPrimaryKey(Type type)
        //{
        //    string className = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //    if (!ModelPrimaryKey.Keys.Contains<string>(className))
        //    {
        //        InitReflectionObject(type, className);
        //    }

        //    return ModelPrimaryKey[className];
        //}
        //public static string GetTableName(Type type)
        //{
        //    string className = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //    if (!ModelPrimaryKey.Keys.Contains<string>(className))
        //    {
        //        InitReflectionObject(type, className);
        //    }

        //    return ModelTableName[className];
        //}


        //private static void InitReflectionObject(Type type, string className)
        //{
        //    // 属性对象
        //    PropertyInfo[] properties = type.GetProperties();

        //    // 主键名
        //    string primaryKey = string.Empty;
        //    string tableName = string.Empty;
        //    SignModelAttribute signModelAttr = type.GetCustomAttribute<SignModelAttribute>();
        //    if (signModelAttr != null)
        //    {
        //        primaryKey = signModelAttr.PrimaryKey;
        //        tableName = signModelAttr.TableName;
        //    }

        //    // 属性名
        //    List<string> fields = new List<string>();
        //    foreach (var property in properties)
        //    {
        //        fields.Add(property.Name);
        //    }

        //    ModelProperties.Add(className, properties.ToList());
        //    ModelFields.Add(className, fields);
        //    ModelPrimaryKey.Add(className, primaryKey);
        //    ModelTableName.Add(className, tableName);
        //}

        #endregion
    }
    /// <summary>
    /// 反射得到的对象
    /// </summary>
    public class ReflectioinObject
    {
        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public PropertyInfo PrimaryKeyProperty { get; set; }
        public List<string> Fields { get; set; }
        public List<PropertyInfo> Properties { get; set; }
    }

}
