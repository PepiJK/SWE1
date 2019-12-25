using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;

namespace TempPlugin
{
    public class TempController
    {
        private readonly string _dbConnectionString;

        public TempController(string connection = "Host=localhost;Username=swe;Password=123456;Database=webserver")
        {
            _dbConnectionString = connection;
        }

        public bool AddTemp(TempModel entity)
        {
            bool successfullyExecuted = false;
            string queryString = entity.Id != null
                ? "INSERT INTO temperatures (id, datetime, value) VALUES (@id, @datetime, @value);"
                : "INSERT INTO temperatures (datetime, value) VALUES (@datetime, @value);";


            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                
                // set id if entity id is provided
                if (entity.Id != null) command.Parameters.AddWithValue("id", entity.Id);
                command.Parameters.AddWithValue("datetime", entity.DateTime);
                command.Parameters.AddWithValue("value", entity.Value);
                
                // return true if command effected exactly one row and did not throw a postgresException
                try
                {
                    successfullyExecuted = command.ExecuteNonQuery() == 1;
                }
                catch (PostgresException)
                {
                    return false;
                }
            }

            return successfullyExecuted;
        }

        public bool RemoveTemp(int id)
        {
            string queryString = "DELETE FROM temperatures WHERE id = @id;";
            bool successfullyExecuted = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("id", id);

                // return true if command effected exactly one row
                successfullyExecuted = command.ExecuteNonQuery() == 1;
            }

            return successfullyExecuted;
        }

        public TempModel GetTemp(int id)
        {
            string queryString = "SELECT * FROM temperatures WHERE id = @id;";
            TempModel entity = new TempModel();

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("id", id);
                using NpgsqlDataReader result = command.ExecuteReader(CommandBehavior.SingleResult);
                        
                // read the Result and safe it to the temperature entity
                // return null if record does not exist
                if (result.Read())
                {
                    entity.Id = Int16.Parse(result[0].ToString());
                    entity.DateTime = DateTime.Parse(result[1].ToString());
                    entity.Value = float.Parse(result[2].ToString());
                    result.Close();
                }
                else
                {
                    result.Close();
                    return null;
                }
            }

            return entity;
        }

        public IEnumerable<TempModel> GetTemps()
        {
            string queryString = "SELECT * FROM temperatures ORDER BY datetime DESC;";
            var temperatures = new List<TempModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                using NpgsqlDataReader result = command.ExecuteReader();

                // read every row and add it to temperature list
                while (result.Read())
                {
                    temperatures.Add(new TempModel
                    {
                        Id = Int16.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            return temperatures;
        }

        public IEnumerable<TempModel> GetTempsByDate(DateTime date)
        {
            string queryString = "SELECT * FROM temperatures WHERE datetime >= @startDate AND datetime < @endDate ORDER BY datetime DESC;";
            var temperatures = new List<TempModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("startDate", date);
                command.Parameters.AddWithValue("endDate", date.AddDays(1));
                using NpgsqlDataReader result = command.ExecuteReader();
                
                // read every row and add it to temperature list
                while (result.Read())
                {
                    temperatures.Add(new TempModel
                    {
                        Id = Int16.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            return temperatures;
        }
        
        public int Count()
        {
            string queryString = "SELECT COUNT(*) FROM temperatures;";
            int count = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                using NpgsqlDataReader result = command.ExecuteReader();

                // read the count
                if (result.Read())
                {
                    count = Int16.Parse(result[0].ToString());
                }

                result.Close();
            }

            return count;
        }
        
        public TempPaginatedList GetTempsAsPaginatedList(int pageIndex, int pageSize)
        {
            var paginatedList = new TempPaginatedList(pageIndex, pageSize, Count());
            string queryString = "SELECT * FROM temperatures ORDER BY datetime DESC OFFSET @skip LIMIT @pageSize;";
            var temperatures = new List<TempModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("skip", paginatedList.Skip);
                command.Parameters.AddWithValue("pageSize", pageSize);
                using NpgsqlDataReader result = command.ExecuteReader();

                // read every row and add it to temperature list
                while (result.Read())
                {
                    temperatures.Add(new TempModel
                    {
                        Id = Int16.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            paginatedList.Items = temperatures;
            return paginatedList;
        }
        
        public TempPaginatedList GetTempsByDateAsPaginatedList(DateTime date, int pageIndex, int pageSize)
        {
            var paginatedList = new TempPaginatedList(pageIndex, pageSize, Count());
            string queryString = "SELECT * FROM temperatures WHERE datetime >= @startDate AND datetime < @endDate ORDER BY datetime DESC OFFSET @skip LIMIT @pageSize;";
            var temperatures = new List<TempModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("startDate", date);
                command.Parameters.AddWithValue("endDate", date.AddDays(1));
                command.Parameters.AddWithValue("skip", paginatedList.Skip);
                command.Parameters.AddWithValue("pageSize", pageSize);
                using NpgsqlDataReader result = command.ExecuteReader();

                // read every row and add it to temperature list
                while (result.Read())
                {
                    temperatures.Add(new TempModel
                    {
                        Id = Int16.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            paginatedList.Items = temperatures;
            return paginatedList;
        }
    }
}