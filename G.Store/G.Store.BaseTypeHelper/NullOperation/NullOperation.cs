using System;

namespace G.Store.BaseTypeHelper.NullOperation
{
    public class NullOperation
    {
        /// <summary>
        /// 校验数据字段Null值
        /// </summary>
        /// <param name="_datareaderobj"></param>
        /// <param name="_defaultvalue"></param>
        /// <returns></returns>
        public static string ConvertDBNull(object _datareaderobj, string _defaultvalue=default(string))
        {
            return (_datareaderobj != DBNull.Value) ? _datareaderobj.ToString() : _defaultvalue;
        }

        /// <summary>
        /// 校验数据字段Null值
        /// </summary>
        /// <param name="_datareaderobj"></param>
        /// <param name="_defaultvalue"></param>
        /// <returns></returns>
        public static int ConvertDBNull(object _datareaderobj, int _defaultvalue = default(int))
        {
            return (_datareaderobj != DBNull.Value) ? int.Parse(_datareaderobj.ToString()) : _defaultvalue;
        }

        /// <summary>
        /// 校验数据字段Null值
        /// </summary>
        /// <param name="_datareaderobj"></param>
        /// <param name="_defaultvalue"></param>
        /// <returns></returns>
        public static decimal ConvertDBNull(object _datareaderobj, decimal _defaultvalue = default(decimal))
        {
            return (_datareaderobj != DBNull.Value) ? decimal.Parse(_datareaderobj.ToString()) : _defaultvalue;
        }

        /// <summary>
        /// 校验数据字段Null值
        /// </summary>
        /// <param name="_datareaderobj"></param>
        /// <param name="_defaultvalue"></param>
        /// <returns></returns>
        public static DateTime ConvertDBNull(object _datareaderobj, DateTime _defaultvalue = default(DateTime))
        {
            return (_datareaderobj != DBNull.Value) ? DateTime.Parse(_datareaderobj.ToString()) : _defaultvalue;
        }

    }
}
