using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APIDigger.Methods
{
    class Functions
    {
        public static bool IsTextAllowed(string Text, string AllowedRegex)
        {
            try
            {
                var regex = new Regex(AllowedRegex);
                return !regex.IsMatch(Text);
            }
            catch
            {
                return true;
            }
        }
        public static void SaveUpdateInterval(string input)
        {
            Properties.Settings.Default.UpdateInterval = Convert.ToInt32(input);
            Properties.Settings.Default.Save();
        }
    }
}
