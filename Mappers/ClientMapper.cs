using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace DVD_Rental_store
{
    class ClientMapper
    {
        private string connection_string = "Server=127.0.0.1;User Id=postgres;Password=pwd;Database=rental;";

        public Client GetById(int id)
        {
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM movies WHERE client_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string firstName = (string)reader["first_name"];
                        string lastName = (string)reader["last_name"];
                        DateTime birthdate = (DateTime)reader["birthday"];
                        return new Client(id, firstName, lastName, birthdate);
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
                using (var cmd = new NpgsqlCommand("SELECT MAX(client_id) FROM clients", conn))
                {
                    var reader = cmd.ExecuteReader();

                    if(reader.Read())
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

        public void Save(Client client)
        {
            var id = client.Id;
            var firstName = client.FirstName;
            var lastName = client.LastName;
            var birthdate = client.Birthdate;

            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO clients (client_id, first_name, last_name, birthday)" +
                                                   "VALUES (@id, @firstName, @lastName, @birthdate)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@birthdate", birthdate);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(Client client)
        {
            var id = client.Id;
            using (var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM clients " +
                                                   "WHERE client_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override string ToString()
        {
            string result = "";
            using(var conn = new NpgsqlConnection(connection_string))
            {
                conn.Open();
                using(var cmd = new NpgsqlCommand("SELECT * FROM clients",conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result += $"{reader["client_id"]}: {reader["first_name"]} {reader["last_name"]}\n";
                    }
                    return result;
                }
            }
        }

    }
}
