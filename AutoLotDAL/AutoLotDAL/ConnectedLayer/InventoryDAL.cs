using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using AutoLotDAL.Models;

namespace AutoLotDAL.ConnectedLayer
{
    public class InventoryDAL
    {
        private SqlConnection _sqlConnection = null;

        public void OpenConnection(string connectionString)
        {
            _sqlConnection = new SqlConnection { ConnectionString = connectionString };
            _sqlConnection.Open();
        }

        public void CloseConnection()
        {
            _sqlConnection.Close();
        }

        public void InsertAuto(int id, string color, string make, string petName)
        {
            string sql = "Insert Into Inventory" +
                $"(Make, Color, PetName) Values (@Make, @Color, @PetName')";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@Make",
                    Value = make,
                    SqlDbType = SqlDbType.Char,
                    Size = 10
                };
                command.Parameters.Add(parameter);
                parameter = new SqlParameter
                {
                    ParameterName = "@Color",
                    Value = color,
                    SqlDbType = SqlDbType.Char,
                    Size = 10
                };
                command.Parameters.Add(parameter);
                parameter = new SqlParameter
                {
                    ParameterName = "@PetName",
                    Value = petName,
                    SqlDbType = SqlDbType.Char,
                    Size = 10
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
        public void InsertAuto(NewCar car)
        {
            string sql = "Insert Into Inventory" + "(Make, ConsoleColor, PetName) Values" + $"('{car.Make}', '{car.Color}')";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void DeleteCar(int id)
        {
            string sql = $"Delete from Inventory where CarId = '{id}'";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    Exception error = new Exception("Sorry! That car is on order!", ex);
                    throw error;
                }
            }
        }

        public void UpdateCarPetName(int id, string newPetName)
        {
            string sql = $"Update Inventory Set PetName = '{newPetName}' Where CarId = '{id}'";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public string LookUpPetName(int carID)
        {
            string carPetName = string.Empty;

            using (SqlCommand command = new SqlCommand("GetPetName", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@CarId",
                    SqlDbType = SqlDbType.Int,
                    Value = carID,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);
                param = new SqlParameter
                {
                    ParameterName = "@PetName",
                    SqlDbType = SqlDbType.Char,
                    Size = 10,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                carPetName = (string)command.Parameters["@PetName"].Value;
            }
            return carPetName;
        }

        public List<NewCar> GetAllInventoryAsList()
        {
            List<NewCar> inv = new List<NewCar>();

            string sql = "Select * From Inventory";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();
                while(dataReader.Read())
                {
                    inv.Add(new NewCar
                    {
                        CarId = (int)dataReader["CarId"],
                        Color = (string)dataReader["Color"],
                        Make = (string)dataReader["Make"],
                        PetName = (string)dataReader["PetName"]
                    });
                }
                return inv;
            }
        }

        public DataTable GetAllInventoryAsDataTable()
        {
            DataTable dataTable = new DataTable();
            string sql = "Select * From Inventory";
            using (SqlCommand cmd = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = cmd.ExecuteReader();
                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }
    }
}
