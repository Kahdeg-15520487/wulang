using System;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Demo.Demonstrations;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive element combination testing laboratory
    /// </summary>
    public static class ElementCombinationLab
    {
        public static void Run()
        {
            Console.WriteLine("⚗️ ELEMENT COMBINATION TESTING LAB");
            Console.WriteLine(new string('-', 30));
            
            ShowAvailableElements();
            
            var element1 = GetElementChoice("first");
            if (element1.HasValue)
            {
                var element2 = GetElementChoice("second");
                if (element2.HasValue)
                {
                    TestElementCombination(element1.Value, element2.Value);
                }
            }
        }

        private static void ShowAvailableElements()
        {
            var elements = Enum.GetValues<ElementType>();
            Console.WriteLine("Available Elements:");
            for (int i = 0; i < elements.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {elements[i]}");
            }
        }

        private static ElementType? GetElementChoice(string ordinal)
        {
            var elements = Enum.GetValues<ElementType>();
            Console.Write($"Choose {ordinal} element (1-{elements.Length}): ");
            
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= elements.Length)
            {
                return elements[choice - 1];
            }
            
            Console.WriteLine($"❌ Invalid {ordinal} element choice.");
            return null;
        }

        private static void TestElementCombination(ElementType element1, ElementType element2)
        {
            Console.WriteLine();
            Console.WriteLine("🧪 Testing Combination:");
            ElementalSystemDemo.ShowElementRelation(element1, element2, "User-selected combination");
            
            var derived = Element.TryCreateDerivedElement(element1, element2);
            if (derived.HasValue)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✨ Combination successful! Created: {derived.Value}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ No derived element from this combination.");
            }
            Console.ResetColor();
        }
    }
}
