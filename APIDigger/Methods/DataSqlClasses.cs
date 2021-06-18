using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using OHDataLogger.Classes;
using System.Windows;
using System.Windows.Media;

namespace OHDataLogger.Methods
{
    public class DataSqlClasses
    {
        public List<string> Tables = new List<string>();

        public void CreateTables(string name, string type)
        {
            string ColTime = "time";
            string ColVal = "value"; 
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
            try
            {
                OpenHABRest.conn.Open();
                SqlCommand sqlCommand = new SqlCommand(cmd, OpenHABRest.conn);
                sqlCommand.ExecuteNonQuery();
                if (OpenHABRest.SqlTabColor != Brushes.Green)
                {
                    OpenHABRest.SqlTabColor = Brushes.Green;
                    OpenHABRest.SqlTabMessage = "";
                }
            }
            catch (SqlException sqlEx)
            {
                if (OpenHABRest.SqlTabColor != Brushes.Red)
                {
                    OpenHABRest.SqlTabColor = Brushes.Red;
                    OpenHABRest.SqlTabMessage = "Create Table Error: " + sqlEx.Message + "...";
                }
            }
            finally
            {
                OpenHABRest.conn.Close();
            }
        }

        public void GetSqlTables()
        {
            Tables.Clear();
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
        public void StoreValuesToSql()
        {
            string query = "DECLARE @Time AS DATETIME2(3)\nSET @Time = GETUTCDATE()\n";
            List<Items> ItemsListCopy = OpenHABRest.ItemsList.ToList();
            string value;
            foreach (Items item in ItemsListCopy)
            {
                if (item.type.ToLower() == "switch" || item.type.ToLower() == "color")
                {
                    value = "'" + item.state.Split(' ')[0] + "'";
                }
                else if (item.type.ToLower() == "datetime")
                {
                    value = "'" + item.state.Split('+')[0] + "'";
                }
                else if (item.type.ToLower() == "string")
                {
                    value = "'" + item.state + "'";
                }
                else
                {
                    value = item.state.Split(' ')[0];
                }
                query += "insert into " + item.name + " (time, value) values (@Time, " + value + ") \n";
            }

            SqlCommand sqlCommand = new SqlCommand(query, OpenHABRest.conn);
            try
            {
                OpenHABRest.conn.Open();
                sqlCommand.ExecuteNonQuery(); 
                if (OpenHABRest.SqlErrColor != Brushes.Green)
                {
                    OpenHABRest.SqlErrColor = Brushes.Green;
                    OpenHABRest.SqlErrMessage = "No SQL Errors";
                }
                if (OpenHABRest.SqlColor != Brushes.Green)
                {
                    OpenHABRest.SqlColor = Brushes.Green;
                    OpenHABRest.SqlMessages = "SQL Connected";
                }
            }
            catch(SqlException sqlEx)
            {
                if(OpenHABRest.SqlErrColor != Brushes.Red)
                {
                    OpenHABRest.SqlErrColor = Brushes.Red;
                    OpenHABRest.SqlErrMessage = sqlEx.Message.Substring(0, 40) + "...";
                }
                if (OpenHABRest.SqlColor != Brushes.Red)
                {
                    OpenHABRest.SqlColor = Brushes.Red;
                    OpenHABRest.SqlMessages = "SQL Error";
                }
            }
            finally
            {
                OpenHABRest.conn.Close();
            }
        }
    }
}