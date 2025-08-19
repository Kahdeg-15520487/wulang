using System;
using System.IO;
using System.Linq;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Serialization;
using WuLangSpellcraft.Demo.Demonstrations;
using WuLangSpellcraft.Demo.Interactive;

namespace WuLangSpellcraft
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check for command-line arguments
            if (args.Length > 0)
            {
                ProcessCommandLineArgs(args);
                return;
            }

            // Check for piped input
            if (Console.IsInputRedirected)
            {
                ProcessPipedInput();
                return;
            }

            // Default interactive mode
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
                Console.WriteLine("  7. � Formation Hierarchy (NEW)");
                Console.WriteLine("  8. �💾 Serialization System");
                Console.WriteLine("  9. 📄 JSON File Parser");
                Console.WriteLine(" 10. 🌀 CNF File Parser");
                Console.WriteLine(" 11. 🎓 Interactive Workshop");
                Console.WriteLine(" 12. Exit");
                Console.WriteLine();
                Console.Write("Choose a demonstration (1-12): ");

                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Clear();
                    Console.WriteLine("🌟 WULANG SPELLCRAFT DEMONSTRATION 🌟");
                    Console.WriteLine(new string('═', 45));
                    Console.WriteLine();
                    Console.WriteLine("❌ Invalid choice. Please enter 1-12.");
                    continue;
                }
                
                Console.Clear();
                
                // Show header again after clear
                Console.WriteLine("🌟 WULANG SPELLCRAFT DEMONSTRATION 🌟");
                Console.WriteLine(new string('═', 45));
                Console.WriteLine();

                switch (input.Trim())
                {
                    case "1":
                        ElementalSystemDemo.Run();
                        break;
                    case "2":
                        TalismanSystemDemo.Run();
                        break;
                    case "3":
                        StabilityCastingDemo.Run();
                        break;
                    case "4":
                        MagicCircleDemo.Run();
                        break;
                    case "5":
                        CompositionSystemDemo.Run();
                        break;
                    case "6":
                        ArtifactSystemDemo.Run();
                        break;
                    case "7":
                        FormationHierarchyDemo.RunDemo();
                        break;
                    case "8":
                        SerializationSystemDemo.Run();
                        break;
                    case "9":
                        JsonFileParserDemo.Run();
                        break;
                    case "10":
                        CnfFileParserDemo.Run();
                        break;
                    case "11":
                        InteractiveWorkshop.Run();
                        break;
                    case "12":
                        Console.WriteLine("🌟 Thank you for exploring Wu Lang Spellcraft! 🌟");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please enter 1-12.");
                        break;
                }

                if (input.Trim() != "12")
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to main menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void ProcessCommandLineArgs(string[] args)
        {
            if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h"))
            {
                ShowHelp();
                return;
            }

            if (args.Length == 1 && args[0] == "--list")
            {
                ShowDemoList();
                return;
            }

            if (args.Length == 2 && args[0] == "--demo")
            {
                if (int.TryParse(args[1], out int demoNumber) && demoNumber >= 1 && demoNumber <= 11)
                {
                    RunSpecificDemo(demoNumber);
                    return;
                }
                else
                {
                    Console.WriteLine($"❌ Invalid demo number: {args[1]}. Use --list to see available demos.");
                    Environment.Exit(1);
                }
            }

            if (args.Length == 2 && args[0] == "--cnf")
            {
                ProcessCnfString(args[1]);
                return;
            }

            if (args.Length == 2 && args[0] == "--file")
            {
                ProcessCnfFile(args[1]);
                return;
            }

            Console.WriteLine("❌ Invalid arguments. Use --help for usage information.");
            Environment.Exit(1);
        }

        private static void ProcessPipedInput()
        {
            Console.WriteLine("📥 Processing piped CNF input...");
            Console.WriteLine();

            var cnfInput = Console.In.ReadToEnd().Trim();
            if (string.IsNullOrWhiteSpace(cnfInput))
            {
                Console.WriteLine("❌ No input received from pipe.");
                Environment.Exit(1);
            }

            ProcessCnfString(cnfInput);
        }

        private static void ProcessCnfString(string cnf)
        {
            try
            {
                Console.WriteLine("🔍 Analyzing CNF Input:");
                Console.WriteLine($"Input: {cnf}");
                Console.WriteLine();

                // Detect if it's single-circle or multi-circle CNF
                var isMultiCircle = WuLangSpellcraft.Serialization.SpellSerializer.IsMultiCircleCnf(cnf);
                
                if (isMultiCircle)
                {
                    Console.WriteLine("🌀 Detected: Multi-Circle Formation");
                    ProcessMultiCircleCnf(cnf);
                }
                else
                {
                    Console.WriteLine("⚪ Detected: Single Magic Circle");
                    ProcessSingleCircleCnf(cnf);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing CNF: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static void ProcessSingleCircleCnf(string cnf)
        {
            try
            {
                var circle = WuLangSpellcraft.Serialization.SpellSerializer.DeserializeCircleFromCnf(cnf);
                
                Console.WriteLine("📊 Circle Analysis:");
                Console.WriteLine($"  Radius: {circle.Radius}");
                Console.WriteLine($"  Ring Talismans: {circle.Talismans.Count}");
                Console.WriteLine($"  Center Talisman: {(circle.CenterTalisman != null ? circle.CenterTalisman.Name : "None")}");
                Console.WriteLine($"  Stability: {circle.Stability:F2}");
                Console.WriteLine($"  Power Output: {circle.PowerOutput:F2}");
                Console.WriteLine();

                if (circle.CenterTalisman != null)
                {
                    Console.WriteLine("🎯 Center Talisman:");
                    Console.WriteLine($"  Name: {circle.CenterTalisman.Name}");
                    var centerElementDisplay = GetElementDisplay(circle.CenterTalisman.PrimaryElement);
                    Console.WriteLine($"  Element: {centerElementDisplay}");
                    Console.WriteLine($"  Power Level: {circle.CenterTalisman.PowerLevel}");
                    Console.WriteLine();
                }

                if (circle.Talismans.Any())
                {
                    Console.WriteLine("🧿 Ring Talismans:");
                    for (int i = 0; i < circle.Talismans.Count; i++)
                    {
                        var talisman = circle.Talismans[i];
                        var elementDisplay = GetElementDisplay(talisman.PrimaryElement);
                        Console.WriteLine($"  {i + 1}. {talisman.Name} ({elementDisplay}) - Power: {talisman.PowerLevel}");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("💾 Serialization Results:");
                Console.WriteLine($"  CNF: {WuLangSpellcraft.Serialization.SpellSerializer.SerializeCircleToCnf(circle)}");
                
                Console.WriteLine();
                Console.WriteLine("🎨 Circle Visualization:");
                WuLangSpellcraft.Demo.Utilities.CircleVisualizer.RenderCircle(circle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error parsing single circle CNF: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static void ProcessMultiCircleCnf(string cnf)
        {
            try
            {
                var formation = WuLangSpellcraft.Serialization.SpellSerializer.DeserializeFormationFromCnf(cnf);
                
                Console.WriteLine("📊 Formation Analysis:");
                Console.WriteLine($"  Total Circles: {formation.Circles.Count}");
                Console.WriteLine($"  Total Connections: {formation.Connections.Count}");
                Console.WriteLine();

                Console.WriteLine("⚪ Circles in Formation:");
                foreach (var (circleId, circle) in formation.Circles)
                {
                    Console.WriteLine($"  🔹 {circleId}:");
                    Console.WriteLine($"    Radius: {circle.Radius}");
                    Console.WriteLine($"    Ring Talismans: {circle.Talismans.Count}");
                    Console.WriteLine($"    Center Talisman: {(circle.CenterTalisman != null ? circle.CenterTalisman.Name : "None")}");
                    Console.WriteLine($"    Stability: {circle.Stability:F2}");
                    Console.WriteLine($"    Power Output: {circle.PowerOutput:F2}");
                    
                    if (circle.Talismans.Any())
                    {
                        var elementsDisplay = string.Join(", ", circle.Talismans.Select(t => GetElementDisplay(t.PrimaryElement)));
                        Console.WriteLine($"    Elements: {elementsDisplay}");
                    }
                    Console.WriteLine();
                }

                if (formation.Connections.Any())
                {
                    Console.WriteLine("🔗 Connections:");
                    foreach (var connection in formation.Connections)
                    {
                        var connectionDisplay = GetConnectionDisplay(connection);
                        Console.WriteLine($"  {connectionDisplay}");
                    }
                    Console.WriteLine();
                }

                // Calculate total formation statistics
                var totalStability = formation.Circles.Values.Sum(c => c.Stability) / formation.Circles.Count;
                var totalPower = formation.Circles.Values.Sum(c => c.PowerOutput);
                
                Console.WriteLine("📈 Formation Statistics:");
                Console.WriteLine($"  Average Stability: {totalStability:F2}");
                Console.WriteLine($"  Total Power: {totalPower:F2}");
                Console.WriteLine($"  Connection Density: {(double)formation.Connections.Count / formation.Circles.Count:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error parsing multi-circle CNF: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static void ProcessCnfFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ File not found: {filePath}");
                    Environment.Exit(1);
                }

                var cnfContent = File.ReadAllText(filePath).Trim();
                Console.WriteLine($"📁 Processing CNF file: {filePath}");
                Console.WriteLine();
                
                ProcessCnfString(cnfContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error reading file: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("🌟 WULANG SPELLCRAFT DEMO 🌟");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  WuLangSpellcraft.Demo [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --demo <number>       Run a specific demo (1-11)");
            Console.WriteLine("  --list                List all available demos");
            Console.WriteLine("  --cnf <cnf-string>    Process a CNF string directly");
            Console.WriteLine("  --file <file-path>    Process a CNF file");
            Console.WriteLine("  --help, -h            Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  WuLangSpellcraft.Demo --demo 7                    # Run Formation Hierarchy demo");
            Console.WriteLine("  WuLangSpellcraft.Demo --list                      # List all demos");
            Console.WriteLine("  WuLangSpellcraft.Demo --cnf \"C3 Fire Water@center\" # Process CNF");
            Console.WriteLine("  echo \"main:C3 Fire support:C2 Water main~support\" | WuLangSpellcraft.Demo");
            Console.WriteLine();
            Console.WriteLine("Pipe Support:");
            Console.WriteLine("  echo \"C3 Fire Water\" | WuLangSpellcraft.Demo");
            Console.WriteLine("  cat spell.cnf | WuLangSpellcraft.Demo");
            Console.WriteLine();
            Console.WriteLine("Supported CNF Features:");
            Console.WriteLine("  • Single circles: C3 Fire Water");
            Console.WriteLine("  • Center talismans: C3 Fire@center Water");
            Console.WriteLine("  • Multi-circle formations: main:C3 Fire support:C2 Water main~support");
            Console.WriteLine("  • Connections: id1~id2 (bidirectional), id1~>id2 (directed)");
        }

        private static void ShowDemoList()
        {
            Console.WriteLine("🌟 AVAILABLE DEMONSTRATIONS 🌟");
            Console.WriteLine();
            Console.WriteLine("  1. 🧪 Elemental System");
            Console.WriteLine("  2. 🎯 Talisman System");
            Console.WriteLine("  3. ⚖️ Stability-Based Casting");
            Console.WriteLine("  4. 🔮 Magic Circles");
            Console.WriteLine("  5. 🏗️ Composition System");
            Console.WriteLine("  6. 🏺 Artifact System");
            Console.WriteLine("  7. 📋 Formation Hierarchy (NEW)");
            Console.WriteLine("  8. 💾 Serialization System");
            Console.WriteLine("  9. 📄 JSON File Parser");
            Console.WriteLine(" 10. 🌀 CNF File Parser");
            Console.WriteLine(" 11. 🎓 Interactive Workshop");
            Console.WriteLine();
            Console.WriteLine("Usage: WuLangSpellcraft.Demo --demo <number>");
        }

        private static void RunSpecificDemo(int demoNumber)
        {
            Console.WriteLine("🌟 WULANG SPELLCRAFT DEMONSTRATION 🌟");
            Console.WriteLine(new string('═', 45));
            Console.WriteLine();

            switch (demoNumber)
            {
                case 1:
                    ElementalSystemDemo.Run();
                    break;
                case 2:
                    TalismanSystemDemo.Run();
                    break;
                case 3:
                    StabilityCastingDemo.Run();
                    break;
                case 4:
                    MagicCircleDemo.Run();
                    break;
                case 5:
                    CompositionSystemDemo.Run();
                    break;
                case 6:
                    ArtifactSystemDemo.Run();
                    break;
                case 7:
                    FormationHierarchyDemo.RunDemo();
                    break;
                case 8:
                    SerializationSystemDemo.Run();
                    break;
                case 9:
                    JsonFileParserDemo.Run();
                    break;
                case 10:
                    CnfFileParserDemo.Run();
                    break;
                case 11:
                    InteractiveWorkshop.Run();
                    break;
                default:
                    Console.WriteLine($"❌ Demo {demoNumber} not found.");
                    break;
            }
        }

        private static string GetConnectionDisplay(FormationConnection connection)
        {
            return connection.Type switch
            {
                ConnectionType.Basic => $"{connection.SourceId} --Basic-- {connection.TargetId} (Strength: {connection.Strength:F2})",
                ConnectionType.Strong => $"{connection.SourceId} ==Strong== {connection.TargetId} (Strength: {connection.Strength:F2})",
                ConnectionType.Harmonic => $"{connection.SourceId} ~~Harmonic~~ {connection.TargetId} (Strength: {connection.Strength:F2})",
                ConnectionType.Unstable => $"{connection.SourceId} ~=Unstable=~ {connection.TargetId} (Strength: {connection.Strength:F2})",
                ConnectionType.Directional => $"{connection.SourceId} -->Directional--> {connection.TargetId} (Strength: {connection.Strength:F2})",
                ConnectionType.Bidirectional => $"{connection.SourceId} <--Bidirectional--> {connection.TargetId} (Strength: {connection.Strength:F2})",
                _ => $"{connection.SourceId} --{connection.Type}-- {connection.TargetId} (Strength: {connection.Strength:F2})"
            };
        }

        private static string GetElementDisplay(Element element)
        {
            var stateSymbol = element.State switch
            {
                ElementState.Unstable => "?",
                ElementState.Damaged => "!",
                ElementState.Resonating => "~",
                _ => ""
            };

            return $"{element.ChineseName} {element.Name}{stateSymbol} (Energy: {element.Energy})";
        }
    }
}
