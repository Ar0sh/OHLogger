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
		// Get the connection string from App config file.
		protected static internal string GetConnectionString_up()
		{
			//SqlConnectionStringBuilder sqlConnectionStringBuilder;
            string connString;
            if (Properties.Settings.Default.SqlPort != "")
                connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "," + Properties.Settings.Default.SqlPort +
                    "; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + Properties.Settings.Default.UserSql + "; password=" +
                    Properties.Settings.Default.PassSql + ";Connection Timeout=1";
            else
                connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "; database=" + Properties.Settings.Default.SqlDbName + "; UID=" +
                    Properties.Settings.Default.UserSql + "; password=" + Properties.Settings.Default.PassSql + ";Connection Timeout=1";
            return connString;
            //if (Properties.Settings.Default.SqlPort != "")
            //{
            //	sqlConnectionStringBuilder = new SqlConnectionStringBuilder(
            //	"Server=" + Properties.Settings.Default.SqlIpAddr + "," + Properties.Settings.Default.SqlPort +
            //		"; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + Properties.Settings.Default.UserSql + "; password=" +
            //		Properties.Settings.Default.PassSql)
            //	{
            //		ConnectTimeout = 1,
            //		AsynchronousProcessing = true
            //	};
            //}
            //else
            //         {
            //	sqlConnectionStringBuilder = new SqlConnectionStringBuilder(
            //	"Server=" + Properties.Settings.Default.SqlIpAddr + "," + Properties.Settings.Default.SqlPort +
            //		"; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + Properties.Settings.Default.UserSql + "; password=" +
            //		Properties.Settings.Default.PassSql)
            //	{
            //		ConnectTimeout = 1,
            //		AsynchronousProcessing = true
            //	};
            //}
            //return sqlConnectionStringBuilder;
        }
    }
	class CheckSql
    {

		
	}
}
