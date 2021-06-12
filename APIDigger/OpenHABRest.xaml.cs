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
        private readonly APILookup getApiData = new APILookup();
        private readonly DataSqlClasses dataSqlClasses = new DataSqlClasses();
        private Thread TableRefresh = null;
        private Thread SqlStoreTask = null;
        private bool loggedIn = false;

        public static List<Items> ItemsList = new List<Items>();
        public static string conStr;
        public static SqlConnection conn;
        public static string ApiMessages = "API Connected";
        public static Brush ApiColor = Brushes.Green;
        public static string SqlMessages = "SQL Connected";
        public static Brush SqlColor = Brushes.Green;
        public static string SqlErrMessage = "No SQL Errors";
        public static Brush SqlErrColor = Brushes.Green;
        public static string SqlTabMessage = "";
        public static Brush SqlTabColor = Brushes.Green;


        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            statSqlCon.Text = SqlMessages;
            statApiCon.Text = ApiMessages;
            statSqlErr.Text = SqlErrMessage;
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
            getApiData.RestConn();
            Load();
            getApiData.PopulateDataTable();
            dgSensors.DataContext = getApiData.ItemsTable.AsDataView();
            foreach (Items item in ItemsList)
            {
                if (!dataSqlClasses.Tables.Contains(item.name))
                {
                    dataSqlClasses.CreateTables(item.name, item.type);
                }
            }
            if(SqlTabMessage != "")
            {
                statSqlTab.Text = SqlTabMessage;
                statSqlTabItem.Visibility = Visibility.Visible;
            }
            if (getApiData.ItemsTable.Rows.Count > 0)
            {
                TableRefresh = new Thread(Update);
                TableRefresh.Start();
                SqlStoreTask = new Thread(StoreSqlCall);
                SqlStoreTask.Start();
            }
        }

        private void Load()
        {
            getApiData.ItemsDict.Clear();
            Items.Clear();
            API_UpdateDict();
        }

        void Update()
        {
            while (TableRefresh.IsAlive)
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

        private void API_UpdateDict(bool update = false)
        {
            if (!update)
                getApiData.PopulateItemsDict();
            else
                getApiData.UpdateItemsDict();

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
                    statSqlErr.Text = SqlErrMessage;
                    statSqlErrItem.Background = SqlErrColor;
                });
                Thread.Sleep(59200);
            }
        }

        private void LogInOut(bool LogIn = true)
        {
            if(LogIn)
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
                    statSqlErr.Text = SqlErrMessage;
                    statSqlErrItem.Background = SqlErrColor;
                    btnSqlLogin.Content = "Log Out";
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
            else
            {
                if (loggedIn)
                {
                    if(TableRefresh != null)
                        TableRefresh.Abort();
                    if(SqlStoreTask != null)
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
                    statSqlCon.Text = "SQL Disconnected";
                    statSqlConItem.Background = Brushes.Red;
                    statApiCon.Text = "API Disconnected";
                    statApiConItem.Background = Brushes.Red;
                    getApiData.ItemsTable.Clear();
                    btnSqlLogin.Content = "Log In";
                    loggedIn = false;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                if(TableRefresh != null)
                    TableRefresh.Abort();
                if(SqlStoreTask != null)
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

        private void TbUpdateSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Functions.IsTextAllowed(e.Text, @"[^0-9]");
        }

        private void PassSql_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return)
            {
                btnSqlLogin.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void BtnLogInSql_Click(object sender, RoutedEventArgs e)
        {
            if (btnSqlLogin.Content.ToString() == "Log In")
                LogInOut();
            else
                LogInOut(false);
        }

        private void BtnReloadUpd_Click(object sender, RoutedEventArgs e)
        {
            Functions.SaveUpdateInterval(tbUpdateSpeed.Text);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TextBlock content = (TextBlock)dgSensors.SelectedCells[0].Column.GetCellContent(dgSensors.SelectedCells[0].Item);
            Process.Start("http://192.168.1.161:8082/rest/items/" + content.Text);
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
