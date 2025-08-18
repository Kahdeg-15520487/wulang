using System;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Demonstrates the elemental system including Wu Xing base elements, derived elements, and relationships
    /// </summary>
    public static class ElementalSystemDemo
    {
        public static void Run()
        {
            Console.WriteLine("🔥 ELEMENTAL SYSTEM DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("📚 Wu Xing Foundation + Derived Elements:");
            Console.WriteLine();
            
            ShowBaseElements();
            Console.WriteLine();
            ShowDerivedElements();
            Console.WriteLine();
            ShowElementRelationships();
            Console.WriteLine();
            ShowDerivedElementCreation();
        }

        private static void ShowBaseElements()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🏛️ BASE ELEMENTS (Wu Xing 五行):");
            Console.ResetColor();
            
            var baseElements = new[]
            {
                new Element(ElementType.Water, 1.0),
                new Element(ElementType.Fire, 1.0),
                new Element(ElementType.Earth, 1.0),
                new Element(ElementType.Metal, 1.0),
                new Element(ElementType.Wood, 1.0)
            };

            foreach (var element in baseElements)
            {
                Console.ForegroundColor = element.Color;
                Console.WriteLine($"  {element.ChineseName} {element.Name} - {GetElementRole(element.Type)}");
            }
            Console.ResetColor();
        }

        private static void ShowDerivedElements()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("⚡ DERIVED ELEMENTS (Combinations):");
            Console.ResetColor();
            
            var derivedElements = new[]
            {
                new Element(ElementType.Lightning, 1.0),
                new Element(ElementType.Wind, 1.0),
                new Element(ElementType.Light, 1.0),
                new Element(ElementType.Dark, 1.0),
                new Element(ElementType.Forge, 1.0),
                new Element(ElementType.Chaos, 1.0),
                new Element(ElementType.Void, 1.0)
            };
            
            foreach (var element in derivedElements)
            {
                Console.ForegroundColor = element.Color;
                var sources = Element.GetSourceElements(element.Type);
                var sourceText = sources.Item1.HasValue && sources.Item2.HasValue 
                    ? $" ({sources.Item1} + {sources.Item2})"
                    : element.Type == ElementType.Chaos ? " (All 5 base elements)"
                    : " (Absence of elements)";
                Console.WriteLine($"  {element.ChineseName} {element.Name}{sourceText} - {GetElementRole(element.Type)}");
            }
            Console.ResetColor();
        }

        private static void ShowElementRelationships()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔄 ELEMENTAL RELATIONSHIPS:");
            Console.ResetColor();
            
            Console.WriteLine();
            Console.WriteLine("Generative Cycle (生 - Growth):");
            ShowElementRelation(ElementType.Water, ElementType.Wood, "Water nourishes Wood");
            ShowElementRelation(ElementType.Wood, ElementType.Fire, "Wood feeds Fire");
            ShowElementRelation(ElementType.Fire, ElementType.Earth, "Fire creates Earth (ash)");
            ShowElementRelation(ElementType.Earth, ElementType.Metal, "Earth produces Metal ore");
            ShowElementRelation(ElementType.Metal, ElementType.Water, "Metal collects Water");
            
            Console.WriteLine();
            Console.WriteLine("Destructive Cycle (克 - Control):");
            ShowElementRelation(ElementType.Water, ElementType.Fire, "Water extinguishes Fire");
            ShowElementRelation(ElementType.Fire, ElementType.Metal, "Fire melts Metal");
            ShowElementRelation(ElementType.Metal, ElementType.Wood, "Metal cuts Wood");
            ShowElementRelation(ElementType.Wood, ElementType.Earth, "Wood depletes Earth");
            ShowElementRelation(ElementType.Earth, ElementType.Water, "Earth absorbs Water");
            
            Console.WriteLine();
            Console.WriteLine("Special Derived Relationships:");
            ShowElementRelation(ElementType.Lightning, ElementType.Dark, "Lightning illuminates darkness");
            ShowElementRelation(ElementType.Light, ElementType.Dark, "Light banishes darkness");
            ShowElementRelation(ElementType.Wind, ElementType.Earth, "Wind erodes earth");
            ShowElementRelation(ElementType.Chaos, ElementType.Void, "Chaos vs Void (mutual destruction)");
        }

        private static void ShowDerivedElementCreation()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("⚗️ DERIVED ELEMENT CREATION:");
            Console.ResetColor();
            
            var combinations = new[]
            {
                (ElementType.Fire, ElementType.Metal, "Lightning"),
                (ElementType.Wood, ElementType.Water, "Wind"),
                (ElementType.Fire, ElementType.Wood, "Light"),
                (ElementType.Earth, ElementType.Water, "Dark"),
                (ElementType.Metal, ElementType.Wood, "Forge")
            };
            
            foreach (var (elem1, elem2, result) in combinations)
            {
                var derived = Element.TryCreateDerivedElement(elem1, elem2);
                if (derived.HasValue)
                {
                    Console.WriteLine($"  {elem1} + {elem2} → {derived.Value} ({result})");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("💡 Special Cases:");
            Console.WriteLine("  • Chaos: Requires all 5 base elements simultaneously");
            Console.WriteLine("  • Void: Represents absence of elements (perfect balance)");
        }

        public static void ShowElementRelation(ElementType source, ElementType target, string description)
        {
            var sourceElement = new Element(source, 1.0);
            var targetElement = new Element(target, 1.0);
            
            Console.ForegroundColor = sourceElement.Color;
            Console.Write($"  {sourceElement.ChineseName}{sourceElement.Name}");
            Console.ResetColor();
            Console.Write(" → ");
            Console.ForegroundColor = targetElement.Color;
            Console.Write($"{targetElement.ChineseName}{targetElement.Name}");
            Console.ResetColor();
            Console.WriteLine($" : {description}");
        }

        private static string GetElementRole(ElementType type)
        {
            return type switch
            {
                ElementType.Water => "Flow control, adaptation",
                ElementType.Fire => "Action, transformation", 
                ElementType.Earth => "Stability, foundation",
                ElementType.Metal => "Logic, precision",
                ElementType.Wood => "Growth, life force",
                ElementType.Lightning => "Sudden directed energy",
                ElementType.Wind => "Adaptive movement",
                ElementType.Light => "Illumination, revelation",
                ElementType.Dark => "Concealment, mystery",
                ElementType.Forge => "Creation, crafting",
                ElementType.Chaos => "Unpredictable potential",
                ElementType.Void => "Balance, neutralization",
                _ => "Unknown"
            };
        }
    }
}
