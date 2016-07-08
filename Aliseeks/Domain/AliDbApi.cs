using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using System.Data;
using MySql.Data.MySqlClient;

using Aliseeks.Models;

namespace Aliseeks.Domain
{
    public class AliDbApi
    {
        string username = "N/A";
        string password = "N/A";

    /*    public void InsertError(AliDBError error)
        {
            StackTrace trace = new StackTrace();
            string method = trace.GetFrame(1).GetMethod().Name;

            MySqlConnection conn = new MySqlConnection(connectionString());

            string storedProcedure = "Insert_Error";
            MySqlCommand command = new MySqlCommand(storedProcedure, conn);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@messagee", error.Message);
            command.Parameters.AddWithValue("@targetsourcee", method);

            try
            {
                conn.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }

            conn.Close();
        } */

        public int InsertSearchHistory(SearchCriteria item, int results)
        {
            MySqlConnection conn = new MySqlConnection(connectionString());

            string storedProcedure = "Insert_SearchHistory";
            MySqlCommand command = new MySqlCommand(storedProcedure, conn);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@searchh", item.SearchText);
            command.Parameters.AddWithValue("@resultss", results);

            Int32 retVal = 0;

            try
            {
                conn.Open();

                object val = command.ExecuteScalar();
                retVal = Convert.ToInt32(val);
            }
            catch (Exception e)
            {
                //AliDBError error = new AliDBError();
                //error.Message = e.Message;

                //InsertError(error);
            }

            conn.Close();

            return retVal;
        }

    /*    public void InsertSearchResults(SearchResult[] items)
        {
            MySqlConnection conn = new MySqlConnection(connectionString());

            string storedProcedure = "Insert_SearchResult";

            MySqlTransaction transaction = null;

            try
            {
                conn.Open();

                transaction = conn.BeginTransaction();

                foreach(SearchResult item in items)
                {
                    MySqlCommand command = new MySqlCommand(storedProcedure, conn, transaction);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@namee", item.Name);
                    command.Parameters.AddWithValue("@pricee", item.Price);
                    command.Parameters.AddWithValue("@quantityy", item.Quantity);
                    command.Parameters.AddWithValue("@urll", item.URL);
                    command.Parameters.AddWithValue("@munitt", item.MUnit);
                    command.Parameters.AddWithValue("@historyidd", item.HistoryID);
                    command.Parameters.AddWithValue("@unitt", item.Unit);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                if(transaction != null) { transaction.Rollback(); }

                //AliDBError error = new AliDBError();
                //error.Message = e.Message;

                //InsertError(error);
            }

            conn.Close();
        } */

        string connectionString()
        {
            string connectionString = "N/A";
            connectionString += "Uid=" + username + ";";
            connectionString += "Pwd=" + password + ";";

            return connectionString.ToString();
        }
    }
}
