using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDigger.Classes
{
    internal class ConSQL
    {
		// Get the connection string from App config file.
		protected static internal string GetConnectionString_up()
		{
			string connString;
			if (Properties.Settings.Default.SqlPort != "")
				connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "," + Properties.Settings.Default.SqlPort + 
					"; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + Properties.Settings.Default.UserSql + "; password=" + 
					Properties.Settings.Default.PassSql + ";Connection Timeout=1";
			else
				connString = "Server=" + Properties.Settings.Default.SqlIpAddr + "; database=" + Properties.Settings.Default.SqlDbName + "; UID=" + 
					Properties.Settings.Default.UserSql +  "; password=" + Properties.Settings.Default.PassSql + ";Connection Timeout=1";
			return connString;
		}
	}
	class CheckSql
    {

		
	}
}
