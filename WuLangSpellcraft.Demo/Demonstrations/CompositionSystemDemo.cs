using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates complex multi-circle compositions including network and nested approaches
    /// </summary>
    public static class CompositionSystemDemo
    {
        public static void Run()
        {
            Console.WriteLine("🏗️ COMPOSITION SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🎯 Building Complex Multi-Circle Compositions:");
            Console.WriteLine();
            
            var circles = CreateCompositionCircles();
            ShowNetworkConnections(circles);
            Console.WriteLine();
            ShowNestedComposition(circles);
        }

        private static (MagicCircle primary, MagicCircle secondary, MagicCircle support) CreateCompositionCircles()
        {
            var primaryCircle = new MagicCircle("Primary Defense Circle", 6.0);
            primaryCircle.AddTalisman(new Talisman(new Element(ElementType.Earth, 0.9), "Foundation Stone"));
            primaryCircle.AddTalisman(new Talisman(new Element(ElementType.Metal, 0.8), "Shield Core"));
            
            var secondaryCircle = new MagicCircle("Secondary Attack Circle", 4.0);
            secondaryCircle.AddTalisman(new Talisman(new Element(ElementType.Fire, 0.9), "Flame Catalyst"));
            secondaryCircle.AddTalisman(new Talisman(new Element(ElementType.Lightning, 0.8), "Storm Focus"));
            
            var supportCircle = new MagicCircle("Support Enhancement Circle", 3.0);
            supportCircle.AddTalisman(new Talisman(new Element(ElementType.Wood, 0.7), "Growth Enhancer"));
            supportCircle.AddTalisman(new Talisman(new Element(ElementType.Water, 0.7), "Flow Regulator"));

            return (primaryCircle, secondaryCircle, supportCircle);
        }

        private static void ShowNetworkConnections((MagicCircle primary, MagicCircle secondary, MagicCircle support) circles)
        {
            Console.WriteLine("🔗 Creating Network Connections:");
            var connection1 = circles.primary.ConnectTo(circles.secondary, ConnectionType.Flow);
            var connection2 = circles.secondary.ConnectTo(circles.support, ConnectionType.Trigger);
            var connection3 = circles.support.ConnectTo(circles.primary, ConnectionType.Resonance);
            
            Console.WriteLine($"  ✓ Primary → Secondary: {connection1.Type} connection");
            Console.WriteLine($"  ✓ Secondary → Support: {connection2.Type} connection");
            Console.WriteLine($"  ✓ Support → Primary: {connection3.Type} connection");
        }

        private static void ShowNestedComposition((MagicCircle primary, MagicCircle secondary, MagicCircle support) circles)
        {
            Console.WriteLine("🏗️ Nested Circle Composition:");
            var masterCircle = new MagicCircle("Master Composition Circle", 10.0);
            masterCircle.NestCircle(circles.primary);
            masterCircle.NestCircle(circles.secondary);
            
            Console.WriteLine($"  Master Circle contains {masterCircle.NestedCircles.Count} nested circles");
            Console.WriteLine($"  Total complexity score: {masterCircle.ComplexityScore:F2}");
            Console.WriteLine($"  Estimated casting time: {masterCircle.CastingTime:F2}s");
            Console.WriteLine();
            
            Console.WriteLine("📊 Composition Analysis:");
            AnalyzeComposition(masterCircle);
        }

        private static void AnalyzeComposition(MagicCircle composition)
        {
            Console.WriteLine($"  Composition Type: {composition.CompositionType}");
            Console.WriteLine($"  Total Power Output: {composition.PowerOutput:F2}");
            Console.WriteLine($"  Overall Stability: {composition.Stability:F2}");
            Console.WriteLine($"  Efficiency Rating: {composition.Efficiency:F2}");
            Console.WriteLine($"  Network Connections: {composition.Connections.Count}");
            Console.WriteLine($"  Nested Circles: {composition.NestedCircles.Count}");
            Console.WriteLine();
        }
    }
}
