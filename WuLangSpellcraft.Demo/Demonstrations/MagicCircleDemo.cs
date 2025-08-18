using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates magic circle creation, connections, and network operations
    /// </summary>
    public static class MagicCircleDemo
    {
        private static MagicCircle[] _demonstrationCircles = Array.Empty<MagicCircle>();

        public static void Run()
        {
            Console.WriteLine("🔮 MAGIC CIRCLE SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🎯 Creating and Connecting Magic Circles:");
            Console.WriteLine();
            
            var (baseCircle, upperCircle) = CreateDemonstrationCircles();
            
            ShowCircleDetails(baseCircle, upperCircle);
            ShowCircleConnections(baseCircle, upperCircle);
            ShowCircleAnalysis(baseCircle, upperCircle);
            
            // Store for other demos
            _demonstrationCircles = new[] { baseCircle, upperCircle };
        }

        public static MagicCircle[] GetDemonstrationCircles() => _demonstrationCircles;

        private static (MagicCircle baseCircle, MagicCircle upperCircle) CreateDemonstrationCircles()
        {
            // Create base circle with Wu Xing elements
            var baseTalismans = new[]
            {
                new Talisman(new Element(ElementType.Fire, 0.8), "Fire Node"),
                new Talisman(new Element(ElementType.Water, 0.8), "Water Node"),
                new Talisman(new Element(ElementType.Earth, 0.8), "Earth Node"),
                new Talisman(new Element(ElementType.Metal, 0.8), "Metal Node"),
                new Talisman(new Element(ElementType.Wood, 0.8), "Wood Node")
            };
            
            var baseCircle = new MagicCircle("Wu Xing Base Circle", 5.0);
            foreach (var talisman in baseTalismans)
            {
                baseCircle.AddTalisman(talisman);
            }
            
            // Create upper circle with derived elements
            var upperTalismans = new[]
            {
                new Talisman(new Element(ElementType.Lightning, 0.7), "Lightning Node"),
                new Talisman(new Element(ElementType.Light, 0.7), "Light Node"),
                new Talisman(new Element(ElementType.Wind, 0.7), "Wind Node")
            };
            
            var upperCircle = new MagicCircle("Derived Elements Circle", 3.0);
            foreach (var talisman in upperTalismans)
            {
                upperCircle.AddTalisman(talisman);
            }

            return (baseCircle, upperCircle);
        }

        private static void ShowCircleDetails(MagicCircle baseCircle, MagicCircle upperCircle)
        {
            Console.WriteLine("🏛️ Base Circle (Wu Xing Formation):");
            DisplayMagicCircle(baseCircle);
            
            Console.WriteLine("⚡ Upper Circle (Derived Elements):");
            DisplayMagicCircle(upperCircle);
        }

        private static void ShowCircleConnections(MagicCircle baseCircle, MagicCircle upperCircle)
        {
            Console.WriteLine("🔗 Connecting Circles:");
            var connection = baseCircle.ConnectTo(upperCircle, ConnectionType.Resonance);
            
            Console.WriteLine($"  Connection established: {baseCircle.Connections.Count} connected circle(s)");
            Console.WriteLine($"  Total combined power: {baseCircle.PowerOutput + upperCircle.PowerOutput:F2}");
            Console.WriteLine();
        }

        private static void ShowCircleAnalysis(MagicCircle baseCircle, MagicCircle upperCircle)
        {
            Console.WriteLine("📊 Circle Analysis:");
            AnalyzeMagicCircle("Base Circle", baseCircle);
            AnalyzeMagicCircle("Upper Circle", upperCircle);
            
            Console.WriteLine("💡 Key Features:");
            Console.WriteLine("  • Automatic talisman positioning around circumference");
            Console.WriteLine("  • Real-time power and stability calculations");
            Console.WriteLine("  • Network connections between circles");
            Console.WriteLine("  • Support for nested circle compositions");
            Console.WriteLine("  • Visual programming through sacred geometry");
        }

        public static void DisplayMagicCircle(MagicCircle circle)
        {
            Console.WriteLine($"  Radius: {circle.Radius:F2}");
            Console.WriteLine($"  Power Output: {circle.PowerOutput:F2}");
            Console.WriteLine($"  Stability: {circle.Stability:F2}");
            Console.WriteLine($"  Talisman Count: {circle.Talismans.Count}");
            Console.WriteLine("  Talismans:");
            
            foreach (var talisman in circle.Talismans)
            {
                Console.ForegroundColor = talisman.PrimaryElement.Color;
                Console.WriteLine($"    • {talisman.Name} ({talisman.PrimaryElement.ChineseName}{talisman.PrimaryElement.Name})");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void AnalyzeMagicCircle(string name, MagicCircle circle)
        {
            Console.WriteLine($"{name}:");
            Console.WriteLine($"  • Total Nodes: {circle.Talismans.Count}");
            Console.WriteLine($"  • Power Density: {circle.PowerOutput / circle.Talismans.Count:F2} per node");
            Console.WriteLine($"  • Stability Rating: {GetStabilityRating(circle.Stability)}");
            Console.WriteLine($"  • Element Diversity: {circle.Talismans.Select(t => t.PrimaryElement.Type).Distinct().Count()} unique elements");
            Console.WriteLine();
        }

        private static string GetStabilityRating(double stability)
        {
            return stability switch
            {
                >= 0.9 => "Exceptional ⭐⭐⭐⭐⭐",
                >= 0.7 => "High ⭐⭐⭐⭐",
                >= 0.5 => "Moderate ⭐⭐⭐",
                >= 0.3 => "Low ⭐⭐",
                _ => "Critical ⭐"
            };
        }
    }
}
