using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;

namespace STXtoSQL_Sales_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            string date1 = "";
            string date2 = "";

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
                Console.WriteLine(ex.Message.ToString());
            }

            // Testing
            Console.WriteLine(date1 + " / " + date2);

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
                Console.WriteLine(ex.Message.ToString());
            }

            // Testing
            Console.WriteLine("Retrieve records: " + lstSales.Count.ToString());

            //foreach (Sales b in lstSales)
            //{
            //    Console.WriteLine(b.qlf + " / " + b.brh + " / " + b.pep + " / " + b.wgt + " / " + b.val + " / " + b.avg_val + " / " + b.inv_dt + " / " + b.mn.ToString() + " / " + b.dy.ToString() + " / " + b.yr.ToString());
            //}
            #endregion

            #region ToSQL
            int rowCnt = 0;

            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstSales.Count != 0)
            {
                // Put lstSales into IMPORT Sales table
                try
                {
                    rowCnt = objSQL.Write_Sales_to_IMPORT(lstSales);
                    Console.WriteLine("IMPORT inserted: " + rowCnt.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }

                // Call SP to put IMPORT Sales into Sales table
                try
                {
                    rowCnt = objSQL.Write_IMPORT_to_Sales(date1, date2);
                    Console.WriteLine("Sales inserted: " + rowCnt.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }        
            #endregion

            // Testing
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
