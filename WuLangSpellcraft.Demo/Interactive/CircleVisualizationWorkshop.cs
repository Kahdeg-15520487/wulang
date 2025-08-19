using System;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Serialization;
using WuLangSpellcraft.Demo.Utilities;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive workshop for visualizing magic circles from CNF notation
    /// </summary>
    public static class CircleVisualizationWorkshop
    {
        public static void Run()
        {
            Console.WriteLine("🎨 CIRCLE VISUALIZATION WORKSHOP");
            Console.WriteLine(new string('═', 50));
            Console.WriteLine();
            
            CircleVisualizer.ShowLegend();
            
            Console.WriteLine("🎯 Interactive CNF Visualization");
            Console.WriteLine("Enter CNF notation to see visual representation, or 'examples' for presets, 'quit' to exit:");
            Console.WriteLine();
            
            ShowExampleGallery();
            
            while (true)
            {
                Console.Write("CNF> ");
                var input = Console.ReadLine()?.Trim();
                
                if (string.IsNullOrEmpty(input))
                    continue;
                
                if (input.ToLowerInvariant() == "quit" || input.ToLowerInvariant() == "exit")
                    break;
                
                if (input.ToLowerInvariant() == "examples")
                {
                    ShowExampleGallery();
                    continue;
                }
                
                if (input.ToLowerInvariant() == "help")
                {
                    ShowHelp();
                    continue;
                }
                
                try
                {
                    Console.WriteLine();
                    CircleVisualizer.RenderCnfExample(input);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
            
            Console.WriteLine("Thanks for using the Circle Visualization Workshop! 🎨");
        }
        
        private static void ShowExampleGallery()
        {
            Console.WriteLine("📚 EXAMPLE GALLERY:");
            Console.WriteLine();
            
            var examples = new[]
            {
                ("Basic Triangle", "C3 FWE"),
                ("Wu Xing Circle", "C5 FWEMO"),
                ("Power Variance", "C4 F3W1E2M0.5"),
                ("Named Elements", "C3 F:fire W:water E:earth"),
                ("All Elements", "C12 FWEMOLINDGCV"),
                ("Lightning Storm", "C6 LLLFFF"),
                ("Elemental Balance", "C8 FWEMOLIN"),
                ("Chaos Formation", "C2 CC"),
                ("Void Meditation", "C1 V"),
                ("Mathematical", "C7 F3.14:pi W2.71:euler")
            };
            
            foreach (var (name, cnf) in examples)
            {
                Console.WriteLine($"   {name}: {cnf}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Try typing any of these CNF strings to see them visualized!");
            Console.WriteLine("Type 'help' for formatting guide or 'quit' to exit.");
            Console.WriteLine();
        }
        
        private static void ShowHelp()
        {
            Console.WriteLine("📖 CNF FORMAT HELP:");
            Console.WriteLine();
            Console.WriteLine("Basic Format: C<radius> <elements>");
            Console.WriteLine();
            Console.WriteLine("Elements:");
            Console.WriteLine("   F=Fire, W=Water, E=Earth, M=Metal, O=Wood");
            Console.WriteLine("   L=Lightning, N=Wind, I=Light, D=Dark, G=Forge");
            Console.WriteLine("   C=Chaos, V=Void");
            Console.WriteLine();
            Console.WriteLine("Power Levels:");
            Console.WriteLine("   F2.5  - Fire with 2.5 power");
            Console.WriteLine("   W0.8  - Water with 0.8 power");
            Console.WriteLine();
            Console.WriteLine("Named IDs:");
            Console.WriteLine("   F:core   - Fire named 'core'");
            Console.WriteLine("   W:shield - Water named 'shield'");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("   C3 FWE           - Triangle with Fire, Water, Earth");
            Console.WriteLine("   C5 F2W1E3        - Circle with varying power levels");
            Console.WriteLine("   C4 F:a W:b E:c   - Circle with named elements");
            Console.WriteLine("   C6 FWEMOL        - Hexagon with 6 elements");
            Console.WriteLine();
        }
    }
}
