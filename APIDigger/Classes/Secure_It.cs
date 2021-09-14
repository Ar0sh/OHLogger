using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHDataLogger.Classes
{
    class Secure_It
    {
        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = ASCIIEncoding.ASCII.GetString(b);
            }
            catch
            {
                decrypted = "";
            }
            return decrypted;
        }

        public string EncryptString(string strEncrypted)
        {
            byte[] b = ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
    }
}
