using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using AutoLotDAL.ConnectedLayer;
using AutoLotDAL.Models;

namespace AdoNetTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("***** Simple Transaction Example *****\n");

            //Simple way to allow the tx to succeed or not
            bool throwEx = true;

            Write("Do you want to throw an exception (Y or N): ");
            var userAnswer = ReadLine();
            if(userAnswer?.ToLower() == "n")
            {
                throwEx = false;
            }

            var dal = new InventoryDAL();
            dal.OpenConnection(@"Data Source=CODY-PC\NUCODE;Initial Catalog=AutoLot;Integrated Security=True;Pooling=False");

            // Process customer 5 - enter the id for Homer Simpsoon in the next line
            dal.ProcessCreditRisk(throwEx, 5);
            WriteLine("Check CreditRisk table for results");
            ReadLine();
        }
    }
}
