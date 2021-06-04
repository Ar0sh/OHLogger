using APIDigger.Classes;
using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace APIDigger
{
    public partial class OpenHABRest : Window
    {
        private readonly List<string> Items = new List<string>();
        private readonly APILookup getData = new APILookup();
        public static List<Items> ItemsList = new List<Items>();
        readonly List<string> ApiElements = new List<string>();
        private Thread TableRefresh = null;
        public bool loggedIn = false;
        CreateTable tables = new CreateTable();
        CreateTable createTable = new CreateTable();
        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            Title = "Openhab REST Items";
        }

        void Run()
        {
            APILookup.RestConn();
            Load();
            getData.PopulateDataTable();
            dgSensors.DataContext = getData.ItemsTable.AsDataView(); 
            foreach (Items item in ItemsList)
            {
                if(!tables.Tables.Contains(item.name))
                { 
                    createTable.CreateTables(item.name);
                }
            }
            if (getData.ItemsTable.Rows.Count > 0)
            {
                tbStateValue.Background = Brushes.Green;
                TableRefresh = new Thread(Update);
                TableRefresh.Start();
            }
            else
            {
                tbStateValue.Background = Brushes.Red;
            }
            
        }

        private void API_UpdateDict(bool update = false)
        {
            if (!update)
                getData.PopulateItemsDict();
            else
                getData.UpdateItemsDict();

        }

        private void Load()
        {
            ApiElements.Clear();
            getData.ItemsDict.Clear();
            Items.Clear();
            API_UpdateDict();
        }

        void Update()
        {
            while(TableRefresh.IsAlive)
            {
                try
                {
                    Items.Clear();
                    API_UpdateDict(true);
                    dgSensors.Dispatcher.Invoke(() =>
                    {
                        if (dgSensors.IsKeyboardFocusWithin)
                        {
                            dgSensors.Items.Refresh();
                            dgSensors.Focus();
                        }
                        else
                            dgSensors.Items.Refresh();
                    });
                    Thread.Sleep(Properties.Settings.Default.UpdateInterval * 1000);
                }
                catch
                {

                }
            }
        }
        private void TbUpdateSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Functions.IsTextAllowed(e.Text, @"[^0-9]");
        }

        private void BtnReloadUpd_Click(object sender, RoutedEventArgs e)
        {
            Functions.SaveUpdateInterval(tbUpdateSpeed.Text);
        }

        private void BtnLogInSql_Click(object sender, RoutedEventArgs e)
        {
            Functions.SaveSqlUser(userSql.Text, passSql.Password);
            string conStr = ConSQL.GetConnectionString_up();
            SqlConnection conn = new SqlConnection(conStr);
            try
            {
                conn.Open();
                userSql.IsEnabled = false;
                passSql.IsEnabled = false;
                userSql.Background = Brushes.Green;
                passSql.Background = Brushes.Green;
                loggedIn = true;
                tables.GetSqlTables();
                conn.Close();
                Run();
            }
            catch
            {
                userSql.Text = "";
                passSql.Password = "";
                userSql.Background = Brushes.Red;
                passSql.Background = Brushes.Red;
            }
            finally
            {
                conn.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(loggedIn)
                TableRefresh.Abort();
        }
    }
}
