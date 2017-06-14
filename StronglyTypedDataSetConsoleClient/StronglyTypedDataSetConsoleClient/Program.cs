using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using AutoLotDAL.DataSets;
using AutoLotDAL.DataSets.AutoLotDataSetTableAdapters;

namespace StronglyTypedDataSetConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Fun with Strongly Typed DataSets *****\n");

            //Caller creates the Dataset object
            var table = new AutoLotDataSet.InventoryDataTable();

            //Inform adapter of the Select command text and connection
            var adapter = new InventoryTableAdapter();

            AddRecords(table, adapter);
            table.Clear();
            adapter.Fill(table);
            PrintInventory(table);

            ReadLine();
        }

        static void PrintInventory(AutoLotDataSet.InventoryDataTable dt)
        {
            for(int curCol = 0; curCol < dt.Columns.Count; curCol++)
            {
                Write(dt.Columns[curCol].ColumnName + "\t");
            }
            WriteLine("\n-----------------------------");

            for(int curRow = 0; curRow< dt.Rows.Count; curRow++)
            {
                for(int curCol=0;curCol<dt.Columns.Count;curCol++)
                {
                    Write(dt.Rows[curRow][curCol] + "\t");
                }
                WriteLine();
            }
        }

        public static void AddRecords(AutoLotDataSet.InventoryDataTable table, InventoryTableAdapter adapter)
        {
            try
            {
                //Get a new strongly typed row from the table
                AutoLotDataSet.InventoryRow newRow = table.NewInventoryRow();

                //Fill row with some sample data
                newRow.Color = "Purple";
                newRow.Make = "BMW";
                newRow.PetName = "Saku";

                //Insert the new row
                table.AddInventoryRow(newRow);

                //Add one more row, using overloaded Add method
                table.AddInventoryRow("Yugo", "Green", "Zippy");

                adapter.Update(table);

            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
            }
        }

        private static void RemoveRecords(AutoLotDataSet.InventoryDataTable table, InventoryTableAdapter adapter)
        {
            try
            {
                Write("Enter ID of car to delete: ");
                string carID = ReadLine() ?? "0";
                AutoLotDataSet.InventoryRow rowToDelete = table.FindByCarId(int.Parse(carID));
                adapter.Delete(rowToDelete.CarId, rowToDelete.Make, rowToDelete.Color, rowToDelete.PetName);
            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
            }
        }

        public static void CallStoredProc()
        {
            try
            {
                var queriesTableAdapter = new QueriesTableAdapter();
                Write("Enter ID of car to look up: ");
                string carID = ReadLine() ?? "0";
                string carName = "";
                queriesTableAdapter.GetPetName(int.Parse(carID), ref carName);
                WriteLine($"CarID {carID} has the name of {carName}");
            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
            }
        }
    }
}
