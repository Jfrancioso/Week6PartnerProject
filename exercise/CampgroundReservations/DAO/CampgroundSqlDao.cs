using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class CampgroundSqlDao : ICampgroundDao
    {
        private readonly string connectionString;

        public CampgroundSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Campground> GetCampgroundsByParkId(int parkId)
        {
            IList<Campground> campgrounds = new List<Campground>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM campground WHERE park_id = @park_id", conn);
                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Campground campground = GetCampgroundFromReader(reader);
                    campgrounds.Add(campground);
                }

            }
            return campgrounds;

            //throw new NotImplementedException();
        }

        private Campground GetCampgroundFromReader(SqlDataReader reader)
        {
            Campground campground = new Campground();
            campground.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            campground.ParkId = Convert.ToInt32(reader["park_id"]);
            campground.Name = Convert.ToString(reader["name"]);
            campground.OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]);
            campground.OpenToMonth = Convert.ToInt32(reader["open_to_mm"]);
            campground.DailyFee = Convert.ToDouble(reader["daily_fee"]);

            return campground;
        }
    }
}
