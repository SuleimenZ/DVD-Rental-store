using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVD_Rental_store
{
    class DateSelector
    {
        public int SelectedElement { get; private set; }
        public int Day { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public int[] Date { get; private set; }
        public int[] MaxDate { get; private set; }

        public DateSelector()
        {
            SelectedElement = 0;
            Day = 1;
            Month = 0;
            Year = 0;

            Date = new int[3] { 1, 1, 2000};
            MaxDate = new int[3] { 31, 12, DateTime.Now.Year};
        }
        public int[] GetDate()
        {
            while (true)
            {
                Console.Clear();
                PrintMenu();
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                switch (pressedKey.Key)
                {
                    case ConsoleKey.LeftArrow:
                        SelectedElement = (SelectedElement - 1 + 3) % 3;
                        break;
                    case ConsoleKey.RightArrow:
                        SelectedElement = (SelectedElement + 1) % 3;
                        break;
                    case ConsoleKey.UpArrow:
                        Date[SelectedElement] = (Date[SelectedElement] % MaxDate[SelectedElement]) + 1;
                        break;
                    case ConsoleKey.DownArrow:
                        Date[SelectedElement] = ((Date[SelectedElement] - 2 + MaxDate[SelectedElement]) % MaxDate[SelectedElement]) + 1;
                        break;
                    case ConsoleKey.Enter:
                        return Date;
                }
                MaxDate[0] = DateTime.DaysInMonth(Date[2], Date[1]);
                Date[0] = Date[0] > MaxDate[0] ? MaxDate[0] : Date[0];
            }
        }

        public void PrintMenu()
        {
            Console.WriteLine("Switch between Day/Month/Year with Left arrow/Right arrow. Switch between number using Up arrow/Down arrow. Press Enter to finish");
            for (int i = 0; i < 3; i++)
            {
                string separator = i < 2 ? "." : "";
                if (i == SelectedElement)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write($"{Date[i]}{ separator}");
                Console.ResetColor();
            }
        }

    }
}
