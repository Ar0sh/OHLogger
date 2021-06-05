﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using APIDigger.Classes;
using System.Windows;

namespace APIDigger.Methods
{
    public class DataSqlClasses
    {
        public List<string> Tables = new List<string>();

        public void CreateTables(string name, string type)
        {
            string ColTime = "time";
            string ColVal = "value"; 
            string conStr = ConSQL.GetConnectionString_up();
            SqlConnection conn = new SqlConnection(conStr);
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
        public void StoreValuesToSql(Items item)
        {
            string conStr = ConSQL.GetConnectionString_up();
            SqlConnection conn = new SqlConnection(conStr);
            string cmd;
            if(item.type.ToLower() == "switch" || item.type.ToLower() == "color")
            {
                cmd = "insert into " + item.name + " (time, value) values (CURRENT_TIMESTAMP, '" + item.state.Split(' ')[0] + "') ";
            }
            else if(item.type.ToLower() == "datetime")
            {
                cmd = "insert into " + item.name + " (time, value) values (CURRENT_TIMESTAMP, '" + item.state.Split('+')[0] + "') ";
            }
            else if(item.type.ToLower() == "string")
            {
                cmd = "insert into " + item.name + " (time, value) values (CURRENT_TIMESTAMP, '" + item.state + "') ";
            }
            else
            { 
                cmd = "insert into "+ item.name + " (time, value) values (CURRENT_TIMESTAMP, " + item.state.Split(' ')[0] + ") ";
            }
            conn.Open();

            SqlCommand sqlCommand = new SqlCommand(cmd, conn);
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Error" + item.name + " " + item.state, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}