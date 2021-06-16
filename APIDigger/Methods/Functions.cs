using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APIDigger.Methods
{
    class Functions
    {
        public static bool IsTextAllowed(string Text, string AllowedRegex)
        {
            try
            {
                var regex = new Regex(AllowedRegex);
                return !regex.IsMatch(Text);
            }
            catch
            {
                return true;
            }
        }
        public static void SaveUpdateInterval(string input)
        {
            Properties.Settings.Default.UpdateInterval = Convert.ToInt32(input);
            Properties.Settings.Default.Save();
        }
        public static void SaveApiDetails(string _addr, bool? save)
        {
            Properties.Settings.Default.ApiAddr = _addr;
            if(save == true)
                Properties.Settings.Default.Save();
        }
        public static void SaveSqlUser(string sqlip, string sqldbname, string user, string pass, bool? save)
        {
            if (sqlip.Contains(":"))
            {
                Properties.Settings.Default.SqlIpAddr = sqlip.Split(':')[0];
                Properties.Settings.Default.SqlPort = sqlip.Split(':')[1];
            }
            else
            {
                Properties.Settings.Default.SqlIpAddr = sqlip;
                Properties.Settings.Default.SqlPort = "";
            }
            Properties.Settings.Default.SqlDbName = sqldbname;
            Properties.Settings.Default.UserSql = user;
            Properties.Settings.Default.PassSql = pass;
            if (save == true)
                Properties.Settings.Default.Save();
        }
    }
}
