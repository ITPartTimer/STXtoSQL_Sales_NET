using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<Sales> Get_Sales(string date1, string date2)
        {

            List<Sales> lstSales = new List<Sales>();

            //OdbcConnection conn = new OdbcConnection(ODBCDataConnString);
            OdbcConnection conn = new OdbcConnection("DSN=Invera;UID=livcalod;Pwd=livcalod");
            

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select stn_sls_qlf,stn_shpt_brh,prm_pep[1,2] as pep,stn_blg_wgt,stn_tot_val,stn_tot_avg_val,stn_inv_dt,
	                                MONTH(stn_inv_dt) as mn,DAY(stn_inv_dt) as dy,YEAR(stn_inv_dt) as yr
	                                from sahstn_rec inner join scrbrh_rec on stn_shpt_brh = brh_brh
	                                inner join inrprm_rec on stn_frm = prm_frm
	                                and stn_grd = prm_grd and stn_size = prm_size and stn_fnsh = prm_fnsh
	                                where stn_inv_dt >= '" + date1 + "' and stn_inv_dt <= '" + date2 + "'";       

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        Sales b = new Sales();

                        b.qlf = rdr["stn_sls_qlf"].ToString();
                        b.brh = rdr["stn_shpt_brh"].ToString();
                        b.pep = rdr["pep"].ToString();
                        b.wgt = rdr["stn_blg_wgt"].ToString();
                        b.val = rdr["stn_tot_val"].ToString();
                        b.avg_val = rdr["stn_tot_avg_val"].ToString();
                        b.inv_dt = rdr["stn_inv_dt"].ToString();
                        b.mn = Convert.ToInt16(rdr["mn"]);
                        b.dy = Convert.ToInt16(rdr["dy"]);
                        b.yr = Convert.ToInt16(rdr["yr"]);

                        lstSales.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
                //Console.WriteLine("MultDetail odbc ex: " + ex.Message);
            }
            catch (Exception)
            {
                throw;
                //Console.WriteLine("MultDetail other ex: " + ex.Message);
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return lstSales;
        }
    }
}
