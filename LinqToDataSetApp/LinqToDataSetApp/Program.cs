using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotDAL.DataSets;
using AutoLotDAL.DataSets.AutoLotDataSetTableAdapters;
using static System.Console;
using System.Data;

namespace LinqToDataSetApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** LINQ over DataSet ******\n");

            //Get a strongly typed DataTable containing the current Inventory of the AutoLot database
            AutoLotDataSet dal = new AutoLotDataSet();
            InventoryTableAdapter tableAdapter = new InventoryTableAdapter();
            AutoLotDataSet.InventoryDataTable data = tableAdapter.GetData();

            //Methods
            PrintAllCarIDs(data);
            ShowBlackCars(data);
            BuildDataTableFromQuery(data);

            ReadLine();
        }
        static void PrintAllCarIDs(DataTable data)
        {
            WriteLine("\n*** These are all the Car IDs ***");
            EnumerableRowCollection enumData = data.AsEnumerable();
            foreach (DataRow r in enumData)
            {
                WriteLine($"Car ID = {r["CarID"]}");
            }
        }
        static void ShowBlackCars(DataTable data)
        {
            //Project a new result set containing the ID/color for rows of red cars
            var cars = from car in data.AsEnumerable()
                       where car.Field<string>("Color").Trim() == "Black"
                       select new
                       {
                           ID = car.Field<int>("CarID"), //can use Field<T> instead of (int)car["CarID"]
                           Make = car.Field<string>("Make")
                       };
            WriteLine("Here are the black cars we have in stock:");
            foreach(var item in cars)
            {
                WriteLine($"-> CarID = {item.ID} is {item.Make}");
            }
        }
        static void BuildDataTableFromQuery(DataTable data)
        {
            WriteLine("*** Data Table from LINQ Query ***");
            var cars = from car in data.AsEnumerable()
                       where car.Field<int>("CarID") > 5
                       select car;
            //Use this result set to build a new DataTable
            DataTable newTable = cars.CopyToDataTable();

            //Print
            for(int i=0;i<newTable.Rows.Count;i++)
            {
                for(int j=0;j<newTable.Columns.Count;j++)
                {
                    Write(newTable.Rows[i][j].ToString().Trim() + "\t");
                }
                WriteLine();
            }
        }
    }
}
