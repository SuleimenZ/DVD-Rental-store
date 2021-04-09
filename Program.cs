using System;
using System.Collections.Generic;

namespace DVD_Rental_store
{
    class Program
    {
        static void Main(string[] args)
        {

            DateSelector date = new DateSelector();

            RentalStore store = new RentalStore();

            store.Run();

            Console.ReadLine();
        }
    }
}
