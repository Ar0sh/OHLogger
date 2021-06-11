using APIDigger.Classes;
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
        public SortedDictionary<string, SensorValues> ItemsDict = new SortedDictionary<string, SensorValues>();
        public DataTable ItemsTable = new DataTable("Items");
        private readonly List<string> exclude = new List<string>
        {
            "test",
            "zwave_device_93139763_node20_switch_dimmer1",
            "hue_scenes"
        };

        public void PopulateItemsDict()
        {
            RestConn();
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if(!exclude.Contains(item.name))
                {
                    string link = item.link;
                    string state = item.state;
                    string pattern = null;
                    bool editable = item.editable;
                    string type = item.type;
                    string name = item.name;
                    string label = item.label;
                    if(item.stateDescription != null && item.stateDescription.Contains("pattern"))
                    {
                        pattern = item.stateDescription.Split(':')[1].Split(',')[0].TrimStart('"').TrimEnd('"');
                    }
                    ItemsDict.Add(name, new SensorValues(link, state, pattern, editable, type, name, label));
                }
            }
        }

        public void UpdateItemsDict()
        {
            RestConn();
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if (!exclude.Contains(item.name))
                {
                    string name = item.name;
                    string state = item.state;
                    if (ItemsDict[name].GetState() != state)
                    {
                        ItemsDict[name].SetState(state);
                        foreach (DataRow dr in ItemsTable.Rows)
                        {
                            if (dr["Name"].ToString() == ItemsDict[name].GetName())
                            {
                                dr[2] = state;
                                dr[3] = DateTime.Now;
                            }
                        }
                    }
                }
            }
        }

        public void PopulateDataTable()
        {
            ItemsTable.Clear();
            if(ItemsTable.Columns.Count == 0)
            { 
                ItemsTable.Columns.Add("Name");
                ItemsTable.Columns.Add("Label");
                ItemsTable.Columns.Add("State");
                ItemsTable.Columns.Add("UpdateTime");
            }
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if(!exclude.Contains(item.name))
                    ItemsTable.Rows.Add(item.name, item.label, item.state, DateTime.Now);
            }

        }

        public void RestConn()
        {
            OpenHABRest.ItemsList.Clear();
            var restClient = new RestClient("http://192.168.1.161:8082/");
            var request = new RestRequest("rest/items/", Method.GET);
            var queryResult = restClient.Execute<List<Items>>(request).Data;
            if(queryResult != null)
            {
                foreach (Items item in queryResult)
                {
                    if (item.type != "Group" && !exclude.Contains(item.name))
                        OpenHABRest.ItemsList.Add(item);
                }
                if(OpenHABRest.ApiColor != Brushes.Green)
                { 
                    OpenHABRest.ApiColor = Brushes.Green;
                    OpenHABRest.ApiMessages = "Api Connected";
                }
            }
            else
            {
                if(OpenHABRest.ApiColor != Brushes.Red)
                { 
                    OpenHABRest.ApiColor = Brushes.Red;
                    OpenHABRest.ApiMessages = "Api Disconnected";
                }
            }
        }
    }
}
