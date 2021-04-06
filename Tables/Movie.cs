using System;
using System.Collections.Generic;
using System.Text;

namespace DVD_Rental_store
{
    class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AgeRestriction { get; set; }
        public int Year { get; set; }
        public double Price { get; set; }

        public Movie(int id, string title, int year, int ageRestriction, double price)
        {
            Id = id;
            Title = title;
            Year = year;
            AgeRestriction = ageRestriction;
            Price = price;
        }
        public override string ToString()
        {
            return $"Movie {Id}: {Title} produced in {Year} costs {Price}";
        }

        public void updatePrice(int price)
        {
            Price = price;
        }
    }
}
