using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public Dictionary<string, SensorValues> ItemsDict = new Dictionary<string, SensorValues>();

        public void populateItemsDict(List<string> list)
        {
            foreach(var element in list)
            {
                string name = null;
                string link = null;
                string state = null;
                string pattern = null;
                bool? readOnly = null;
                string options = null;
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
                    if(chopped[i].Contains("state\""))
                    {
                        state = chopped[i].Split(':')[1].TrimStart('"').TrimEnd('"');
                    }
                    else if (chopped[i].Contains("pattern\""))
                    {
                        if(chopped[i].Contains("stateDescription\""))
                            pattern = chopped[i].Split(':')[2].TrimStart('"').TrimEnd('"');
                    }
                }
                ItemsDict.Add(name, new SensorValues(link, state, pattern, readOnly, options));
            }
        }

        public void updateItemsDict(List<string> list)
        {
            foreach (var element in list)
            {
                string name = null;
                string state = null;
                string[] chopped = element.Split(new char[] { ',', '/' },
                            StringSplitOptions.RemoveEmptyEntries);
                name = chopped[4].TrimEnd('"');
                for (int i = 5; i < chopped.Length; i++)
                {
                    if (chopped[i].Contains("state\""))
                    {
                        state = chopped[i].Split(':')[1].TrimStart('"').TrimEnd('"');
                    }
                }
                if (ItemsDict[name].GetState() != state)
                    ItemsDict[name].SetState(state);
            }
        }
    }
}
