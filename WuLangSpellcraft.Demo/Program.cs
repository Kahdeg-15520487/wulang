using System;
using WuLangSpellcraft.Demo.Demonstrations;
using WuLangSpellcraft.Demo.Interactive;

namespace WuLangSpellcraft
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowWelcomeMessage();
            ShowMainMenu();
        }

        private static void ShowWelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine("🌟 WELCOME TO WULANG SPELLCRAFT DEMONSTRATION 🌟");
            Console.WriteLine(new string('═', 60));
            Console.WriteLine();
            Console.WriteLine("A comprehensive magical system based on Wu Xing (五行) philosophy");
            Console.WriteLine("Featuring elements, talismans, circles, artifacts, and compositions");
            Console.WriteLine();
        }

        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("📋 MAIN DEMONSTRATION MENU:");
                Console.WriteLine("  1. 🔥 Elemental System");
                Console.WriteLine("  2. 🧿 Talisman System");
                Console.WriteLine("  3. ⚖️ Stability-Based Casting");
                Console.WriteLine("  4. 🔮 Magic Circles");
                Console.WriteLine("  5. 🏗️ Composition System");
                Console.WriteLine("  6. 🏺 Artifact System");
                Console.WriteLine("  7. 💾 Serialization System");
                Console.WriteLine("  8. 🎓 Interactive Workshop");
                Console.WriteLine("  9. Exit");
                Console.WriteLine();
                Console.Write("Choose a demonstration (1-9): ");

                var choice = Console.ReadKey(true).KeyChar;
                Console.Clear();
                
                // Show header again after clear
                Console.WriteLine("🌟 WULANG SPELLCRAFT DEMONSTRATION 🌟");
                Console.WriteLine(new string('═', 45));
                Console.WriteLine();

                switch (choice)
                {
                    case '1':
                        ElementalSystemDemo.Run();
                        break;
                    case '2':
                        TalismanSystemDemo.Run();
                        break;
                    case '3':
                        StabilityCastingDemo.Run();
                        break;
                    case '4':
                        MagicCircleDemo.Run();
                        break;
                    case '5':
                        CompositionSystemDemo.Run();
                        break;
                    case '6':
                        ArtifactSystemDemo.Run();
                        break;
                    case '7':
                        SerializationSystemDemo.Run();
                        break;
                    case '8':
                        InteractiveWorkshop.Run();
                        break;
                    case '9':
                        Console.WriteLine("🌟 Thank you for exploring Wu Lang Spellcraft! 🌟");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please enter 1-9.");
                        break;
                }

                if (choice != '9')
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
    }
}
