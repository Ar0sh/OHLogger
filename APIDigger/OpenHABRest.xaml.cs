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
        DataSqlClasses dataSqlClasses = new DataSqlClasses();
        public static string conStr;
        public static SqlConnection conn;
        public static string ErrorMessages = "Status = OK";
        public static Brush ErrorColor = Brushes.Green;
        public static int ErrorCount = 0;


        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            Title = "Openhab REST Items";
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
                ErrorCount = 0;
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
                    statStatus.Dispatcher.Invoke(() =>
                    {
                        statStatus.Text = "Number of errors: " + ErrorCount;
                        if (ErrorCount > 0)
                        {
                            statStatusItem.Background = ErrorColor;
                        }
                        else
                        {
                            statStatusItem.Background = Brushes.Green;
                        }

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
                statSqlCon.Text = "Sql Connected";
                statSqlConItem.Background = Brushes.Green;
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
                conn.Close();
            }
        }

        private void passSql_KeyDown(object sender, KeyEventArgs e)
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
    }
}
