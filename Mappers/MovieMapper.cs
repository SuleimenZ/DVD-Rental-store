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

        public Movie GetByCopyId(int id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM movies " +
                                                   "JOIN copies ON copies.movie_id = movies.movie_id " +
                                                   "WHERE copy_id = @id", conn))
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
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int GetLastId()
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT MAX(movie_id) FROM movies", conn))
                {
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return (int)reader["max"];
                    }
                    return 0;
                }

            }
        }

        public int GetNextId()
        {
            return GetLastId() + 1;
        }

        public void SaveNewAndAddCopy(Movie movie)
        {
            var cm = new CopyMapper();
            var title = movie.Title;
            var year = movie.Year;
            var minAge = movie.AgeRestriction;
            var id = movie.Id;
            var price = movie.Price;

            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                var cmd = new NpgsqlCommand();
                cmd.Transaction = transaction;
                try
                {
                    cmd = new NpgsqlCommand("INSERT INTO movies (title, year, age_restriction, movie_id, price)" +
                                            "VALUES (@title, @year, @minAge, @id, @price)", conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@minAge", minAge);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.ExecuteNonQuery();

                    cmd = new NpgsqlCommand("INSERT INTO copies (copy_id, available, movie_id)" +
                                            "VALUES (@copy_id, @available, @movie_id)", conn);
                    cmd.Parameters.AddWithValue("@copy_id", cm.GetNextId());
                    cmd.Parameters.AddWithValue("@available", true);
                    cmd.Parameters.AddWithValue("@movie_id", GetNextId());

                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e.ToString());
                }
            }
        }
        public string GetStatistics()
        {
            string stats = "";
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT movies.movie_id, title, SUM(price), COUNT(rental_id) FROM rentals " +
                                                   "JOIN copies ON copies.copy_id = rentals.copy_id " +
                                                   "JOIN movies ON movies.movie_id = copies.movie_id " +
                                                   "JOIN clients ON clients.client_id = rentals.client_id GROUP BY movies.movie_id, title " +
                                                   "ORDER BY movies.movie_id", conn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        double sum = (Single)reader["sum"];
                        stats += $"\nID {reader["movie_id"]}: {reader["title"]}; Rentals: {reader["count"]}, Money earned: {sum.ToString()}";
                    }
                }
            }

            return $"Total movies: {GetLastId()}. Those which were rented:" + stats;
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
                                                   "VALUES (@title, @year, @minAge, @id, @price) " +
                                                   "ON CONFLICT (movie_id) DO UPDATE " +
                                                   "SET title = @title, year = @year, minAge = @minAge, price = @price", conn))
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

        public override string ToString()
        {
            string result = "";
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using(var cmd = new NpgsqlCommand("SELECT movies.movie_id, title, COUNT(CASE WHEN available = true THEN copy_id end) as available, COUNT(copies.movie_id) AS max " +
                                                  "FROM movies " +
                                                  "JOIN copies ON copies.movie_id = movies.movie_id " +
                                                  "GROUP BY movies.movie_id " +
                                                  "ORDER BY movies.movie_id", conn))
                {
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {                        
                        result += $"{reader["movie_id"]}: {reader["title"]}, {reader["available"]}/{reader["max"]} available\n";
                    }
                    return result;
                }
            }
        }
    }
}
