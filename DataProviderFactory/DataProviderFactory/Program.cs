using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;

namespace DataProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("*** Fun with Data Provider Factories ***\n");
            //Get stuff from config
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.ConnectionStrings["AutoLotSqlProvider"].ConnectionString;
            //Get the factory provider.
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
            //Get connection object
            using (DbConnection connection = factory.CreateConnection())
            {
                if(connection == null)
                {
                    ShowError("Connection");
                    return;
                }
                WriteLine($"Your connection object is a: {connection.GetType().Name}");
                connection.ConnectionString = connectionString;
                connection.Open();

                //Make command object
                DbCommand command = factory.CreateCommand();
                if(command == null)
                {
                    ShowError("Command");
                    return;
                }
                WriteLine($"Your command object is a: {command.GetType().Name}");
                command.Connection = connection;
                command.CommandText = "Select * From Inventory";

                //Print out data with data reader
                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    WriteLine($"Your data reader object is a: {dataReader.GetType().Name}");
                    WriteLine("\n*** Current Inventory ***");
                    while(dataReader.Read())
                    {
                        WriteLine($"-> Car #{dataReader["CarId"]} is a {dataReader["Make"]}.");
                    }
                }
                ReadLine();
            }
        }
        private static void ShowError(string objectName)
        {
            WriteLine($"There was an issue creating the {objectName}");
            ReadLine();
        }
    }
}
