using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ReservationSqlDao : IReservationDao
    {
        private readonly string connectionString;

        public ReservationSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int CreateReservation(int siteId, string name, DateTime fromDate, DateTime toDate)
        {
            int reservationId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date, create_date)" +
                    "OUTPUT INSERTED.reservation_id " +
                    "VALUES (@site_id, @name, @from_date, @to_date, GETDATE())", conn);

                cmd.Parameters.AddWithValue("@site_id", siteId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@from_date", fromDate);
                cmd.Parameters.AddWithValue("@to_date", toDate);

                reservationId = Convert.ToInt32(cmd.ExecuteScalar());

            }
            return reservationId;
        }

        private Reservation GetReservationFromReader(SqlDataReader reader)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            reservation.SiteId = Convert.ToInt32(reader["site_id"]);
            reservation.Name = Convert.ToString(reader["name"]);
            reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
            reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
            reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

            return reservation;
        }



        public IList<Reservation> GetUpcomingReservations(int parkId)
        {

            IList<Reservation> reservations = new List<Reservation>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM reservation" +
                            "JOIN site ON site.site_id = reservation.site_id " +
                            "JOIN campground ON campground.campground_id = site.campground_id " +
                            "WHERE park_id = @park_id", conn);
                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = GetReservationFromReader(reader);
                    reservations.Add(reservation);
                }

            }
            return reservations;
        }
            public IList<Reservation> GetAvailableSites(int parkId)
            {

                IList<Reservation> reservations = new List<Reservation>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM site" +
                                "LEFT JOIN reservation ON site.site_id = reservation.site_id" +
                                "JOIN campground ON campground.campground_id = site.campground_id " +
                                "WHERE create_date IS NULL AND park_id = @park_id", conn);
                    cmd.Parameters.AddWithValue("@park_id", parkId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Reservation reservation = GetReservationFromReader(reader);
                        reservations.Add(reservation);
                    }

                }
                return reservations;
            }
    }
}