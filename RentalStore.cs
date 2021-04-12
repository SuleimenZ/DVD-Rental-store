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
            new ConsoleMenuElement("1.Show Full list", ShowFullList),
            new ConsoleMenuElement("2.Show by client id", ShowClientRentals),
            new ConsoleMenuElement("3.Create new rental",CreateRental),
            new ConsoleMenuElement("4.Register return of a copy", ReturnOfACopy),
            new ConsoleMenuElement("5.Add new client", CreateClient),
            new ConsoleMenuElement("6.Add new movie to the database", CreateMovie),
            new ConsoleMenuElement("7.Show statistics", ShowStatistics),
            new ConsoleMenuElement("8.Show overdue rentals", ShowOverdueRentals)};

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
            string returned;
            RentalMapper rm = new RentalMapper();

            do
            {
                Console.Clear();
                Console.Write("Write client id to show his rentals: ");
            } while (!int.TryParse(Console.ReadLine(), out id));

            List<Rental> rentals = rm.GetClientHistory(id);
            if(rentals.Count == 0)
            {
                Console.WriteLine("No clients with such id exists / No copy is rented by this client yet");
            }
            foreach (var rental in rentals)
            {
                returned = string.IsNullOrWhiteSpace(rental.Date_of_return.ToString()) ? "not returned yet" : $"returned {rental.Date_of_return}";
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
            rm.Save(new Rental(rm.GetNextId(), copy_id, client_id, DateTime.Now, null));
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
            } while (string.IsNullOrWhiteSpace(name) || !name.Replace(" ", "").All(char.IsLetter));

            do
            {
                Console.Clear();
                Console.WriteLine("Write surname of new client: ");
                surname = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(surname) || !surname.Replace(" ", "").All(char.IsLetter));

            int[] date = ds.GetDate();

            clm.Save(new Client(clm.GetNextId(), name, surname, new DateTime(date[2], date[1], date[0], 0, 0, 0)));
        }

        public void CreateMovie()
        {
            string title;
            int year, price, ageRestriction;
            var mm = new MovieMapper();
            do
            {
                Console.Clear();
                Console.WriteLine("Write name of new movie: ");
                title = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(title));

            do
            {
                Console.Clear();
                Console.Write("Write year of this movie: ");
            } while (!int.TryParse(Console.ReadLine(), out year) || year < 1887 || year > DateTime.Today.Year);

            do
            {
                Console.Clear();
                Console.Write("Write price of this movie: ");
            } while (!int.TryParse(Console.ReadLine(), out price) || price < 0);

            do
            {
                Console.Clear();
                Console.Write("Write age of restriction: " );
            } while (!int.TryParse(Console.ReadLine(), out ageRestriction));

            var movie = new Movie(mm.GetNextId(), title, year, ageRestriction, price);
            mm.SaveNewAndAddCopy(movie);
        }

        public void ShowStatistics()
        {
            var mm = new MovieMapper();
            var rm = new RentalMapper();

            Console.Clear();
            Console.WriteLine(rm.GetStatistics() + "\n\n" + mm.GetStatistics());
            Console.ReadLine();
        }

        public void ShowOverdueRentals()
        {
            Console.Clear();
            var rm = new RentalMapper();
            List<Rental> rentals = rm.GetOverdueRentals();

            foreach(var rental in rentals)
            {
                Console.WriteLine($"Rental {rental.Rental_id}: Copy {rental.Copy_id} was rented on {rental.Date_of_rental} and overdue by {rm.GetOverdueInDays(rental.Rental_id)} days.");
            }
            Console.ReadLine();
        }
    }
}
