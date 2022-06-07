using APIDigger.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace APIDigger.SQL
{
    class Sql_Methods : OpenHABRest
    {

        void StoreSqlCall()
        {
            while (SqlStoreTask.IsAlive)
            {
                if (ItemsList.Count > 0 && !_resetSqlInfo)
                {
                    dataSqlClasses.StoreValuesToSql();
                    statSqlCon.Dispatcher.Invoke(() =>
                    {
                        UpdateGui(true, false, true);
                    });
                }
                else
                {
                    if (!_apiloggedIn)
                    {
                        SqlErrMessage = "No API Data";
                        SqlErrColor = Brushes.Orange;
                        _resetSqlInfo = true;
                    }
                    else if (_resetSqlInfo)
                    {
                        SqlMessages = "SQL Connected";
                        SqlColor = Brushes.Green;
                        _resetSqlInfo = false;
                    }
                    statSqlCon.Dispatcher.Invoke(() =>
                    {
                        UpdateGui(true, false, true);
                    });
                }
                Thread.Sleep(9400);
            }
        }


        public void RunSql()
        {
            foreach (Items item in ItemsList)
            {
                if (!dataSqlClasses.Tables.Contains(item.name))
                {
                    dataSqlClasses.CreateTables(item.name, item.type);
                }
            }
            if (SqlTabMessage != "")
            {
                statSqlTab.Text = SqlTabMessage;
                statSqlTabItem.Visibility = Visibility.Visible;
            }
            SqlStoreTask = new Thread(StoreSqlCall)
            {
                IsBackground = true
            };
            SqlStoreTask.Start();
        }
    }
}
