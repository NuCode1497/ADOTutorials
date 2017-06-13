using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using static System.Console;
using System.Data.Common;

namespace FillDataSetUsingSqlDataAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Fun with Data Adapters *****\n");

            string connectionString = @"Data Source=CODY-PC\NUCODE;Initial Catalog=AutoLot;Integrated Security=True;Pooling=False";

            DataSet ds = new DataSet("AutoLot");

            SqlDataAdapter adapter = new SqlDataAdapter("Select * From Inventory", connectionString);

            DataTableMapping tableMapping = adapter.TableMappings.Add("Inventory", "Current Inventory");
            tableMapping.ColumnMappings.Add("CarId", "Car Id");
            tableMapping.ColumnMappings.Add("PetName", "Name of Car");
            

            adapter.Fill(ds, "Inventory");

            PrintDataSet(ds);
            ReadLine();
        }


        static void PrintDataSet(DataSet ds)
        {
            //Print out the DataSet name and any extended properties
            WriteLine($"DataSet is named: {ds.DataSetName}");
            foreach (DictionaryEntry de in ds.ExtendedProperties)
            {
                WriteLine($"Key = {de.Key}, Value = {de.Value}");
            }
            WriteLine();
            //Print out each table using rows and columns
            foreach (DataTable dt in ds.Tables)
            {
                WriteLine($"=> {dt.TableName} Table:");

                //print out the column names
                for (var curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Write($"{dt.Columns[curCol].ColumnName}\t");
                }
                WriteLine("\n----------------------------");

                ////Print the DataTable
                //for(var curRow = 0; curRow < dt.Rows.Count; curRow++)
                //{
                //    for (var curCol = 0; curCol < dt.Columns.Count; curCol++)
                //    {
                //        Write($"{dt.Rows[curRow][curCol]}\t");
                //    }
                //    WriteLine();
                //}

                //Print the DataTable using DataTableReader
                //This is similar to using DataReaders in connected layer
                PrintTable(dt);
            }
        }

        static void PrintTable(DataTable dt)
        {
            //Get the DataTableReader type
            DataTableReader dtReader = dt.CreateDataReader();

            //The DataTableReader works just like the DataReader
            while (dtReader.Read())
            {
                for (var i = 0; i < dtReader.FieldCount; i++)
                {
                    Write($"{dtReader.GetValue(i).ToString().Trim()}\t");
                }
                WriteLine();
            }
            dtReader.Close();
        }
    }
}
