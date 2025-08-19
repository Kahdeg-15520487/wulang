using System;
using System.IO;
using System.Linq;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;
using WuLangSpellcraft.Demo.Utilities;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    public static class CnfFileParserDemo
    {
        public static void Run()
        {
            Console.WriteLine("🌀 CNF FILE PARSER DEMONSTRATION");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            Console.WriteLine("🔍 CNF Parser Features:");
            Console.WriteLine("  • Parse Circle Notation Format (CNF) strings");
            Console.WriteLine("  • Support for all element types and power levels");
            Console.WriteLine("  • Handle compact and verbose CNF syntax");
            Console.WriteLine("  • Visual representation of parsed circles");
            Console.WriteLine("  • Batch processing of multiple CNF entries");
            Console.WriteLine();
            
            ShowCnfParsingExamples();
            ShowInteractiveCnfParser();
        }

        private static void ShowCnfParsingExamples()
        {
            Console.WriteLine("📚 CNF PARSING EXAMPLES");
            Console.WriteLine(new string('─', 40));
            Console.WriteLine();
            
            var examples = new[]
            {
                ("Basic Elements", "C3 FWE"),
                ("Power Levels", "C5 F2.5 W1.2 L0.8"),
                ("Named IDs", "C4 F:core W:shield E:ground"),
                ("Mixed Format", "C6 F2:flame W E1.5:earth"),
                ("Compact Notation", "C7 FWEMOLIN"),
                ("All Elements", "C12 FWEMOLINDGCV"),
                ("Chaos Formation", "C2 CCC"),
                ("Void Meditation", "C1 V"),
                ("Decimal Radius", "C2.5 F2:flame W1.5:water"),
                ("Mathematical", "C7 F3.14:pi W2.71:euler E1.41:sqrt2")
            };
            
            foreach (var (description, cnf) in examples)
            {
                Console.WriteLine($"📝 {description}: {cnf}");
                
                try
                {
                    var circle = SpellSerializer.DeserializeCircleFromCnf(cnf);
                    Console.WriteLine($"   ✅ Parsed successfully:");
                    Console.WriteLine($"      Radius: {circle.Radius}");
                    Console.WriteLine($"      Talismans: {circle.Talismans.Count}");
                    
                    // Show talisman details
                    foreach (var talisman in circle.Talismans)
                    {
                        var powerStr = Math.Abs(talisman.PowerLevel - 1.0) < 0.001 ? "" : $" (P:{talisman.PowerLevel:F1})";
                        var idStr = string.IsNullOrEmpty(talisman.Name) || talisman.Name.Length > 10 ? "" : $" '{talisman.Name}'";
                        Console.WriteLine($"         {GetElementSymbol(talisman.PrimaryElement.Type)}{powerStr}{idStr}");
                    }
                    
                    // Show visual representation for smaller circles
                    if (circle.Radius <= 8)
                    {
                        Console.WriteLine("      Visual:");
                        CircleVisualizer.RenderCircleCompact(circle);
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ Parse error: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        private static void ShowInteractiveCnfParser()
        {
            Console.WriteLine("🎯 INTERACTIVE CNF PARSER");
            Console.WriteLine(new string('─', 40));
            Console.WriteLine();
            
            Console.WriteLine("📖 CNF FORMAT GUIDE:");
            Console.WriteLine("  • C<radius> - Circle with specified radius");
            Console.WriteLine("  • Elements: F(Fire) W(Water) E(Earth) M(Metal) O(Wood)");
            Console.WriteLine("  •           L(Lightning) N(Wind) I(Light) D(Dark) G(Forge)");
            Console.WriteLine("  •           C(Chaos) V(Void)");
            Console.WriteLine("  • Power: F2.5 (element with power level)");
            Console.WriteLine("  • Named: F:core (element with ID)");
            Console.WriteLine("  • Combined: F2.5:flame (power + ID)");
            Console.WriteLine("  • Compact: FWEMO (no spaces)");
            Console.WriteLine();
            
            Console.WriteLine("💡 Try these examples:");
            Console.WriteLine("   C3 FWE");
            Console.WriteLine("   C5 F2.5 W1.2 L0.8");
            Console.WriteLine("   C4 F:core W:shield E:ground");
            Console.WriteLine("   C7 FWEMOLIN");
            Console.WriteLine();
            
            Console.WriteLine("Enter CNF strings to parse (or 'quit' to exit):");
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("CNF> ");
                var input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "quit")
                {
                    break;
                }
                
                try
                {
                    var circle = SpellSerializer.DeserializeCircleFromCnf(input);
                    
                    Console.WriteLine("✅ Successfully parsed CNF:");
                    Console.WriteLine($"   Circle: {circle.Name}");
                    Console.WriteLine($"   Radius: {circle.Radius}");
                    Console.WriteLine($"   Talismans: {circle.Talismans.Count}");
                    
                    // Show detailed talisman information
                    if (circle.Talismans.Count > 0)
                    {
                        Console.WriteLine("   Elements:");
                        foreach (var talisman in circle.Talismans)
                        {
                            var symbol = GetElementSymbol(talisman.PrimaryElement.Type);
                            var name = talisman.PrimaryElement.Type.ToString();
                            var powerStr = $"Power: {talisman.PowerLevel:F2}";
                            var idStr = string.IsNullOrEmpty(talisman.Name) ? "" : $" '{talisman.Name}'";
                            
                            Console.WriteLine($"      {symbol} ({name}): {powerStr}{idStr}");
                        }
                    }
                    
                    // Show visual representation
                    if (circle.Radius <= 10)
                    {
                        Console.WriteLine();
                        Console.WriteLine("📝 Visual Representation:");
                        CircleVisualizer.RenderCnfExample(input);
                    }
                    else
                    {
                        Console.WriteLine("   (Circle too large for visualization)");
                    }
                    
                    // Show round-trip CNF
                    var roundTripCnf = SpellSerializer.SerializeCircleToCnf(circle);
                    Console.WriteLine($"   Round-trip CNF: {roundTripCnf}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Parse error: {ex.Message}");
                    
                    // Provide helpful suggestions
                    if (input.Length > 0)
                    {
                        if (!input.StartsWith("C", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("💡 Hint: CNF must start with 'C' followed by radius (e.g., 'C3')");
                        }
                        else if (!input.Contains(' ') && input.Length > 3)
                        {
                            Console.WriteLine("💡 Hint: For readability, try adding spaces (e.g., 'C3 F W E' instead of 'C3FWE')");
                        }
                        else
                        {
                            Console.WriteLine("💡 Hint: Check element symbols and format. Valid elements: F W E M O L N I D G C V");
                        }
                    }
                }
                
                Console.WriteLine();
            }
        }

        private static string GetElementSymbol(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Fire => "F",
                ElementType.Water => "W",
                ElementType.Earth => "E",
                ElementType.Metal => "M",
                ElementType.Wood => "O",
                ElementType.Lightning => "L",
                ElementType.Wind => "N",
                ElementType.Light => "I",
                ElementType.Dark => "D",
                ElementType.Forge => "G",
                ElementType.Chaos => "C",
                ElementType.Void => "V",
                _ => "?"
            };
        }
    }
}
