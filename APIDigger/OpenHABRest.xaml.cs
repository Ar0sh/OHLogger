using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private readonly List<string> ItemMembers = new List<string>();
        private readonly List<string> Items = new List<string>();
        private readonly APILookup getData = new APILookup();
        readonly List<string> ApiElements = new List<string>();
        private Thread TableRefresh = null;
        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            Title = "RestAPIDigger";
            Load();
            getData.populateDataTable();
            dgSensors.DataContext = getData.ItemsTable.AsDataView();
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

        private void API_Method_Extract(string[] elements, string type)
        {
            if (type == "items")
            {
                foreach (string t in elements)
                {
                    if (t.Contains("\"link\"") && !t.Contains("\"members\""))
                    {
                        Items.Add(t.TrimStart('{').TrimEnd('}'));
                        ApiElements.Add(Items.Last().Split(new char[] { ',', '/' }, 
                            StringSplitOptions.RemoveEmptyEntries)[4].TrimEnd('"'));
                    }
                }
                ApiElements.Sort();
                
            }
            else if (type == "members")
            {
                foreach (string t in elements)
                {
                    if(t.Contains("\"members\""))
                        ItemMembers.Add(t);
                }
            }
        }

        private void API_Method_Call(string url, string type, bool update = false)
        {
            string[] APIElementsUnsorted = getData.OpenHab2Rest(url);
            API_Method_Extract(APIElementsUnsorted, type);
            if (!update)
                getData.populateItemsDict(Items);
            else
                getData.updateItemsDict(Items);

        }

        private void Load()
        {
            ApiElements.Clear();
            getData.ItemsDict.Clear();
            Items.Clear();
            API_Method_Call("http://192.168.1.151:8080/rest/items", "items");
        }

        void Update()
        {
            while(TableRefresh.IsAlive)
            {
                try
                {
                    Items.Clear();
                    API_Method_Call("http://192.168.1.151:8080/rest/items", "items", true);
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

        private void Window_Closed(object sender, EventArgs e)
        {
            TableRefresh.Abort();
        }
    }
}
