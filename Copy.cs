using System;
using System.Collections.Generic;
using System.Text;

namespace DVD_Rental_store
{
    class Copy
    {
        public int Copy_id { get; set; }
        public int Movie_id { get; set; }
        public bool Available { get; set; }

        public Copy(int copy_id, int movie_id, bool available)
        {
            Copy_id = copy_id;
            Movie_id = movie_id;
            Available = available;
        }
        public override string ToString()
        {
            return $"Copy {Copy_id}: Movie id - {Movie_id}, Availability - {Available}";
        }
        public void updateAvailability(bool newState)
        {
            Available = newState;
        }
    }
}
