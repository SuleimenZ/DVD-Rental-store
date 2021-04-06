using System;

namespace DVD_Rental_store
{
    class Program
    {
        static void Main(string[] args)
        {
            var MovieMapper = new MovieMapper();
            var Movie = new Movie(12, "test1", 2021,18, 200);
            MovieMapper.Save(Movie);

            MovieMapper.Delete(Movie);
        }
    }
}
