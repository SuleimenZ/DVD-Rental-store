using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace DVD_Rental_store
{
    class RentalMapper
    {
        private string connection_string = "Server=127.0.0.1;User Id=postgres;Password=pwd;Database=rental;";

        public Rental GetById(int rental_id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM rentals WHERE rental_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", rental_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int copy_id = (int)reader["copy_id"];
                        int client_id = (int)reader["client_id"];
                        string date_of_rental = (string)reader["date_of_rental"];
                        string date_of_return = (string)reader["date_of_return"];
                        return new Rental(rental_id, copy_id, client_id, date_of_rental, date_of_return);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void Save(Rental rental)
        {
            var rental_id = rental.Rental_id;
            var copy_id = rental.Copy_id;
            var client_id = rental.Client_id;
            var date_of_rental = rental.Date_of_rental;
            var date_of_return = rental.Date_of_return;

            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO movies (rental_id, copy_id, client_id, date_of_rental, date_of_return)" +
                                                   "VALUES (@rental_id, @copy_id, @client_id, @date_of_rental, @date_of_return)", conn))
                {
                    cmd.Parameters.AddWithValue("@rental_id", rental_id);
                    cmd.Parameters.AddWithValue("@copy_id", copy_id);
                    cmd.Parameters.AddWithValue("@client_id", client_id);
                    cmd.Parameters.AddWithValue("@date_of_rental", date_of_rental);
                    cmd.Parameters.AddWithValue("@date_of_return", date_of_return);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(Rental rental)
        {
            var rental_id = rental.Rental_id;
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM rentals " +
                                                   "WHERE rental_id = @rental_id", conn))
                {
                    cmd.Parameters.AddWithValue("@rental_id", rental_id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
