using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace G.Store.BaseTypeHelper
{
    public class StringOperation
    {
        /// <summary>
        /// 过滤空格、HTML换行等字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanString(string _text)
        {
            if (_text == null)
            {
                return string.Empty;
            }
            _text = _text.Trim();
            if (string.IsNullOrEmpty(_text))
            {
                return string.Empty;
            }

            _text = Regex.Replace(_text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            _text = Regex.Replace(_text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;

            _text = Regex.Replace(_text, "[']{1,}", "\"");
            _text = Regex.Replace(_text, "[’]{1,}", "\"");
            //_text = Regex.Replace(_text, "[-]{2,}", "——");

            _text = Regex.Replace(_text, "[<]{1,}", "&lt;");
            _text = Regex.Replace(_text, "[>]{1,}", "&gt;");
            _text = Regex.Replace(_text, "[~]{1,}", "");
            //_text = Regex.Replace(_text, "[*]{1,}", "");
            //_text = Regex.Replace(_text, "[%]{1,}", "");
            //_text = Regex.Replace(_text, "[#]{1,}", "");
            _text = Regex.Replace(_text, "[!]{1,}", "！");
            //_text = Regex.Replace(_text, "[|]{1,}", "");

            return _text;
        }

        /// <summary>
        /// 过滤自定义对象中所有String类型和DateTime类型的属性
        /// String类型的属性会过滤空格\HTML换行字符
        /// DateTime类型的属性为空会转换为1900-1-1
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static object CleanObjectProperty(object _object)
        {
            foreach (PropertyInfo theproperty in _object.GetType().GetProperties())
            {
                if (theproperty.PropertyType.Name == "String")
                {
                    theproperty.SetValue(_object, CleanString((string)(theproperty.GetValue(_object, null))), null);
                }
                else if (theproperty.PropertyType.Name == "DateTime")
                {
                    if (theproperty.GetValue(_object, null).ToString().Contains("0001-1-1") || theproperty.GetValue(_object, null) == null)
                    {
                        theproperty.SetValue(_object, DateTime.Parse("1900-1-1"), null);
                    }
                }
            }

            return _object;
        }
        
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="_text">明文文本</param>
        /// <returns></returns>
        public static string MD5Encode(string _text)
        {
            byte[] result = Encoding.Default.GetBytes(_text.Trim()); 
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            //输出加密文本
            return BitConverter.ToString(output).Replace("-", "");  
        }


        /// <summary>
        /// 将半角字符转换为全角字符(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>        
        public static string ConvertToSBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 将全角字符转换为半角字符(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ConvertToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

    }
}
