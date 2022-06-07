using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHDataLogger.Classes
{
    internal class ConSQL
    {
        private static Secure_It secureIt = new Secure_It();
        // Get the connection string from App config file.
        protected static internal string GetConnectionString_up()
		{
            string connString;
            if (Properties.Settings.Default.SqlPort != "")
                connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "," + Properties.Settings.Default.SqlPort +
                    "; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + secureIt.DecryptString(Properties.Settings.Default.UserSql) + "; password=" +
                    secureIt.DecryptString(Properties.Settings.Default.PassSql) + ";Connection Timeout=1";
            else
                connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "; database=" + Properties.Settings.Default.SqlDbName + "; UID=" +
                    secureIt.DecryptString(Properties.Settings.Default.UserSql) + "; password=" + secureIt.DecryptString(Properties.Settings.Default.PassSql) + ";Connection Timeout=1";
            return connString;
        }
    }
}
