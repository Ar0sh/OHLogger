using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;

namespace APIDigger.Methods
{
    public class CreateTable
    {
        public void CreateTables(string name)
        {
            string ColTime = "time";
            string ColVal = "value";
            string conStr = ConSQL.GetConnectionString();
            SqlConnection conn = new SqlConnection(conStr);
            string cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NULL,[" + ColVal + "] [nvarchar](50) NULL) ON[PRIMARY]";
            conn.Open();
            SqlCommand sqlCommand = new SqlCommand(cmd, conn);
            sqlCommand.ExecuteNonQuery();
        }
    }
}