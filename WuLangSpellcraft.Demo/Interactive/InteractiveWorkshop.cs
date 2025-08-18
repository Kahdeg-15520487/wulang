using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive workshop for hands-on experimentation with WuLang Spellcraft systems
    /// </summary>
    public static class InteractiveWorkshop
    {
        public static void Run()
        {
            Console.WriteLine("🎓 INTERACTIVE WORKSHOP - WuLang Spellcraft Sandbox");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("Welcome to the Wu Lang Spellcraft Interactive Workshop!");
            Console.WriteLine("Here you can experiment with creating your own spells and compositions.");
            Console.WriteLine();
            
            ShowWorkshopMenu();
        }

        private static void ShowWorkshopMenu()
        {
            while (true)
            {
                Console.WriteLine("🎯 Workshop Options:");
                Console.WriteLine("  1. Create custom talisman");
                Console.WriteLine("  2. Design magic circle");
                Console.WriteLine("  3. Test element combinations");
                Console.WriteLine("  4. Build artifact");
                Console.WriteLine("  5. Stability testing lab");
                Console.WriteLine("  6. Exit workshop");
                Console.WriteLine();
                Console.Write("Choose an option (1-6): ");
                
                var choice = Console.ReadLine();
                Console.WriteLine();
                
                switch (choice)
                {
                    case "1":
                        TalismanCreationWorkshop.Run();
                        break;
                    case "2":
                        CircleDesignWorkshop.Run();
                        break;
                    case "3":
                        ElementCombinationLab.Run();
                        break;
                    case "4":
                        ArtifactCreationWorkshop.Run();
                        break;
                    case "5":
                        StabilityTestingLab.Run();
                        break;
                    case "6":
                        Console.WriteLine("🎓 Thank you for using the Wu Lang Spellcraft Workshop!");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid option. Please choose 1-6.");
                        break;
                }
                
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }
    }
}
