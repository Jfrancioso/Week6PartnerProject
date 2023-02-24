using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class SiteSqlDao : ISiteDao
    {
        private readonly string connectionString;

        public SiteSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Site> GetSitesThatAllowRVs(int parkId)
        {
           
            IList<Site> Sites = new List<Site>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT site.site_id, site.camground_id, site.site_number, site.max_occupancy, site.accessbile, site.max_rv_length, site.utilities" +
                    "FROM site" +
                    "JOIN campground ON site.campground_id = campground.campground_id" +
                    "JOIN park ON campground.park_id = park.park_id" +
                    "WHERE campground.park_id = @parkId AND site.max_rv_length > 0", conn);
                     cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = GetSiteFromReader(reader);
                    Sites.Add(site);
                }

            }
            return Sites;
        }


        private Site GetSiteFromReader(SqlDataReader reader)
        {
            Site site = new Site();
            site.SiteId = Convert.ToInt32(reader["site_id"]);
            site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            site.SiteNumber = Convert.ToInt32(reader["site_number"]);
            site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            site.Accessible = Convert.ToBoolean(reader["accessible"]);
            site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
            site.Utilities = Convert.ToBoolean(reader["utilities"]);

            return site;
        }
    }
}
