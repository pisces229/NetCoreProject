using System.Text.RegularExpressions;

namespace NetCoreProject.Domain.Util
{
    public class ValidateValueUtil
    {
        public ValidateValueUtil()
        { 
        
        }
        public bool Password(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[a-zA-Z])(?=.*\W).{8,20}$");
        }
        public bool Email(string strIn)
        {
            //return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return Regex.IsMatch(strIn, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }
    }
}
