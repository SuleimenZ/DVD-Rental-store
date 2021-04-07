using System;
using System.Collections.Generic;

namespace DVD_Rental_store
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMenuElement[] menuElements = {
                new ConsoleMenuElement("Show Full list", ShowFullList),
            new ConsoleMenuElement("Show by client id", ShowClientRentals)};
            ConsoleMenu menu = new ConsoleMenu("Menu", menuElements);
            menu.RunMenu();

            Console.ReadLine();
        }

        static void ShowFullList()
        {
            Console.Clear();
            MovieMapper mm = new MovieMapper();
            Console.WriteLine(mm.ToString());
            Console.ReadLine();
        }

        static void ShowClientRentals()
        {
            Console.Clear();
            int id;
            RentalMapper rm = new RentalMapper();

            do
            {
                Console.Write("Write client id to show his rentals: ");
            } while (!int.TryParse(Console.ReadLine(), out id));

            List<Rental> rentals = rm.GetClientHistory(id);
            foreach(var rental in rentals)
            {
                string returned = rental.Date_of_return == "" ? "not returned yet" : $"returned {rental.Date_of_return}";
                Console.WriteLine($"Rental of copy {rental.Copy_id}, rented {rental.Date_of_rental} and {returned}");
            }
            Console.ReadLine();
        }
    }
}
