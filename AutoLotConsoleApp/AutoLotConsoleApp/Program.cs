using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotConsoleApp.EF;
using static System.Console;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace AutoLotConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Fun with ADO.NET EF *****\n");
            int carId = AddNewRecord();
            WriteLine(carId);
            UpdateRecord(carId);
            //RemoveRecord(carId);
            RemoveRecordUsingEntityState(carId); //more efficient
            //PrintAllInventory();
            FunWithLinqQueries();
            ReadLine();
        }

        private static int AddNewRecord()
        {
            //Add a record to the Inventory table of the AutoLot database
            using (var context = new AutoLotEntities())
            {
                try
                {
                    //Hard-code data fro a new record, for testing
                    var car = new Car() { Make = "Yugo", Color = "Brown", CarNickName = "Brownie" };
                    context.Cars.Add(car);
                    context.SaveChanges();
                    //On a successful save, EF populates the db geneated identity field
                    return car.CarId;
                }
                catch(Exception ex)
                {
                    WriteLine(ex.InnerException.Message);
                    return 0;
                }
            }
        }
        private static void RemoveRecord(int carId)
        {
            //find a car to delete by primary key
            using (var context = new AutoLotEntities())
            {
                //see if we have it
                Car carToDelete = context.Cars.Find(carId);
                if(carToDelete != null)
                {
                    context.Cars.Remove(carToDelete);
                    context.SaveChanges();
                }
            }
        }

        private static void RemoveRecordUsingEntityState(int carId)
        {
            using (var context = new AutoLotEntities())
            {
                Car carToDelete = new Car() { CarId = carId };
                context.Entry(carToDelete).State = EntityState.Deleted;
                try
                {
                    context.SaveChanges();
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    WriteLine(ex);
                }
            }
        }
        private static void UpdateRecord(int carId)
        {
            //find a car to delete by primary key
            using (var context = new AutoLotEntities())
            {
                //grab the car, change it, save!
                Car carToUpdate = context.Cars.Find(carId);
                if(carToUpdate != null)
                {
                    WriteLine(context.Entry(carToUpdate).State);
                    carToUpdate.Color = "Blue";
                    WriteLine(context.Entry(carToUpdate).State);
                    context.SaveChanges();
                }
            }
        }
        private static void PrintAllInventory()
        {
            //Select all items from the Inventory table of AutoLot and print out the data using
            //our custom ToString() of the Car entity class
            using (var context = new AutoLotEntities())
            {
                //foreach(Car c in context.Cars)
                //{
                //    WriteLine(c);
                //}

                //foreach(Car c in context.Cars.SqlQuery("Select CarId,Make,Color,PetName as CarNickName from Inventory where Make=@p0","BMW"))
                //{
                //    WriteLine(c);
                //}

                foreach(Car c in context.Cars.Where(c => c.Make == "BMW"))
                {
                    WriteLine(c);
                }
            }
        }

        private static void FunWithLinqQueries()
        {
            //using (var context = new AutoLotEntities())
            //{
            //    //Get a projection of new data
            //    var colorsMakes = from item in context.Cars select new { item.Color, item.Make };
            //    foreach(var item in colorsMakes)
            //    {
            //        WriteLine(item);
            //    }

            //    //Get only items where Color == "Black"
            //    var blackCars = from item in context.Cars where item.Color == "Black" select item;
            //    foreach(var item in blackCars)
            //    {
            //        WriteLine(item);
            //    }
            //}

            //instead of hitting the db every time we use LINQ, get the data first then use LINQ on disconnected data
            using (var context = new AutoLotEntities())
            {
                //Get all data from the Inventory table
                //Could also write:
                //var allData = (from item in context.Cars select item).ToArray();
                var allData = context.Cars.ToArray();

                //Get a projection of new data
                var colorsMakes = from item in allData select new { item.Color, item.Make };
                foreach(var item in colorsMakes)
                {
                    WriteLine(item);
                }

                //Get only items where Color == "Black"
                var blackCars = from item in context.Cars where item.Color == "Black" select item;
                foreach (var item in blackCars)
                {
                    WriteLine(item);
                }
            }
        }
    }
}
