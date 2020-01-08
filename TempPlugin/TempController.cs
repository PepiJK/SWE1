using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;

namespace TempPlugin
{
    /// <summary>
    /// TempController class that is handling the communication with the PostgreSQL database.
    /// </summary>
    public class TempController
    {
        private readonly string _dbConnectionString;

        /// <summary>
        /// Set the connection string which will be used to connect to the database.
        /// </summary>
        /// <param name="connection"></param>
        public TempController(string connection = "Host=localhost;Username=swe;Password=123456;Database=webserver")
        {
            _dbConnectionString = connection;
        }

        /// <summary>
        /// Insert a row containing either an id, timestamp and value or only a timestamp and value into the table.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Boolean, if entity has successfully been added</returns>
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

        /// <summary>
        /// Delete a row from the table with a specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Boolean, if entity has successfully been removed</returns>
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

        /// <summary>
        /// Return the id, datetime and value of a row with a specific id. Return null when specified id is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A new TempModel Object</returns>
        public TempModel GetTemp(int id)
        {
            string queryString = "SELECT * FROM temperatures WHERE id = @id;";
            TempModel entity;

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
                    entity = new TempModel
                    {
                        Id = int.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    };
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

        /// <summary>
        /// Return a list of rows containing id, datetime and value.
        /// </summary>
        /// <returns>A new Enumerable TempModel Object</returns>
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
                        Id = int.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            return temperatures;
        }

        /// <summary>
        /// Return a list of rows containing id, datetime and value of a specified date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A new Enumerable TempModel Object</returns>
        public IEnumerable<TempModel> GetTempsByDate(DateTime date)
        {
            string queryString =
                "SELECT * FROM temperatures WHERE datetime >= @startDate AND datetime < @endDate ORDER BY datetime DESC;";
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
                        Id = int.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            return temperatures;
        }

        /// <summary>
        /// Return a paginated list of rows containing id, datetime and value with a specified page index and page size.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>A new TempPaginatedList Object</returns>
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
                        Id = int.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            paginatedList.Items = temperatures;
            return paginatedList;
        }

        /// <summary>
        /// Return a paginated list of rows containing id, datetime and value of a specified date, page index and page size.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>A new TempPaginatedList Object</returns>
        public TempPaginatedList GetTempsByDateAsPaginatedList(DateTime date, int pageIndex, int pageSize)
        {
            var paginatedList = new TempPaginatedList(pageIndex, pageSize, CountByDate(date));
            string queryString =
                "SELECT * FROM temperatures WHERE datetime >= @startDate AND datetime < @endDate ORDER BY datetime DESC OFFSET @skip LIMIT @pageSize;";
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
                        Id = int.Parse(result[0].ToString()),
                        DateTime = DateTime.Parse(result[1].ToString()),
                        Value = float.Parse(result[2].ToString())
                    });
                }

                result.Close();
            }

            paginatedList.Items = temperatures;
            return paginatedList;
        }

        /// <summary>
        /// Return the number of rows in the table.
        /// </summary>
        /// <returns>The amount off rows in table</returns>
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
                    count = int.Parse(result[0].ToString());
                }

                result.Close();
            }

            return count;
        }

        /// <summary>
        /// Return the number of rows in the table of a specified date.
        /// </summary>
        /// <returns>The amount off rows in table</returns>
        public int CountByDate(DateTime date)
        {
            string queryString =
                "SELECT COUNT(*) FROM temperatures WHERE datetime >= @startDate AND datetime < @endDate;";
            int count = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(_dbConnectionString))
            {
                connection.Open();
                using NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
                command.Parameters.AddWithValue("startDate", date);
                command.Parameters.AddWithValue("endDate", date.AddDays(1));
                using NpgsqlDataReader result = command.ExecuteReader();

                // read the count
                if (result.Read())
                {
                    count = int.Parse(result[0].ToString());
                }

                result.Close();
            }

            return count;
        }
    }
}