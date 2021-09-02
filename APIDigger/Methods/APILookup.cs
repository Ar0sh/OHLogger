﻿using OHDataLogger.Classes;
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
        bool tableTest = false;
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
            if (OpenHABRest.ItemsList.Count != ItemsDict.Count)
            {
                foreach(Items item in OpenHABRest.ItemsList)
                {
                    if(!ItemsDict.ContainsKey(item.name))
                    { 
                        ItemsDict.Add(item.name, new SensorValues(item.link, item.state, null, item.editable, item.type, item.name, item.label));
                        OpenHABRest.AddedSensor.Add(item);
                    }
                }
            }
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
                                //dr[3] = DateTime.Now;
                                dr[3] = OpenHABRest.dtApi;
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
            if(tableTest)
            {
                tableTest = false;
            }
        }

        public void UpdateDataTable(List<Items> itemList)
        {
            for(int i = 0; i < itemList.Count; i++)
                if (!exclude.Contains(itemList[i].name))
                    ItemsTable.Rows.Add(itemList[i].name, itemList[i].label, itemList[i].state, DateTime.Now);
        }

        public void RestConn(bool checkCon = false)
        {
            OpenHABRest._CheckApiCon = true;
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
