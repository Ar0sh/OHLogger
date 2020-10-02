using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public partial class MainWindow : Window
    {
        private List<string> ItemMembers = new List<string>();
        private List<string> Items = new List<string>();
        private APILookup getData = new APILookup();
        List<string> ApiElements = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
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

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            ApiElements.Clear();
            getData.ItemsDict.Clear();
            Items.Clear();
            API_Method_Call("http://192.168.1.151:8080/rest/items", "items");
            cbItems.ItemsSource = ApiElements.Select(x => x.ToString());
            cbItems.SelectedIndex = 0;
            
        }

        private void CbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbItemName.Text = (sender as ComboBox).SelectedItem.ToString();
            if(getData.ItemsDict.Count > 0)
                tbStateValue.Text = getData.ItemsDict[(sender as ComboBox).SelectedItem.ToString()].GetState();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Update(true);
        }

        private void Update(bool start)
        {
            if (start)
            {
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        Items.Clear();
                        UpdateStatus("http://192.168.1.151:8080/rest/items", "items");
                        Dispatcher.Invoke(() =>
                        {
                            if (getData.ItemsDict.Count > 0)
                                tbStateValue.Text = getData.ItemsDict[cbItems.SelectedItem.ToString()].GetState();
                        });
                        Thread.Sleep(10000);
                    }
                });
                task.Start();
               
            }
        }
    }
}
