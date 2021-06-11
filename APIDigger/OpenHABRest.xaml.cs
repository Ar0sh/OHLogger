using APIDigger.Classes;
using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Thread SqlStoreTask = null;
        public bool loggedIn = false;
        //DataSqlClasses tables = new DataSqlClasses();
        readonly DataSqlClasses dataSqlClasses = new DataSqlClasses();
        public static string conStr;
        public static SqlConnection conn;
        public static string ApiMessages = "Api Connected";
        public static Brush ApiColor = Brushes.Green;
        public static string SqlMessages = "Sql Connected";
        public static Brush SqlColor = Brushes.Green;


        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            statSqlCon.Text = SqlMessages;
            statApiCon.Text = ApiMessages;
            Title = "Openhab REST Items"; 
            if (Properties.Settings.Default.RememberLogin)
            {
                userSql.Text = Properties.Settings.Default.UserSql;
                passSql.Password = Properties.Settings.Default.PassSql;
                chkRemember.IsChecked = true;
                BtnLogInSql_Click(null, null);
            }
        }

        void Run()
        {
            getData.RestConn();
            Load();
            getData.PopulateDataTable();
            dgSensors.DataContext = getData.ItemsTable.AsDataView();
            foreach (Items item in ItemsList)
            {
                if (!dataSqlClasses.Tables.Contains(item.name))
                {
                    dataSqlClasses.CreateTables(item.name, item.type);
                }
            }
            if (getData.ItemsTable.Rows.Count > 0)
            {
                TableRefresh = new Thread(Update);
                TableRefresh.Start();
                SqlStoreTask = new Thread(StoreSqlCall);
                SqlStoreTask.Start();
            }
            else
            {

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

        void StoreSqlCall()
        {
            while(SqlStoreTask.IsAlive)
            {
                dataSqlClasses.StoreValuesToSql();
                statSqlCon.Dispatcher.Invoke(() =>
                {
                    statSqlCon.Text = SqlMessages;
                    statSqlConItem.Background = SqlColor;
                });
                Thread.Sleep(59200);
            }
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
                    statApiCon.Dispatcher.Invoke(() =>
                    {
                        statApiCon.Text = ApiMessages; ;
                        statApiConItem.Background = ApiColor;

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
            Functions.SaveSqlUser(userSql.Text, passSql.Password, chkRemember.IsChecked); 
            
            try
            {
                conStr = ConSQL.GetConnectionString_up();
                conn = new SqlConnection(conStr);
                conn.Open();
                userSql.IsEnabled = false;
                passSql.IsEnabled = false;
                userSql.Background = Brushes.Green;
                passSql.Background = Brushes.Green;
                loggedIn = true;
                conn.Close();
                dataSqlClasses.GetSqlTables();
                statSqlCon.Text = SqlMessages;
                statSqlConItem.Background = SqlColor;
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
            if (loggedIn)
            {
                TableRefresh.Abort();
                SqlStoreTask.Abort();
                if(chkRemember.IsChecked == false)
                {
                    Properties.Settings.Default.UserSql = "";
                    Properties.Settings.Default.PassSql = "";
                    Properties.Settings.Default.Save();
                }
                conn.Close();
            }
        }

        private void PassSql_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return)
            {
                btnSqlLogin.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TextBlock content = (TextBlock)dgSensors.SelectedCells[0].Column.GetCellContent(dgSensors.SelectedCells[0].Item);
            Process.Start("http://192.168.1.161:8082/rest/items/" + content.Text);
        }

        private void BtnSqlLogout_Click(object sender, RoutedEventArgs e)
        {
            if (loggedIn)
            {
                TableRefresh.Abort();
                SqlStoreTask.Abort(); 
                userSql.IsEnabled = true;
                passSql.IsEnabled = true;
                if (!Properties.Settings.Default.RememberLogin)
                {
                    userSql.Text = "";
                    passSql.Password = "";
                }
                userSql.Background = Brushes.White;
                passSql.Background = Brushes.White;
                statSqlCon.Text = "Sql Disconnected";
                statSqlConItem.Background = Brushes.Red;
                statApiCon.Text = "Api Disconnected";
                statApiConItem.Background = Brushes.Red;
                getData.ItemsTable.Clear();
                loggedIn = false;
            }
        }

        private void ChkRemember_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RememberLogin = true;
            Properties.Settings.Default.Save();

        }

        private void ChkRemember_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RememberLogin = false;
            Properties.Settings.Default.Save();
        }
    }
}
