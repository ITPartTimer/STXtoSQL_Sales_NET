﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class SQLData : Helpers
    {
        public int Write_Sales_to_IMPORT(List<Sales> lstSales)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from SCORE_IMPORT_tbl_Sales";

                    cmd.ExecuteNonQuery();
                }
                catch(Exception)
                {
                    throw;
                }

                try
                {
                    // Change Text to Insert data into IMPORT
                    cmd.CommandText = "INSERT INTO SCORE_IMPORT_tbl_Sales (qlf,brh,pep,wgt,val,ave_val,inv_dt,actvy_mn,actvy_dy,actvy_yr) " +
                                        "VALUES (@qlf,@brh,@pep,@wgt,@val,@ave_val,@inv_dt,@actvy_mn,@actvy_dy,@actvy_yr)";

                    cmd.Parameters.Add("@qlf", SqlDbType.Char);
                    cmd.Parameters.Add("@brh", SqlDbType.VarChar);
                    cmd.Parameters.Add("@pep", SqlDbType.VarChar);
                    cmd.Parameters.Add("@wgt", SqlDbType.Decimal);
                    cmd.Parameters.Add("@val", SqlDbType.Decimal);
                    cmd.Parameters.Add("@ave_val", SqlDbType.Decimal);
                    cmd.Parameters.Add("@inv_dt", SqlDbType.DateTime);
                    cmd.Parameters.Add("@actvy_mn", SqlDbType.Int);
                    cmd.Parameters.Add("@actvy_dy", SqlDbType.Int);
                    cmd.Parameters.Add("@actvy_yr", SqlDbType.Int);

                    foreach (Sales s in lstSales)
                    {

                        cmd.Parameters[0].Value = s.qlf;
                        cmd.Parameters[1].Value = s.brh;
                        cmd.Parameters[2].Value = s.pep;
                        cmd.Parameters[3].Value = Convert.ToDecimal(s.wgt);
                        cmd.Parameters[4].Value = Convert.ToDecimal(s.val);
                        cmd.Parameters[5].Value = Convert.ToDecimal(s.avg_val);
                        cmd.Parameters[6].Value = Convert.ToDateTime(s.inv_dt);
                        cmd.Parameters[7].Value = s.mn;
                        cmd.Parameters[8].Value = s.dy;
                        cmd.Parameters[9].Value = s.yr;

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't leave a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(qlf) from SCORE_IMPORT_tbl_Sales";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch(Exception)
                {
                    throw;
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        public int Write_IMPORT_to_Sales(string date1, string date2)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to Sales table.  Return rows inserted.
                cmd.CommandText ="SCORE_proc_IMPORT_to_Sales";

                AddParamToSQLCmd(cmd, "@date1", SqlDbType.DateTime, 4, ParameterDirection.Input, date1);
                AddParamToSQLCmd(cmd, "@date2", SqlDbType.DateTime, 4, ParameterDirection.Input, date2);
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

    }
}
