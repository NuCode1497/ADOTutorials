using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoLotDAL2.EF;
using AutoLotDAL2.Models;
using AutoLotDAL2.Repos;
using System.Data.Entity;
using static System.Console;
using System.Data.Entity.Infrastructure;

namespace AutoLotTestDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database.SetInitializer(new DataInitializer()); //Drops and recreates the database every time the program starts
            WriteLine("***** Fun with ADO.NET EF Code First *****\n");
            //var car1 = new Car() { Make = "Yugo", Color = "Brown", CarNickName = "Brownie" };
            //var car2 = new Car() { Make = "SmartCar", Color = "Brown", CarNickName = "Shorty" };
            //AddNewRecord(car1);
            //AddNewRecord(car2);
            //AddNewRecords(new List<Car> { car1, car2 });
            //UpdateRecord(car1.CarId);
            PrintAllInventory();
            ShowAllOrders();
            //ShowAllOrdersEagerlyFetched();
            //WriteLine("*******Testing making a customer a credit risk********");
            //PrintAllCustomersAndCreditRisks();
            //var customerRepo = new CustomerRepo();
            //var customer = customerRepo.GetOne(4);
            //customerRepo.Context.Entry(customer).State = EntityState.Detached;
            //var risk = MakeCustomerARisk(customer);
            //PrintAllCustomersAndCreditRisks();
            //UpdateRecordWithConcurrency();
            ReadLine();
        }
        private static void PrintAllInventory()
        {
            using (var repo = new CarRepo())
            {
                foreach (Car c in repo.GetAll())
                {
                    WriteLine(c);
                }
            }
        }
        private static void AddNewRecord(Car car)
        {
            //Add record to the Inventory table of the AutoLot db
            using (var repo = new CarRepo())
            {
                repo.Add(car);
            }
        }
        private static void AddNewRecords(IList<Car> cars)
        {
            //Add record to the Inventory table of the AutoLot db
            using (var repo = new CarRepo())
            {
                repo.AddRange(cars);
            }
        }
        private static void RemoveRecordById(int carId, byte[] timeStamp)
        {
            using (var repo = new CarRepo())
            {
                repo.Delete(carId, timeStamp);
            }
        }
        private static void UpdateRecord(int carId)
        {
            using (var repo = new CarRepo())
            {
                //Grab the car, change it, save!
                var carToUpdate = repo.GetOne(carId);
                if(carToUpdate != null)
                {
                    WriteLine("Before change: " + repo.Context.Entry(carToUpdate).State);
                    carToUpdate.Color = "Blue";
                    WriteLine("After change: " + repo.Context.Entry(carToUpdate).State);
                    repo.Save(carToUpdate);
                    WriteLine("After save: " + repo.Context.Entry(carToUpdate).State);
                }
            }
        }
        private static void ShowAllOrders()
        {
            using (var repo = new OrderRepo())
            {
                WriteLine("******** Pending Orders ***********");
                //Fetch stuff from Db using default lazy method
                foreach(var item in repo.GetAll())
                {
                    WriteLine($"->{item.Customer.FullName} is waiting on {item.Car.CarNickName}");
                }
            }
        }
        private static void ShowAllOrdersEagerlyFetched()
        {
            using (var context = new AutoLotEntities())
            {
                WriteLine("************ Pending Orders ***************");
                //Fetch stuff from Db using eager method
                var orders = context.Orders
                    .Include(x => x.Customer)
                    .Include(x => x.Car)
                    .ToList();
                foreach(var item in orders)
                {
                    WriteLine($"->{item.Customer.FullName} is waiting on {item.Car.CarNickName}");
                }
            }
        }
        private static CreditRisk MakeCustomerARisk(Customer customer)
        {
            using (var context = new AutoLotEntities())
            {
                context.Customers.Attach(customer);
                context.Customers.Remove(customer);
                var creditRisk = new CreditRisk()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };
                context.CreditRisks.Add(creditRisk);
                var creditRiskDupe = new CreditRisk()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };
                context.CreditRisks.Add(creditRiskDupe);
                try
                {
                    context.SaveChanges();
                }
                catch(DbUpdateException ex)
                {
                    WriteLine(ex);
                }
                catch(Exception ex)
                {
                    WriteLine(ex);
                }
                return creditRisk;
            }
        }
        private static void PrintAllCustomersAndCreditRisks()
        {
            WriteLine("********** Customers ***********");
            using (var repo = new CustomerRepo())
            {
                foreach(var cust in repo.GetAll())
                {
                    WriteLine($"->{cust.FirstName} {cust.LastName} is a Customer.");
                }
            }
            WriteLine("********* Credit Risks **********");
            using (var repo = new CreditRiskRepo())
            {
                foreach (var cust in repo.GetAll())
                {
                    WriteLine($"->{cust.FirstName} {cust.LastName} is a Credit Risk!");
                }

            }
        }
        private static void UpdateRecordWithConcurrency()
        {
            //simulate two different users updating the same record
            var car = new Car() { Make = "Yugo", Color = "Brown", CarNickName = "Brownie" };
            AddNewRecord(car);
            var repo1 = new CarRepo();
            var car1 = repo1.GetOne(car.CarId);
            car1.CarNickName = "Updated";

            var repo2 = new CarRepo();
            var car2 = repo2.GetOne(car.CarId);
            car2.Make = "Nissan";

            repo1.Save(car1);
            try
            {
                repo2.Save(car2);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                WriteLine(ex);
            }
            //RemoveRecordById(car1.CarId, car1.Timestamp);
        }
    }
}
