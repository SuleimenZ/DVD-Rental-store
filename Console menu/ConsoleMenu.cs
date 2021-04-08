using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVD_Rental_store
{
    class ConsoleMenu
    {
        private bool notExit = true;
        private string Description { get; set; }
        private ConsoleMenuElement[] MenuElements { get; set; }
        public int SelectedElement { get; private set; }
        public ConsoleMenu(string description, ConsoleMenuElement[] menuElements)
        {
            SelectedElement = 0;
            Description = description;
            MenuElements = menuElements;
        }
        public void PrintMenu()
        {
            Console.WriteLine(Description);
            for (int i = 0; i < MenuElements.Length; i++)
            {
                if (i == SelectedElement)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(MenuElements[i]);
                Console.ResetColor();
            }
        }
        public void RunMenu()
        {
            while (notExit)
            {
                Console.Clear();
                PrintMenu();
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                switch (pressedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        SelectedElement = (SelectedElement - 1 + MenuElements.Length) % MenuElements.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        SelectedElement = (SelectedElement + 1) % MenuElements.Length;
                        break;
                    case ConsoleKey.Enter:
                        if (MenuElements.Length == 0) { break; }
                        MenuElements[SelectedElement].ActionToRun();
                        break;
                    case ConsoleKey.Escape:
                        notExit = false;
                        
                        break;
                }
            }
        }
    }
}
