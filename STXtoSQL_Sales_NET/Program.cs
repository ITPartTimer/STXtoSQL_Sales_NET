using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;
using STXtoSQL.Log;

namespace STXtoSQL_Sales_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogWrite("MSG", "Start: " + DateTime.Now.ToString());

            // Args will change based on STXtoSQL program goal
            string date1 = "";
            string date2 = "";
            int odbcCnt = 0;
            int insertCnt = 0;
            int importCnt = 0;

            try
            {
                if (args.Length > 0)
                {
                    /*
                     * Must be in format mm/dd/yyyy.  No time part
                     */
                    date1 = args[0].ToString();
                    date2 = args[1].ToString();
                }
                else
                {
                    // No args = current month to yesterday
                    DateTime dtToday = DateTime.Today;

                    DateTime dtFirst = new DateTime(dtToday.Year, dtToday.Month, 1);

                    /*
                     * Need one date part of datetime.
                     * Time and date are separated by a space, so split the string
                     * and only use the 1st element.
                     */
                    string[] date1Split = dtFirst.ToString().Split(' ');
                    string[] date2Split = dtToday.AddDays(-1).ToString().Split(' ');

                    date1 = date1Split[0];
                    date2 = date2Split[0];
                }
            }
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
                //Console.WriteLine(ex.Message.ToString());
            }

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<Sales> lstSales = new List<Sales>();

            // Get Sales from Stratix and put in List<Sales>
            try
            {
                lstSales = objODBC.Get_Sales(date1, date2); 
            }           
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                //Console.WriteLine(ex.Message.ToString());
            }
            #endregion

            #region ToSQL
            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstSales.Count != 0)
            {
                odbcCnt = lstSales.Count;

                // Put lstSales into IMPORT Sales table
                try
                {
                    importCnt = objSQL.Write_Sales_to_IMPORT(lstSales);

                    //Console.WriteLine("IMPORT inserted: " + importCnt.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    //Console.WriteLine(ex.Message.ToString());
                }

                // Call SP to put IMPORT Sales into Sales table
                try
                {
                    insertCnt = objSQL.Write_IMPORT_to_Sales(date1, date2);

                    //Console.WriteLine("Sales inserted: " + salesCnt.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    //Console.WriteLine(ex.Message.ToString());
                }

                Logger.LogWrite("MSG", "Range=" + date1 + ":" + date2 + " ODBC/IMPORT/SALES=" + odbcCnt.ToString() + ":" + importCnt.ToString() + ":" + insertCnt.ToString());
            }
            else
                Logger.LogWrite("MSG", "No data");

            Logger.LogWrite("MSG", "End: " + DateTime.Now.ToString());
            #endregion

            // Testing
            //Console.WriteLine("Press key to exit");
            //Console.ReadKey();
        }
    }
}
