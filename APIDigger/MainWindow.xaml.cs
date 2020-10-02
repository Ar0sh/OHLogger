using APIDigger.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void API_Method_Extract(string[] elements, string type)
        {
            List<string> ApiElements = new List<string>();
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
                cbItems.ItemsSource = ApiElements.Select(x => x.ToString());
                cbItems.SelectedIndex = 0;
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

        private void API_Method_Call(string url, string type)
        {
            string[] APIElementsUnsorted = getData.OpenHab2Rest(url);
            API_Method_Extract(APIElementsUnsorted, type);
            getData.populateItemsDict(Items);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            API_Method_Call("http://192.168.1.151:8080/rest/items", "items");
            string stop = "";
        }

        private void CbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbItemName.Text = (sender as ComboBox).SelectedItem.ToString();
            if(getData.ItemsDict.Count > 0)
                tbStateValue.Text = getData.ItemsDict[(sender as ComboBox).SelectedItem.ToString()].GetState();
        }
    }
}
