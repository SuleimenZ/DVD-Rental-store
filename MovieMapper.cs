using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace DVD_Rental_store
{
    class MovieMapper
    {
        private string connection_string = "Server=127.0.0.1;User Id=postgres;Password=pwd;Database=rental;";

        public Movie GetById(int id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM movies WHERE movie_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string title = (string)reader["title"];
                        int year = (int)reader["year"];
                        int ageRestriction = (int)reader["age_restriction"];
                        double price = Convert.ToDouble(reader["price"]);
                        return new Movie(id, title, year, ageRestriction, price);
                    } else
                    {
                        return null;
                    }
                }
            }
        }

        public void Save(Movie movie)
        {
            var title = movie.Title;
            var year = movie.Year;
            var minAge = movie.AgeRestriction;
            var id = movie.Id;
            var price = movie.Price;

            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO movies (title, year, age_restriction, movie_id, price)" +
                                                   "VALUES (@title, @year, @minAge, @id, @price)", conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@minAge", minAge);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(Movie movie)
        {
            var id = movie.Id;
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM movies " +
                                                   "WHERE movie_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
