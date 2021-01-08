using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APIDigger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OpenHABRest : Window
    {
        private List<string> ItemMembers = new List<string>();
        private List<string> Items = new List<string>();
        private APILookup getData = new APILookup();
        CancellationTokenSource source = new CancellationTokenSource();
        List<string> ApiElements = new List<string>();
        public OpenHABRest()
        {
            InitializeComponent();
            tbUpdateSpeed.Text = Properties.Settings.Default.UpdateInterval.ToString();
            Load();
            getData.populateDataTable();
            dgSensors.DataContext = getData.ItemsTable.AsDataView();
            int updateInt = Convert.ToInt32(tbUpdateSpeed.Text) * 1000;
            if (getData.ItemsTable.Rows.Count > 0)
            {
                tbStateValue.Background = Brushes.Green;
                Updates(updateInt);
            }
            else
            {
                tbStateValue.Background = Brushes.Red;
            }
        }
        private void Updates(int updateInt)
        {
            Update(true, updateInt);
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

        private void UpdateStatus(string url, string type)
        {
            string[] APIElementsUnsorted = getData.OpenHab2Rest(url);
            API_Method_Extract(APIElementsUnsorted, type);
            getData.updateItemsDict(Items);
        }

        private void API_Method_Call(string url, string type)
        {
            string[] APIElementsUnsorted = getData.OpenHab2Rest(url);
            API_Method_Extract(APIElementsUnsorted, type);
            getData.populateItemsDict(Items);

        }

        private void Load()
        {
            ApiElements.Clear();
            getData.ItemsDict.Clear();
            Items.Clear();
            API_Method_Call("http://192.168.1.151:8080/rest/items", "items");
        }

        private async void Update(bool start, int updateInt)
        {
            source = new CancellationTokenSource();
            var token = source.Token;
            if (start)
            {
                await Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Items.Clear();
                        UpdateStatus("http://192.168.1.151:8080/rest/items", "items");
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
                        Thread.Sleep(updateInt);
                    }
                }, token);
               
            }
        }
        private void tbUpdateSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ! RegexRules.IsTextAllowed(e.Text, @"[^0-9]");
        }

        private void ReloadUpdateInterval()
        {
            source.Cancel();
            Update(true, Convert.ToInt32(tbUpdateSpeed.Text) * 1000);
            Properties.Settings.Default.UpdateInterval = Convert.ToInt32(tbUpdateSpeed.Text);
            Properties.Settings.Default.Save();
        }

        private void btnReloadUpd_Click(object sender, RoutedEventArgs e)
        {
            ReloadUpdateInterval();
        }
    }
}
