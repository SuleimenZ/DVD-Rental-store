using System;
using System.Collections.Generic;
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
                        DateTime date_of_rental = (DateTime)reader["date_of_rental"];
                        DateTime date_of_return = (DateTime)reader["date_of_return"];
                        return new Rental(rental_id, copy_id, client_id, date_of_rental, date_of_return);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public Rental GetByCopyId(int id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM rentals " +
                                                   "JOIN copies ON copies.copy_id = rentals.copy_id " +
                                                   "WHERE rentals.copy_id = @id AND date_of_return IS null", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int rental_id = (int)reader["rental_id"];
                        int copy_id = (int)reader["copy_id"];
                        int client_id = (int)reader["client_id"];
                        DateTime date_of_rental = (DateTime)reader["date_of_rental"];
                        if(reader["date_of_return"] == null)
                        {
                            return new Rental(rental_id, copy_id, client_id, date_of_rental, (DateTime)reader["date_of_return"]);
                        }
                        else
                        {
                            return new Rental(rental_id, copy_id, client_id, date_of_rental, null);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public List<Rental> GetClientHistory(int client_id)
        {
            List<Rental> rentals = new List<Rental>();
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM rentals " +
                                                   "JOIN copies ON copies.copy_id = rentals.copy_id " +
                                                   "WHERE client_id = @id " +
                                                   "ORDER BY date_of_rental", conn))
                {
                    cmd.Parameters.AddWithValue("@id", client_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int rental_id = (int)reader["rental_id"];
                        int copy_id = (int)reader["copy_id"];
                        DateTime date_of_rental = (DateTime)reader["date_of_rental"];
                        DateTime date_of_return = (DateTime)reader["date_of_return"];
                        rentals.Add(new Rental(rental_id, copy_id, client_id, date_of_rental, date_of_return));
                    }
                }
            }

            return rentals;
        }

        public int GetNumberOfRentalsByClientId(int id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(rental_id) FROM rentals " +
                                                   "WHERE client_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return (int)(long)reader["count"];
                    }
                    return 0;
                }

            }
        }

        public string GetStatistics()
        {
            string stats = "";
            string total;
            double totalsum = 0;
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT clients.client_id, first_name, last_name, SUM(price) FROM rentals " +
                                                   "JOIN copies ON copies.copy_id = rentals.copy_id " +
                                                   "JOIN movies ON movies.movie_id = copies.movie_id " +
                                                   "JOIN clients ON clients.client_id = rentals.client_id " +
                                                   "GROUP BY clients.client_id, first_name, last_name " +
                                                   "ORDER BY clients.client_id", conn))
                {
                    var reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        double sum = (Single)reader["sum"];
                        totalsum += sum;
                        stats += $"\nID {reader["client_id"]}: {reader["first_name"]} {reader["last_name"]}; " +
                            $"Money spent: {sum.ToString()}, avg per rental: " +
                            $"{String.Format("{0:0.00}", sum / GetNumberOfRentalsByClientId((int)reader["client_id"]))}";
                    }
                    total = $"Total rentals: {GetLastId()}, total money spent: {totalsum}.";
                }
            }

            return total + stats;
        }

        public int GetLastId()
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT MAX(rental_id) FROM rentals", conn))
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
                using (var cmd = new NpgsqlCommand("INSERT INTO rentals (rental_id, copy_id, client_id, date_of_rental, date_of_return)" +
                                                   "VALUES (@rental_id, @copy_id, @client_id, @date_of_rental, @date_of_return)" +
                                                   "ON CONFLICT (rental_id) DO UPDATE " +
                                                   "SET copy_id = @copy_id, client_id = @client_id, date_of_rental = @date_of_rental, date_of_return = @date_of_return", conn))
                {
                    cmd.Parameters.AddWithValue("@rental_id", rental_id);
                    cmd.Parameters.AddWithValue("@copy_id", copy_id);
                    cmd.Parameters.AddWithValue("@client_id", client_id);
                    cmd.Parameters.AddWithValue("@date_of_rental", date_of_rental);
                    cmd.Parameters.AddWithValue("@date_of_return", date_of_return == null ? DBNull.Value : date_of_return);

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

        public override string ToString()
        {
            string result = "";
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM rentals " +
                                                   "JOIN clients ON clients.client_id = rentals.client_id " +
                                                   "ORDER BY rental_id", conn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var returned = reader["date_of_return"].ToString() == "" ? "not yet returned" : $"returned on {reader["date_of_return"]}";
                        result += $"{reader["rental_id"]}: {reader["first_name"]} {reader["last_name"]} rented copy {reader["copy_id"]} on {reader["date_of_rental"]} and {returned} \n";
                    }
                    return result;
                }
            }
        }
    }
}
