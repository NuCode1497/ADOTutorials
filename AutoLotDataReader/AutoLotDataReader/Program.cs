using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AutoLotDataReader
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("**** Fun with Data Readers. ****\n");

            //Create a connection string with a builder
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "AutoLot",
                DataSource = @"CODY-PC\NUCODE",
                ConnectTimeout = 30,
                IntegratedSecurity = true
            };

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = cnStringBuilder.ConnectionString;
                connection.Open();
                ShowConnectionStatus(connection);

                //Create a SQL command object
                string sql = "Select * From Inventory;Select * From Customers";
                SqlCommand myCommand = new SqlCommand(sql, connection);

                //Obtain a data reader
                using (SqlDataReader myDataReader = myCommand.ExecuteReader())
                {
                    do
                    {

                        while (myDataReader.Read())
                        {
                            for (int i = 0; i < myDataReader.FieldCount; i++)
                            {
                                WriteLine($"{myDataReader.GetName(i)} = {myDataReader.GetValue(i)}");
                            }
                            WriteLine();
                        }
                    } while (myDataReader.NextResult());
                }
            }
            ReadLine();
        }

        static void ShowConnectionStatus(SqlConnection connection)
        {
            WriteLine("*** Info about your connection ***");
            WriteLine($"Database location: {connection.DataSource}");
            WriteLine($"Database name: {connection.Database}");
            WriteLine($"Timeout: {connection.ConnectionTimeout}");
            WriteLine($"Connection state: {connection.State}\n");
        }
    }
}
