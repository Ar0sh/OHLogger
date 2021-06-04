using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace APIDigger.Methods
{
    public class APILookup
    {
        public string[] OpenHab2Rest(string URL)
        {
            var pattern = @"\{(.*?)}";
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(URL);
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            string[] RestList = Regex.Matches(content, pattern).Cast<Match>().Select(m => m.Value).ToArray();
            return RestList;
        }

        public SortedDictionary<string, SensorValues> ItemsDict = new SortedDictionary<string, SensorValues>();
        public DataTable ItemsTable = new DataTable("Items");

        public void PopulateItemsDict()
        {
            RestConn();
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if (item.name != "test" &&
                    item.name != "zwave_device_93139763_node20_switch_dimmer1" &&
                    item.name != "hue_scenes")
                {
                    string name;
                    string link;
                    string state = null;
                    string pattern = null;
                    bool? readOnly = null;
                    string options = null;
                    Brushes color = null;
                    link = item.link;
                    name = item.name;
                    state = item.state;
                    if(item.stateDescription != null && item.stateDescription.Contains("pattern"))
                    {
                        pattern = item.stateDescription.Split(':')[1].Split(',')[0].TrimStart('"').TrimEnd('"');
                    }
                    ItemsDict.Add(name, new SensorValues(link, state, pattern, readOnly, options, name, color));
                }
            }
        }

        public void UpdateItemsDict()
        {
            RestConn();
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if (item.name != "test" &&
                    item.name != "zwave_device_93139763_node20_switch_dimmer1" &&
                    item.name != "hue_scenes" )
                {
                    string name = null;
                    string state = null;
                    name = item.name;
                    state = item.state;
                    if (ItemsDict[name].GetState() != state)
                    {
                        ItemsDict[name].SetState(state);
                        foreach (DataRow dr in ItemsTable.Rows)
                        {
                            if (dr["Name"].ToString() == ItemsDict[name].GetName())
                            {
                                dr[1] = state;
                                dr[2] = DateTime.Now.ToLongTimeString();

                            }

                        }
                    }
                }
            }
        }

        public void PopulateDataTable(bool first = true)
        {
            ItemsTable.Clear();
            if(first)
            { 
                ItemsTable.Columns.Add("Name");
                ItemsTable.Columns.Add("State");
                ItemsTable.Columns.Add("UpdateTime");
            }
            foreach (Items a in OpenHABRest.ItemsList)
            {
                ItemsTable.Rows.Add(a.name, a.state, first ? DateTime.Now.ToLongTimeString() : "");
            }

        }



        public static void RestConn()
        {
            OpenHABRest.ItemsList.Clear();
            var restClient = new RestClient("http://192.168.1.161:8082/");
            var request = new RestRequest("rest/items/", Method.GET); 
            var queryResult = restClient.Execute<List<Items>>(request).Data;
            var test = request.Parameters;
            foreach(Items a in queryResult)
            {
                if (a.type != "Group")
                    OpenHABRest.ItemsList.Add(a);
            }
        }
    }
    public class Items
    {
        public string link { get; set; }
        public string state { get; set; }
        public string stateDescription { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string label { get; set; }

    }

}
