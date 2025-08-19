using System;
using System.IO;
using System.Linq;
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
                Console.WriteLine("  7. 💾 Serialization System");
                Console.WriteLine("  8. 📄 JSON File Parser");
                Console.WriteLine("  9. 🌀 CNF File Parser");
                Console.WriteLine(" 10. 🎓 Interactive Workshop");
                Console.WriteLine(" 11. Exit");
                Console.WriteLine();
                Console.Write("Choose a demonstration (1-11): ");

                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Clear();
                    Console.WriteLine("🌟 WULANG SPELLCRAFT DEMONSTRATION 🌟");
                    Console.WriteLine(new string('═', 45));
                    Console.WriteLine();
                    Console.WriteLine("❌ Invalid choice. Please enter 1-11.");
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
                        SerializationSystemDemo.Run();
                        break;
                    case "8":
                        JsonFileParserDemo.Run();
                        break;
                    case "9":
                        CnfFileParserDemo.Run();
                        break;
                    case "10":
                        InteractiveWorkshop.Run();
                        break;
                    case "11":
                        Console.WriteLine("🌟 Thank you for exploring Wu Lang Spellcraft! 🌟");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice. Please enter 1-11.");
                        break;
                }

                if (input.Trim() != "11")
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
                var isMultiCircle = WuLangSpellcraft.Core.Serialization.SpellSerializer.IsMultiCircleCnf(cnf);
                
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
                var circle = WuLangSpellcraft.Core.Serialization.SpellSerializer.DeserializeCircleFromCnf(cnf);
                
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
                    Console.WriteLine($"  Element: {circle.CenterTalisman.PrimaryElement}");
                    Console.WriteLine($"  Power Level: {circle.CenterTalisman.PowerLevel}");
                    Console.WriteLine();
                }

                if (circle.Talismans.Any())
                {
                    Console.WriteLine("🧿 Ring Talismans:");
                    for (int i = 0; i < circle.Talismans.Count; i++)
                    {
                        var talisman = circle.Talismans[i];
                        Console.WriteLine($"  {i + 1}. {talisman.Name} ({talisman.PrimaryElement}) - Power: {talisman.PowerLevel}");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("💾 Serialization Results:");
                Console.WriteLine($"  CNF: {WuLangSpellcraft.Core.Serialization.SpellSerializer.SerializeCircleToCnf(circle)}");
                
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
                var formation = WuLangSpellcraft.Core.Serialization.SpellSerializer.DeserializeFormationFromCnf(cnf);
                
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
                        Console.WriteLine($"    Elements: {string.Join(", ", circle.Talismans.Select(t => t.PrimaryElement.ToString()))}");
                    }
                    Console.WriteLine();
                }

                if (formation.Connections.Any())
                {
                    Console.WriteLine("🔗 Connections:");
                    foreach (var connection in formation.Connections)
                    {
                        Console.WriteLine($"  {connection.SourceId} --{connection.Type}--> {connection.TargetId} (Strength: {connection.Strength:F2})");
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
            Console.WriteLine("🌟 WULANG SPELLCRAFT CNF PROCESSOR 🌟");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  WuLangSpellcraft.Demo [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --cnf <cnf-string>    Process a CNF string directly");
            Console.WriteLine("  --file <file-path>    Process a CNF file");
            Console.WriteLine("  --help, -h            Show this help message");
            Console.WriteLine();
            Console.WriteLine("Pipe Support:");
            Console.WriteLine("  echo \"C3 Fire Water\" | WuLangSpellcraft.Demo");
            Console.WriteLine("  cat spell.cnf | WuLangSpellcraft.Demo");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  WuLangSpellcraft.Demo --cnf \"C3 Fire Water@center\"");
            Console.WriteLine("  WuLangSpellcraft.Demo --file myspell.cnf");
            Console.WriteLine("  echo \"main:C3 Fire Water@center\" | WuLangSpellcraft.Demo");
            Console.WriteLine();
            Console.WriteLine("Supported CNF Features:");
            Console.WriteLine("  • Single circles: C3 Fire Water");
            Console.WriteLine("  • Center talismans: C3 Fire@center Water");
            Console.WriteLine("  • Multi-circle formations: main:C3 Fire support:C2 Water main~support");
            Console.WriteLine("  • Connections: id1~id2 (bidirectional), id1~>id2 (directed)");
        }
    }
}
