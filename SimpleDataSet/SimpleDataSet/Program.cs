using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleDataSet
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Fun with DataSets *****\n");

            var carsInventoryDS = new DataSet("Car Inventory");
            carsInventoryDS.ExtendedProperties["TimeStamp"] = DateTime.Now;
            carsInventoryDS.ExtendedProperties["DataSetID"] = Guid.NewGuid();
            carsInventoryDS.ExtendedProperties["Company"] = "Mikko's Hot Tub Super Store";
            FillDataSet(carsInventoryDS);
            PrintDataSet(carsInventoryDS);

            //Test dataset row state
            //ManipulateDataRowState();

            WriteLine("\nCheck out carsDataSet.xml in folder!");
            SaveAndLoadAsXml(carsInventoryDS);
            WriteLine("\nCheck out BinaryCars.bin in folder!");
            SaveAndLoadAsBinary(carsInventoryDS);

            ReadLine();
        }

        static void FillDataSet(DataSet ds)
        {
            //Create data columns that map to the columns in the Inventory table of the AutoLot db
            var carIDColumn = new DataColumn("CarID", typeof(int))
            {
                Caption = "Car ID",
                ReadOnly = true,
                AllowDBNull = false,
                Unique = true,
                AutoIncrement = true,
                AutoIncrementSeed = 1,
                AutoIncrementStep = 1
            };

            var carMakeColumn = new DataColumn("Make", typeof(string));
            var carColorColumn = new DataColumn("Color", typeof(string));
            var carPetNameColumn = new DataColumn("PetName", typeof(string))
            { Caption = "Pet Name" };

            //Add columns to DataTable
            var inventoryTable = new DataTable("Inventory");
            inventoryTable.Columns.AddRange(new[] { carIDColumn, carMakeColumn, carColorColumn, carPetNameColumn });

            //Add some rows to the Inventory Table
            DataRow carRow = inventoryTable.NewRow();
            carRow["Make"] = "BMW";
            carRow["Color"] = "Black";
            carRow["PetName"] = "Hamlet";
            inventoryTable.Rows.Add(carRow);

            carRow = inventoryTable.NewRow();
            //carRow[0] is the ID
            carRow[1] = "Saab";
            carRow[2] = "Red";
            carRow[3] = "Sea Breeze";
            inventoryTable.Rows.Add(carRow);

            //Mark the primary key of this table
            inventoryTable.PrimaryKey = new[] { inventoryTable.Columns[0] };
            //Finally, add our table to the DataSet
            ds.Tables.Add(inventoryTable);
        }

        static void PrintDataSet(DataSet ds)
        {
            //Print out the DataSet name and any extended properties
            WriteLine($"DataSet is named: {ds.DataSetName}");
            foreach(DictionaryEntry de in ds.ExtendedProperties)
            {
                WriteLine($"Key = {de.Key}, Value = {de.Value}");
            }
            WriteLine();
            //Print out each table using rows and columns
            foreach(DataTable dt in ds.Tables)
            {
                WriteLine($"=> {dt.TableName} Table:");

                //print out the column names
                for(var curCol = 0; curCol < dt.Columns.Count; curCol++)
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
            while(dtReader.Read())
            {
                for (var i = 0; i < dtReader.FieldCount; i++)
                {
                    Write($"{dtReader.GetValue(i).ToString().Trim()}\t");
                }
                WriteLine();
            }
            dtReader.Close();
        }

        private static void ManipulateDataRowState()
        {
            //Create a temp DataTable for testing.
            var temp = new DataTable("Temp");
            temp.Columns.Add(new DataColumn("TempColumn", typeof(int)));

            //RowState = Detatched
            var row = temp.NewRow();
            WriteLine($"After calling NewRow(): {row.RowState}");

            //RowState = Added
            temp.Rows.Add(row);
            WriteLine($"After first assignment: {row.RowState}");

            //RowState = Unchanged
            temp.AcceptChanges();
            WriteLine($"After calling AcceptChanges: {row.RowState}");

            //RowState = Modified
            row["TempColumn"] = 11;
            WriteLine($"After first assignment: {row.RowState}");

            //RowState = Deleted
            temp.Rows[0].Delete();
            WriteLine($"After calling Delete: {row.RowState}");

        }

        static void SaveAndLoadAsXml(DataSet carsInventoryDS)
        {
            //Save this DataSet as XML.
            carsInventoryDS.WriteXml("carsDataSet.xml");
            carsInventoryDS.WriteXmlSchema("carsDataSet.xsd");

            //Clear out DataSet
            carsInventoryDS.Clear();

            //Load DataSet from XML file
            carsInventoryDS.ReadXml("carsDataSet.xml");
        }

        static void SaveAndLoadAsBinary(DataSet carsInventoryDS)
        {
            carsInventoryDS.RemotingFormat = SerializationFormat.Binary;

            var fs = new FileStream("BinaryCars.bin", FileMode.Create);
            var bFormat = new BinaryFormatter();
            bFormat.Serialize(fs, carsInventoryDS);
            fs.Close();

            carsInventoryDS.Clear();

            fs = new FileStream("BinaryCars.bin", FileMode.Open);
            var data = (DataSet)bFormat.Deserialize(fs);
        }
    }
}
