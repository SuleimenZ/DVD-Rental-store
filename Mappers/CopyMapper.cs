using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace DVD_Rental_store
{
    class CopyMapper
    {
        private string connection_string = "Server=127.0.0.1;User Id=postgres;Password=pwd;Database=rental;";

        public Copy GetById(int copy_id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM copies WHERE copy_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", copy_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int movie_id = (int)reader["movie_id"];
                        bool available = (bool)reader["available"];
                        return new Copy(copy_id, movie_id, available);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void Save(Copy copy)
        {
            var copy_id = copy.Copy_id;
            var movie_id = copy.Movie_id;
            var available = copy.Available;

            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO movies (copy_id, available, movie_id)" +
                                                   "VALUES (@copy_id, @available, @movie_id)", conn))
                {
                    cmd.Parameters.AddWithValue("@copy_id", copy_id);
                    cmd.Parameters.AddWithValue("@available", available);
                    cmd.Parameters.AddWithValue("@movie_id", movie_id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(Copy copy)
        {
            var copy_id = copy.Copy_id;
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM copies " +
                                                   "WHERE copy_id = @copy_id", conn))
                {
                    cmd.Parameters.AddWithValue("@copy_id", copy_id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
