using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using APIDigger.Classes;

namespace APIDigger.Methods
{
    public class CreateTable
    {
        public void CreateTables(string name)
        {
            string ColTime = "time";
            string ColVal = "value";
            string conStr = ConSQL.GetConnectionString_up();
            SqlConnection conn = new SqlConnection(conStr);
            string cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NULL,[" + ColVal + "] [nvarchar](50) NULL) ON[PRIMARY]";
            conn.Open();
            
            SqlCommand sqlCommand = new SqlCommand(cmd, conn);
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
        }
        public List<string> Tables = new List<string>();

        public void GetSqlTables()
        {
            string conStr = ConSQL.GetConnectionString_up();
            SqlConnection conn = new SqlConnection(conStr);
            string cmd = "SELECT name FROM sys.Tables";
            conn.Open();

            SqlCommand sqlCommand = new SqlCommand(cmd, conn);
            try
            {
                SqlDataReader rd = sqlCommand.ExecuteReader();
                while(rd.Read())
                {
                    Tables.Add(rd[0].ToString());
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
        }
    }
}