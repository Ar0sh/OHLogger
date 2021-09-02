using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHDataLogger.Methods
{
    public class Logger
    {
        readonly static string LOG_FILE = Directory.GetCurrentDirectory() + "/LogFile/LogFile.txt";
        readonly static int MaxRolledLogCount = 3;
        readonly static int MaxLogSize = 1024*1024*32; // 1 * 1024 * 1024; <- small value for testing that it works, you can try yourself, and then use a reasonable size, like 1M-10M

        public static void LogMessage(string msg, ErrorLevel errorLevel)
        {
            lock (LOG_FILE) // lock is optional, but.. should this ever be called by multiple threads, it is safer
            {
                RollLogFile(LOG_FILE);
                File.AppendAllText(LOG_FILE, "[" + DateTime.Now + "]" + "[" + errorLevel + "]" + msg + Environment.NewLine, Encoding.UTF8);
            }
        }

        private static void RollLogFile(string logFilePath)
        {
            try
            {
                var length = new FileInfo(logFilePath).Length;

                if (length > MaxLogSize)
                {
                    var path = Path.GetDirectoryName(logFilePath);
                    var wildLogName = Path.GetFileNameWithoutExtension(logFilePath) + "*" + Path.GetExtension(logFilePath);
                    var bareLogFilePath = Path.Combine(path, Path.GetFileNameWithoutExtension(logFilePath));
                    string[] logFileList = Directory.GetFiles(path, wildLogName, SearchOption.TopDirectoryOnly);
                    if (logFileList.Length > 0)
                    {
                        // only take files like logfilename.log and logfilename.0.log, so there also can be a maximum of 10 additional rolled files (0..9)
                        var rolledLogFileList = logFileList.Where(fileName => fileName.Length == (logFilePath.Length + 2)).ToArray();
                        Array.Sort(rolledLogFileList, 0, rolledLogFileList.Length);
                        if (rolledLogFileList.Length >= MaxRolledLogCount)
                        {
                            File.Delete(rolledLogFileList[MaxRolledLogCount - 1]);
                            var list = rolledLogFileList.ToList();
                            list.RemoveAt(MaxRolledLogCount - 1);
                            rolledLogFileList = list.ToArray();
                        }
                        // move remaining rolled files
                        for (int i = rolledLogFileList.Length; i > 0; --i)
                            File.Move(rolledLogFileList[i - 1], bareLogFilePath + "." + i + Path.GetExtension(logFilePath));
                        var targetPath = bareLogFilePath + ".0" + Path.GetExtension(logFilePath);
                        // move original file
                        File.Move(logFilePath, targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
    public enum ErrorLevel
    {
        WARNING, SQL, API, OTHER, SQLTABLE, LOGIN, THREAD
    }
}
