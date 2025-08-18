using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates the talisman system including creation, interactions, and secondary elements
    /// </summary>
    public static class TalismanSystemDemo
    {
        public static void Run()
        {
            Console.WriteLine("🧿 TALISMAN SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🎯 Creating Talismans with Different Elements:");
            Console.WriteLine();
            
            ShowSingleElementTalismans();
            Console.WriteLine();
            ShowMultiElementTalisman();
            Console.WriteLine();
            ShowTalismanInteractions();
        }

        private static void ShowSingleElementTalismans()
        {
            var fireTalisman = new Talisman(new Element(ElementType.Fire, 0.8), "Fire Protection Talisman");
            var waterTalisman = new Talisman(new Element(ElementType.Water, 0.9), "Water Flow Talisman");
            
            DisplayTalisman("🔥 Fire Protection Talisman", fireTalisman);
            DisplayTalisman("💧 Water Flow Talisman", waterTalisman);
        }

        private static void ShowMultiElementTalisman()
        {
            Console.WriteLine("⚗️ Multi-Element Talisman:");
            
            var lightningTalisman = new Talisman(new Element(ElementType.Fire, 0.6), "Lightning Strike Talisman");
            lightningTalisman.AddSecondaryElement(new Element(ElementType.Metal, 0.7));
            
            DisplayTalisman("⚡ Lightning Strike Talisman", lightningTalisman);
        }

        private static void ShowTalismanInteractions()
        {
            Console.WriteLine("🔮 Talisman Interactions:");
            
            var fireTalisman = new Talisman(new Element(ElementType.Fire, 0.8), "Fire Talisman");
            var waterTalisman = new Talisman(new Element(ElementType.Water, 0.9), "Water Talisman");
            var lightningTalisman = new Talisman(new Element(ElementType.Fire, 0.6), "Lightning Talisman");
            lightningTalisman.AddSecondaryElement(new Element(ElementType.Metal, 0.7));
            
            DemonstrateInteraction(fireTalisman, waterTalisman);
            DemonstrateInteraction(fireTalisman, lightningTalisman);
        }

        public static void DisplayTalisman(string name, Talisman talisman)
        {
            Console.WriteLine($"{name}:");
            Console.WriteLine($"  ID: {talisman.Id:N}");
            Console.WriteLine($"  Power Level: {talisman.PowerLevel:F2}");
            Console.WriteLine($"  Stability: {talisman.Stability:F2}");
            Console.WriteLine($"  Primary Element:");
            Console.ForegroundColor = talisman.PrimaryElement.Color;
            Console.WriteLine($"    {talisman.PrimaryElement.ChineseName}{talisman.PrimaryElement.Name} ({talisman.PrimaryElement.Energy:F2})");
            Console.ResetColor();
            
            if (talisman.SecondaryElements.Any())
            {
                Console.WriteLine($"  Secondary Elements:");
                foreach (var element in talisman.SecondaryElements)
                {
                    Console.ForegroundColor = element.Color;
                    Console.WriteLine($"    {element.ChineseName}{element.Name} ({element.Energy:F2})");
                }
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        private static void DemonstrateInteraction(Talisman talisman1, Talisman talisman2)
        {
            var interaction = talisman1.GetInteractionWith(talisman2);
            Console.WriteLine($"Interaction: {talisman1.Name} ↔ {talisman2.Name}");
            Console.WriteLine($"  Relation: {interaction.Relation}");
            Console.WriteLine($"  Resonance: {interaction.Resonance:F2}");
            Console.WriteLine($"  Energy Flow: {interaction.EnergyFlow:F2}");
            Console.WriteLine($"  Stable: {(interaction.IsStable ? "✓" : "✗")}");
            Console.WriteLine();
        }
    }
}
