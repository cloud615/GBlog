using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBlog.Model.CustomAttribute
{
    public class SignModelAttribute : Attribute
    {
        public string PrimaryKey { get; set; }
        public string TableName { get; set; }
    }
}
