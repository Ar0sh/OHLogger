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

        public void PopulateItemsDict(List<string> list)
        {
            foreach(var element in list)
            {
                if (element.Contains("test") == false && 
                    element.Contains("zwave_device_93139763_node20_switch_dimmer1") == false &&
                    element.Contains("hue_scenes") == false)
                {
                    string name;
                    string link;
                    string state = null;
                    string pattern = null;
                    bool? readOnly = null;
                    string options = null;
                    Brushes color = null;
                    string[] chopped = element.Split(new char[] { ',', '/' },
                                StringSplitOptions.RemoveEmptyEntries);
                    link = chopped[0].Split(':')[1].TrimStart('"').TrimEnd('"') + "://" +
                                  chopped[1].TrimStart('"').TrimEnd('"') + "/" +
                                  chopped[2].TrimStart('"').TrimEnd('"') + "/" +
                                  chopped[3].TrimStart('"').TrimEnd('"') + "/" +
                                  chopped[4].TrimStart('"').TrimEnd('"');
                    name = chopped[4].TrimEnd('"');
                    for (int i = 5; i < chopped.Length; i++)
                    {
                        if (chopped[i].Contains("\"state\""))
                        {
                            state = chopped[i].Split(':')[1].TrimStart('"').TrimEnd('"');
                        }
                        else if (chopped[i].Contains("pattern\""))
                        {
                            if (chopped[i].Contains("stateDescription\""))
                                pattern = chopped[i].Split(':')[2].TrimStart('"').TrimEnd('"');
                        }
                    }
                    ItemsDict.Add(name, new SensorValues(link, state, pattern, readOnly, options, name, color));
                }
            }
        }

        public void UpdateItemsDict(List<string> list)
        {
            foreach (var element in list)
            {
                if (element.Contains("test") == false &&
                    element.Contains("zwave_device_93139763_node20_switch_dimmer1") == false &&
                    element.Contains("hue_scenes") == false)
                {
                    string name = null;
                    string state = null;
                    string[] chopped = element.Split(new char[] { ',', '/' },
                                StringSplitOptions.RemoveEmptyEntries);
                    name = chopped[4].TrimEnd('"');
                    for (int i = 5; i < chopped.Length; i++)
                    {
                        if (chopped[i].Contains("\"state\""))
                        {
                            state = chopped[i].Split(':')[1].TrimStart('"').TrimEnd('"');
                        }
                    }
                    if (ItemsDict[name].GetState() != state)
                    {
                        ItemsDict[name].SetState(state);
                        foreach(DataRow dr in ItemsTable.Rows)
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

        public void PopulateDataTable()
        {
            ItemsTable.Clear();
            ItemsTable.Columns.Add("Name");
            ItemsTable.Columns.Add("State");
            ItemsTable.Columns.Add("UpdateTime");
            foreach (var t in ItemsDict)
            {
                ItemsTable.Rows.Add(t.Key, t.Value.GetState(), DateTime.Now.ToLongTimeString());
            }

        }
    }
}
