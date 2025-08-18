using System;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Demo.Demonstrations;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive magic circle design workshop
    /// </summary>
    public static class CircleDesignWorkshop
    {
        public static void Run()
        {
            Console.WriteLine("🔮 MAGIC CIRCLE CREATION WORKSHOP");
            Console.WriteLine(new string('-', 30));
            
            var name = GetCircleName();
            var radius = GetCircleRadius();
            
            if (radius.HasValue)
            {
                var talismanCount = GetTalismanCount();
                if (talismanCount.HasValue)
                {
                    CreateAndDisplayCircle(name, radius.Value, talismanCount.Value);
                }
            }
        }

        private static string GetCircleName()
        {
            Console.Write("Enter circle name: ");
            return Console.ReadLine() ?? "Custom Circle";
        }

        private static double? GetCircleRadius()
        {
            Console.Write("Enter circle radius (1-10): ");
            if (double.TryParse(Console.ReadLine(), out double radius) && radius >= 1 && radius <= 10)
            {
                return radius;
            }
            
            Console.WriteLine("❌ Invalid radius.");
            return null;
        }

        private static int? GetTalismanCount()
        {
            Console.WriteLine("How many talismans to add? (1-8): ");
            if (int.TryParse(Console.ReadLine(), out int count) && count >= 1 && count <= 8)
            {
                return count;
            }
            
            Console.WriteLine("❌ Invalid talisman count.");
            return null;
        }

        private static void CreateAndDisplayCircle(string name, double radius, int talismanCount)
        {
            var circle = new MagicCircle(name, radius);
            
            for (int i = 0; i < talismanCount; i++)
            {
                var randomElement = (ElementType)Random.Shared.Next(0, Enum.GetValues<ElementType>().Length);
                var randomEnergy = 0.5 + Random.Shared.NextDouble() * 0.5;
                var talisman = new Talisman(new Element(randomElement, randomEnergy), $"Node {i + 1}");
                circle.AddTalisman(talisman);
            }
            
            Console.WriteLine();
            Console.WriteLine("✅ Magic circle created successfully!");
            MagicCircleDemo.DisplayMagicCircle(circle);
        }
    }
}
