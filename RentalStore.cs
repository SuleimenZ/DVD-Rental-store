using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVD_Rental_store
{
    class RentalStore
    {
        public void Run()
        {
            ConsoleMenuElement[] menuElements = {
            new ConsoleMenuElement("Show Full list", ShowFullList),
            new ConsoleMenuElement("Show by client id", ShowClientRentals),
            new ConsoleMenuElement("Create new rental",CreateRental),
            new ConsoleMenuElement("Register return of a copy", ReturnOfACopy),
            new ConsoleMenuElement("Add new client", CreateClient)};

            ConsoleMenu menu = new ConsoleMenu("Menu", menuElements);
            menu.RunMenu();
        }

        public void ShowFullList()
        {
            Console.Clear();
            MovieMapper mm = new MovieMapper();
            Console.WriteLine(mm.ToString());
            Console.ReadLine();
        }

        public void ShowClientRentals()
        {
            int id;
            RentalMapper rm = new RentalMapper();

            do
            {
                Console.Clear();
                Console.Write("Write client id to show his rentals: ");
            } while (!int.TryParse(Console.ReadLine(), out id));

            List<Rental> rentals = rm.GetClientHistory(id);
            foreach (var rental in rentals)
            {
                string returned = rental.Date_of_return.ToString() == "" ? "not returned yet" : $"returned {rental.Date_of_return}";
                Console.WriteLine($"Rental of copy {rental.Copy_id}, rented {rental.Date_of_rental} and {returned}");
            }
            Console.ReadLine();
        }

        public void CreateRental()
        {
            int client_id;
            int copy_id;
            RentalMapper rm = new RentalMapper();
            ClientMapper clm = new ClientMapper();
            CopyMapper cpm = new CopyMapper();
            MovieMapper mm = new MovieMapper();

            int lastClient_id = clm.GetLastId();
            do
            {
                Console.Clear();
                Console.WriteLine("\n" + clm.ToString());
                Console.SetCursorPosition(0, 0);
                Console.Write("Client id: ");
            } while (!int.TryParse(Console.ReadLine(), out client_id) || client_id <= 0 ||client_id > lastClient_id);

            var AvailableCopies = cpm.GetAvailableCopies();
            string copiesList = "";
            List<int> ids = new List<int>();

            foreach(var copies in AvailableCopies)
            {
                Movie movie = mm.GetByCopyId(copies.Copy_id);
                copiesList += $"{copies.Copy_id}: {movie.Title}, price: {movie.Title}\n";
                ids.Add(copies.Copy_id);
            }

            do
            {
                Console.Clear();
                Console.WriteLine("\n" + copiesList);
                Console.SetCursorPosition(0, 0);
                Console.Write("Select copy id: ");
            } while (!int.TryParse(Console.ReadLine(), out copy_id) || !ids.Contains(copy_id));


            var copy = cpm.GetById(copy_id);
            copy.updateAvailability(false);
            cpm.Save(copy);
            rm.Save(new Rental(rm.GetNextId(), copy_id, client_id, DateTime.Now));
        }

        public void ReturnOfACopy()
        {
            int copy_id;
            ClientMapper clm = new ClientMapper();
            CopyMapper cpm = new CopyMapper();
            MovieMapper mm = new MovieMapper();
            RentalMapper rm = new RentalMapper();

            List<Copy> unreternedCopies = cpm.GetUnreturnedCopies();
            List<int> copyIds = new List<int>();

            if(unreternedCopies.Count != 0)
            {
                do
                {
                    Console.Clear();
                    foreach (var copy in unreternedCopies)
                    {
                        Console.WriteLine($"\nCopy {copy.Copy_id}: {mm.GetByCopyId(copy.Copy_id).Title}");
                        Console.SetCursorPosition(0, 0);
                        Console.Write("Write copy id that was returned: ");
                        copyIds.Add(copy.Copy_id);
                    }
                } while (!int.TryParse(Console.ReadLine(), out copy_id) || !copyIds.Contains(copy_id));

                Rental rental = rm.GetByCopyId(copy_id);

                rental.SetReturned();
                rm.Save(rental);
            }
        }

        public void CreateClient()
        {
            DateSelector ds = new DateSelector();
            ClientMapper clm = new ClientMapper();
            string name, surname;
            do
            {
                Console.Clear();
                Console.WriteLine("Write name of new client: ");
                name = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(name) || !name.All(char.IsLetter));

            do
            {
                Console.Clear();
                Console.WriteLine("Write surname of new client: ");
                surname = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(surname) || !surname.All(char.IsLetter));

            int[] date = ds.GetDate();

            clm.Save(new Client(clm.GetNextId(), name, surname, new DateTime(date[2], date[1], date[0], 0, 0, 0)));
        }
    }
}
