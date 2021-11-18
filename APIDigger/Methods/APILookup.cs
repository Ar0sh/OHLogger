using OHDataLogger.Classes;
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

namespace OHDataLogger.Methods
{
    public class APILookup
    {
        public SortedDictionary<string, SensorValues> ItemsDict = new SortedDictionary<string, SensorValues>();
        public DataTable ItemsTable = new DataTable("Items");
        private bool tableTest = false;
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

        public void UpdateItemsDict(bool? enabled = null)
        {
            RestConn();
            if (OpenHABRest.ItemsList.Count != ItemsDict.Count)
            {
                foreach (Items item in OpenHABRest.ItemsList)
                {
                    if(!ItemsDict.ContainsKey(item.name))
                    {
                        ItemsDict.Add(item.name, new SensorValues(item.link, item.state, null, item.editable, item.type, item.name, item.label));
                        OpenHABRest.AddedSensor.Add(item);
                    }
                }
            }
            List<Items> list = OpenHABRest.ItemsList.ToList();
            foreach (Items item in list)
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
                                //dr[3] = DateTime.Now;
                                dr[3] = OpenHABRest.dtApi;
                            }
                        }
                    }
                }
            }
        }

        public void EnableItems(bool enabled, string _name = null)
        {
            List<Items> list = OpenHABRest.ItemsList.ToList();
            foreach (Items item in list)
            {
                string name = item.name;
                foreach (DataRow dr in ItemsTable.Rows)
                {
                    if (dr["Name"].ToString() == ItemsDict[name].GetName())
                    {
                        if (enabled)
                        {
                            if (dr[4].ToString() == "False")
                            {
                                dr[4] = true;
                                if (!Properties.Settings.Default.Enabled.Contains(dr["Name"].ToString()))
                                {
                                    Properties.Settings.Default.Enabled.Add(dr["Name"].ToString());
                                }
                            }
                        }
                        else if (!enabled)
                        {
                            if (dr[4].ToString() == "True")
                            {
                                dr[4] = false;
                                if (Properties.Settings.Default.Enabled.Contains(dr["Name"].ToString()))
                                {
                                    _ = Properties.Settings.Default.Enabled.Remove(dr["Name"].ToString());
                                }
                            }
                        }
                    }
                }
                Properties.Settings.Default.Save();
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
                ItemsTable.Columns.Add("Enabled");
            }
            foreach (Items item in OpenHABRest.ItemsList)
            {
                if(!exclude.Contains(item.name))
                    ItemsTable.Rows.Add(
                        item.name, 
                        item.label, 
                        item.state, 
                        DateTime.Now,
                        Properties.Settings.Default.Enabled.Contains(item.name) ? true : false
                        );
            }
            if(tableTest)
            {
                tableTest = false;
            }
        }

        public void UpdateDataTable(List<Items> itemList)
        {
            for(int i = 0; i < itemList.Count; i++)
                if (!exclude.Contains(itemList[i].name))
                    ItemsTable.Rows.Add(
                        itemList[i].name, 
                        itemList[i].label, 
                        itemList[i].state, 
                        DateTime.Now, 
                        Properties.Settings.Default.Enabled.Contains(itemList[i].name) ? true : false
                        );
        }

        public void RestConn(bool checkCon = false)
        {
            OpenHABRest._CheckApiCon = true;
            OpenHABRest.ItemsListTemp = OpenHABRest.ItemsList.ToList();
            OpenHABRest.ItemsList.Clear();
            RestClient restClient = new RestClient("http://" + Properties.Settings.Default.ApiAddr + "/");
            RestRequest request = new RestRequest("rest/items/", Method.GET);
            List<Items> queryResult = restClient.Execute<List<Items>>(request).Data;
            if (queryResult != null && !checkCon)
            {
                foreach (Items item in queryResult)
                {
                    if (item.type != "Group" && !exclude.Contains(item.name))
                    {
                        OpenHABRest.ItemsList.Add(item);
                    }
                }
                if (OpenHABRest.ApiColor != Brushes.Green)
                {
                    OpenHABRest.ApiColor = Brushes.Green;
                    OpenHABRest.ApiMessages = "Api Connected";
                }
            }
            else if(queryResult == null && checkCon)
            {
                OpenHABRest._CheckApiCon = false;
            }
            else
            {
                if (OpenHABRest.ApiColor != Brushes.Red)
                {
                    OpenHABRest.ApiColor = Brushes.Red;
                    OpenHABRest.ApiMessages = "Api Disconnected";
                }
            }
        }
    }
}
