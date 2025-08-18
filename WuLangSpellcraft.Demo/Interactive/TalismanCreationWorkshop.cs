using System;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Demo.Demonstrations;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive talisman creation workshop
    /// </summary>
    public static class TalismanCreationWorkshop
    {
        public static void Run()
        {
            Console.WriteLine("🧿 TALISMAN CREATION WORKSHOP");
            Console.WriteLine(new string('-', 30));
            
            ShowAvailableElements();
            var element = GetElementChoice();
            
            if (element.HasValue)
            {
                var energy = GetEnergyLevel();
                if (energy.HasValue)
                {
                    var name = GetTalismanName();
                    CreateAndDisplayTalisman(element.Value, energy.Value, name);
                }
            }
        }

        private static void ShowAvailableElements()
        {
            Console.WriteLine("Available Elements:");
            var elements = Enum.GetValues<ElementType>();
            for (int i = 0; i < elements.Length; i++)
            {
                var element = new Element(elements[i], 1.0);
                Console.ForegroundColor = element.Color;
                Console.WriteLine($"  {i + 1}. {element.ChineseName}{element.Name}");
            }
            Console.ResetColor();
        }

        private static ElementType? GetElementChoice()
        {
            var elements = Enum.GetValues<ElementType>();
            Console.Write($"Choose primary element (1-{elements.Length}): ");
            
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= elements.Length)
            {
                return elements[choice - 1];
            }
            
            Console.WriteLine("❌ Invalid element choice.");
            return null;
        }

        private static double? GetEnergyLevel()
        {
            Console.Write("Enter energy level (0.1-1.0): ");
            if (double.TryParse(Console.ReadLine(), out double energy) && energy >= 0.1 && energy <= 1.0)
            {
                return energy;
            }
            
            Console.WriteLine("❌ Invalid energy level.");
            return null;
        }

        private static string GetTalismanName()
        {
            Console.Write("Enter talisman name: ");
            return Console.ReadLine() ?? "Custom Talisman";
        }

        private static void CreateAndDisplayTalisman(ElementType elementType, double energy, string name)
        {
            var talisman = new Talisman(new Element(elementType, energy), name);
            
            Console.WriteLine();
            Console.WriteLine("✅ Talisman created successfully!");
            TalismanSystemDemo.DisplayTalisman($"🧿 {name}", talisman);
        }
    }
}
