using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using APIDigger.Classes;
using System.Windows;
using System.Windows.Media;

namespace APIDigger.Methods
{
    public class DataSqlClasses
    {
        public List<string> Tables = new List<string>();

        public void CreateTables(string name, string type)
        {
            string ColTime = "time";
            string ColVal = "value"; 
            //string conStr = ConSQL.GetConnectionString_up();
            //SqlConnection conn = new SqlConnection(conStr);
            string cmd;
            if (type.ToLower() == "dimmer" || type.ToLower().Contains("number"))
            {
                cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NOT NULL,[" + ColVal + "] [float] DEFAULT NULL) ON[PRIMARY]";
            }
            else if(type.ToLower() == "datetime")
            {
                cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NOT NULL,[" + ColVal + "] [datetime2](7) DEFAULT NULL) ON[PRIMARY]";
            }
            else if(type.ToLower() == "switch")
            {
                cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NOT NULL,[" + ColVal + "] [nvarchar](6) DEFAULT NULL) ON[PRIMARY]";
            }
            else if(type.ToLower() == "color")
            {
                cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NOT NULL,[" + ColVal + "] [nvarchar](70) DEFAULT NULL) ON[PRIMARY]";
            }
            else
            {
                cmd = "CREATE TABLE [dbo].[" + name + "]([" + ColTime + "][datetime2](7) NULL,[" + ColVal + "] [nvarchar](50) NULL) ON[PRIMARY]";
            }
            OpenHABRest.conn.Open();

            SqlCommand sqlCommand = new SqlCommand(cmd, OpenHABRest.conn);
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                OpenHABRest.conn.Close();
            }
        }

        public void GetSqlTables()
        {
            //string conStr = ConSQL.GetConnectionString_up();
            //SqlConnection conn = new SqlConnection(conStr);
            string cmd = "SELECT name FROM sys.Tables";
            OpenHABRest.conn.Open();

            SqlCommand sqlCommand = new SqlCommand(cmd, OpenHABRest.conn);
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
                OpenHABRest.conn.Close();
            }
        }
        public void StoreValuesToSql()//Items item)
        {
            string query = "DECLARE @Time AS DATETIME2(3)\nSET @Time = GETUTCDATE()\n";
            List<Items> ItemsListCopy = OpenHABRest.ItemsList.ToList();
            foreach (Items item in ItemsListCopy)
            {
                if (item.type.ToLower() == "switch" || item.type.ToLower() == "color")
                {
                    query += "insert into " + item.name + " (time, value) values (@Time, '" + item.state.Split(' ')[0] + "') \n";
                }
                else if (item.type.ToLower() == "datetime")
                {
                    query += "insert into " + item.name + " (time, value) values (@Time, '" + item.state.Split('+')[0] + "') \n";
                }
                else if (item.type.ToLower() == "string")
                {
                    query += "insert into " + item.name + " (time, value) values (@Time, '" + item.state + "') \n";
                }
                else
                {
                    query += "insert into " + item.name + " (time, value) values (@Time, " + item.state.Split(' ')[0] + ") \n";
                }
            }

            SqlCommand sqlCommand = new SqlCommand(query, OpenHABRest.conn);
            try
            {
                OpenHABRest.conn.Open();
                sqlCommand.ExecuteNonQuery();
                if(OpenHABRest.SqlColor != Brushes.Green)
                {
                    OpenHABRest.SqlColor = Brushes.Green;
                    OpenHABRest.SqlMessages = "Sql Connected";
                }
            }
            catch
            {
                //MessageBox.Show("Error" + item.name + " " + item.state, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //OpenHABRest.ErrorMessages = "Error: Item: " + item.name + ", State: " + item.state;
                if (OpenHABRest.SqlColor != Brushes.Red)
                {
                    OpenHABRest.SqlColor = Brushes.Red;
                    OpenHABRest.SqlMessages = "Sql Disconnected";
                }
            }
            finally
            {
                OpenHABRest.conn.Close();
            }
        }
    }
}