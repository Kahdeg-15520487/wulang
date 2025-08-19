using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;

namespace WuLangSpellcraft.Demo.Utilities
{
    /// <summary>
    /// Provides ASCII art visualization for magic circles
    /// </summary>
    public static class CircleVisualizer
    {
        public static void RenderCircle(MagicCircle circle, bool showPowerLevels = true, bool showNames = false)
        {
            // Calculate consistent grid size - increase for larger circles
            var baseSize = Math.Max(15, (int)(circle.Radius * 8) + 6); // Increased multiplier and padding
            var gridSize = baseSize | 1; // Ensure odd number for proper center
            if (gridSize > 41) gridSize = 41; // Increased cap for larger circles
            
            // Create title and calculate total width needed
            var titlePart = $" {circle.Name} (R:{circle.Radius}) ";
            var contentWidth = gridSize + 2; // Grid + 2 spaces (one on each side)
            var totalWidth = Math.Max(contentWidth, titlePart.Length + 2); // Ensure title fits
            
            // Create top border
            var remainingSpace = totalWidth - titlePart.Length;
            var rightBorder = new string('─', remainingSpace);
            Console.WriteLine($"┌{titlePart}{rightBorder}┐");
            
            if (circle.Talismans.Count == 0)
            {
                var emptyLine = new string(' ', totalWidth);
                Console.WriteLine($"│{emptyLine}│");
                var emptyMsg = "  (Empty Circle)";
                var emptyPadding = new string(' ', totalWidth - emptyMsg.Length);
                Console.WriteLine($"│{emptyMsg}{emptyPadding}│");
                Console.WriteLine($"│{emptyLine}│");
                Console.WriteLine($"└{new string('─', totalWidth)}┘");
                return;
            }

            var grid = CreateGrid(gridSize);
            var centerX = gridSize / 2;
            var centerY = gridSize / 2;

            // Draw circle outline
            DrawCircleOutline(grid, centerX, centerY, circle.Radius, gridSize);

            // Draw radius indicator
            DrawRadiusIndicator(grid, centerX, centerY, circle.Radius, gridSize);

            // Place talismans
            PlaceTalismans(grid, circle, centerX, centerY, gridSize, showPowerLevels, showNames);

            // Render the grid with proper borders
            RenderGrid(grid, gridSize, totalWidth);
            
            Console.WriteLine($"└{new string('─', totalWidth)}┘");
        }

        public static void RenderCircleCompact(MagicCircle circle)
        {
            var symbols = circle.Talismans.Select(t => ElementSymbols.GetSymbol(t.PrimaryElement.Type)).ToArray();
            var cnf = SpellSerializer.SerializeCircleToCnf(circle);
            
            Console.WriteLine($"🔮 {circle.Name}");
            Console.WriteLine($"   CNF: {cnf}");
            Console.WriteLine($"   Layout: {string.Join("-", symbols)}");
            Console.WriteLine($"   Power: {circle.PowerOutput:F2}");
        }

        public static void RenderFormation(MagicCircle[] circles, string formationName = "Formation")
        {
            Console.WriteLine($"🌀 {formationName.ToUpper()} VISUALIZATION");
            Console.WriteLine(new string('═', 50));
            
            foreach (var circle in circles)
            {
                RenderCircleCompact(circle);
                Console.WriteLine();
            }
            
            // Show total formation power
            var totalPower = circles.Sum(c => c.PowerOutput);
            Console.WriteLine($"⚡ Total Formation Power: {totalPower:F2}");
            Console.WriteLine();
        }

        private static char[,] CreateGrid(int size)
        {
            var grid = new char[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    grid[y, x] = ' ';
                }
            }
            return grid;
        }

        private static void DrawCircleOutline(char[,] grid, int centerX, int centerY, double radius, int gridSize)
        {
            // More precise circle drawing with proper radius scaling
            var radiusInGrid = Math.Max(2.0, radius * 2.5); // Better radius scaling
            
            // Draw circle using Bresenham-style algorithm for better precision
            for (int angle = 0; angle < 360; angle += 8) // Smaller angle steps for smoother circle
            {
                var radians = angle * Math.PI / 180.0;
                var x = centerX + (int)Math.Round(Math.Cos(radians) * radiusInGrid);
                var y = centerY + (int)Math.Round(Math.Sin(radians) * radiusInGrid);
                
                if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
                {
                    if (grid[y, x] == ' ') // Don't overwrite talismans
                    {
                        grid[y, x] = '·';
                    }
                }
            }
        }

        private static void DrawRadiusIndicator(char[,] grid, int centerX, int centerY, double radius, int gridSize)
        {
            // Draw exactly 'radius' number of dashes to represent the logical radius
            var dashCount = (int)Math.Round(radius);
            
            // Draw horizontal line from center with exactly the right number of dashes
            for (int i = 1; i <= dashCount && centerX + (i * 2 - 1) < gridSize; i++)
            {
                int dashX = centerX + (i * 2 - 1); // Position for dash: 1, 3, 5, 7...
                if (dashX < gridSize && (grid[centerY, dashX] == ' ' || grid[centerY, dashX] == '·'))
                {
                    grid[centerY, dashX] = '-';
                }
            }
        }

        private static void PlaceTalismans(char[,] grid, MagicCircle circle, int centerX, int centerY, 
            int gridSize, bool showPowerLevels, bool showNames)
        {
            // Place talismans slightly inside the circle outline for better visibility
            var radiusInGrid = Math.Max(2.0, circle.Radius * 2.2); // Slightly smaller than outline radius
            
            for (int i = 0; i < circle.Talismans.Count; i++)
            {
                var talisman = circle.Talismans[i];
                
                // Calculate position around circle - start from top and go clockwise
                var angle = (2 * Math.PI * i) / circle.Talismans.Count - Math.PI / 2; // Start from top
                var x = centerX + (int)Math.Round(Math.Cos(angle) * radiusInGrid);
                var y = centerY + (int)Math.Round(Math.Sin(angle) * radiusInGrid);
                
                if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
                {
                    var symbol = ElementSymbols.GetSymbol(talisman.PrimaryElement.Type);
                    grid[y, x] = symbol;
                    
                    // Add power level indicator if requested and power is non-standard
                    if (showPowerLevels && Math.Abs(talisman.PrimaryElement.Energy - 1.0) > 0.1)
                    {
                        var powerIndicator = GetPowerIndicator(talisman.PrimaryElement.Energy);
                        // Try to place power indicator in a logical position
                        var placed = false;
                        
                        // Try positions in order: right, below, left, above
                        var offsets = new[] { (1, 0), (0, 1), (-1, 0), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
                        
                        foreach (var (dx, dy) in offsets)
                        {
                            var px = x + dx;
                            var py = y + dy;
                            if (px >= 0 && px < gridSize && py >= 0 && py < gridSize && 
                                grid[py, px] == ' ')
                            {
                                grid[py, px] = powerIndicator;
                                placed = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            // Mark center - either with center talisman or just center mark
            if (centerX >= 0 && centerX < gridSize && centerY >= 0 && centerY < gridSize)
            {
                if (circle.CenterTalisman != null)
                {
                    // Place center talisman symbol
                    var centerSymbol = ElementSymbols.GetSymbol(circle.CenterTalisman.PrimaryElement.Type);
                    grid[centerY, centerX] = centerSymbol;
                    
                    // Add power level indicator if requested and power is non-standard
                    if (showPowerLevels && Math.Abs(circle.CenterTalisman.PrimaryElement.Energy - 1.0) > 0.1)
                    {
                        var powerIndicator = GetPowerIndicator(circle.CenterTalisman.PrimaryElement.Energy);
                        // Try to place power indicator around the center talisman
                        var placed = false;
                        
                        // Try positions in order: right, below, left, above, diagonals
                        var offsets = new[] { (1, 0), (0, 1), (-1, 0), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
                        
                        foreach (var (dx, dy) in offsets)
                        {
                            var px = centerX + dx;
                            var py = centerY + dy;
                            if (px >= 0 && px < gridSize && py >= 0 && py < gridSize && 
                                grid[py, px] == ' ')
                            {
                                grid[py, px] = powerIndicator;
                                placed = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // No center talisman, just mark the center
                    grid[centerY, centerX] = '◦';
                }
            }
        }

        private static char GetPowerIndicator(double power)
        {
            return power switch
            {
                > 2.0 => '▲',
                > 1.5 => '△',
                < 0.5 => '▽',
                < 0.8 => '∇',
                _ => '·'
            };
        }

        private static void RenderGrid(char[,] grid, int gridSize, int totalWidth)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Console.Write("│ ");
                for (int x = 0; x < gridSize; x++)
                {
                    var ch = grid[y, x];
                    
                    // Color-code elements
                    switch (ch)
                    {
                        case 'F': // Fire
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 'W': // Water
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 'E': // Earth
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 'M': // Metal
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 'O': // Wood
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 'L': // Lightning
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case 'N': // Wind
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case 'I': // Light
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 'D': // Dark
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                        case 'G': // Forge
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        case 'C': // Chaos
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case 'V': // Void
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            break;
                        case '◦': // Center
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case '·': // Circle outline
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                        case '-': // Radius indicator
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        default:
                            Console.ResetColor();
                            break;
                    }
                    
                    Console.Write(ch);
                    Console.ResetColor();
                }
                
                // Calculate and add right padding to match total width
                var usedWidth = gridSize + 2; // Grid + "│ " at start
                var rightPadding = totalWidth - usedWidth;
                Console.Write(new string(' ', rightPadding));
                Console.WriteLine(" │");
            }
        }

        public static void ShowLegend()
        {
            Console.WriteLine("🎨 ELEMENT LEGEND:");
            Console.WriteLine("   F=Fire🔥 W=Water💧 E=Earth🌍 M=Metal⚡ O=Wood🌳");
            Console.WriteLine("   L=Lightning⚡ N=Wind💨 I=Light☀️ D=Dark🌙 G=Forge🔨");
            Console.WriteLine("   C=Chaos🌀 V=Void⚫ ◦=Center");
            Console.WriteLine("   Power: ▲=High △=Medium ∇=Low ▽=Very Low");
            Console.WriteLine();
        }

        public static void RenderCnfExample(string cnf)
        {
            Console.WriteLine($"📝 CNF: {cnf}");
            try
            {
                var circle = SpellSerializer.DeserializeCircleFromCnf(cnf);
                RenderCircle(circle, showPowerLevels: true);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Parse Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
