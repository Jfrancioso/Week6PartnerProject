using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ParkSqlDao : IParkDao
    {
        private readonly string connectionString;

        public ParkSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Park> GetAllParks()
        {
            IList<Park> parks = new List<Park>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM park ORDER BY location", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Park park = GetParkFromReader(reader);
                    parks.Add(park);
                }
            }
            return parks;
        }

        private Park GetParkFromReader(SqlDataReader reader)
        {
            Park park = new Park();
            park.ParkId = Convert.ToInt32(reader["park_id"]);
            park.Name = Convert.ToString(reader["name"]);
            park.Location = Convert.ToString(reader["location"]);
            park.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
            park.Area = Convert.ToInt32(reader["area"]);
            park.Visitors = Convert.ToInt32(reader["visitors"]);
            park.Description = Convert.ToString(reader["description"]);

            return park;
        }
    }
}
