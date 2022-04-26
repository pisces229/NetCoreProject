using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreProject.Domain.Util
{
    public class ValidateMessageUtil
    {
        public ValidateMessageUtil()
        { 
        
        }
        public string Required(string name) => $"[{name}]不可空白";
        public string Password(string name) => $"[{name}]密碼強度不符";
        public string Email(string name) => $"[{name}]信箱格式不正確";
    }
}
