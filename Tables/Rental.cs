using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVD_Rental_store
{
    class Rental
    {
        public int Rental_id { get; set; }
        public int Copy_id { get; set; }
        public int Client_id { get; set; }
        public string Date_of_rental { get; set; }
        public string Date_of_return { get; set; }

        public Rental(int rental_id, int copy_id, int client_id, string date_of_rental, string date_of_return)
        {
            Rental_id = rental_id;
            Copy_id = copy_id;
            Client_id = client_id;
            Date_of_rental = date_of_rental;
            Date_of_return = date_of_return;
        }

        public override string ToString()
        {
            return $"Rent {Rental_id}: , rented copy {Copy_id} by customer {Client_id} at {Date_of_rental}, returned at {Date_of_return}";
        }
    }
}
