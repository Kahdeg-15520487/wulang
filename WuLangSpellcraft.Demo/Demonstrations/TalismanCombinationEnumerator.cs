using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo.Demonstrations
{
    /// <summary>
    /// Enumerates all possible talisman combinations for magic circles of size 4 and under with up to 8 talismans
    /// </summary>
    public static class TalismanCombinationEnumerator
    {
        /// <summary>
        /// Main demonstration entry point
        /// </summary>
        public static void RunDemo()
        {
            Console.WriteLine("🔮 TALISMAN COMBINATION ENUMERATOR");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("This demo explores all possible talisman combinations for:");
            Console.WriteLine("• Circle radius ≤ 4.0 units");
            Console.WriteLine("• Maximum 8 talismans per circle");
            Console.WriteLine("• All base and derived elements");
            Console.WriteLine();

            // Get all available elements
            var allElements = GetAllElementTypes();
            Console.WriteLine($"📋 Available Elements ({allElements.Count}):");
            foreach (var element in allElements)
            {
                var sampleElement = new Element(element);
                Console.ForegroundColor = sampleElement.Color;
                Console.Write($"  {sampleElement.Name} ({sampleElement.ChineseName})");
                Console.ResetColor();
                Console.WriteLine(sampleElement.IsBaseElement() ? " [Base]" : " [Derived]");
            }
            Console.WriteLine();

            // Ask user what they want to see
            Console.WriteLine("Choose demonstration mode:");
            Console.WriteLine("1. 🔍 Spell Prediction for combinations (≤5 talismans)");
            Console.WriteLine("2. 📊 Full enumeration statistics");
            Console.Write("Enter choice (1 or 2): ");
            
            var choice = Console.ReadLine();
            Console.WriteLine();

            if (choice == "1")
            {
                RunSpellPredictionDemo(allElements);
            }
            else
            {
                // Original enumeration logic
                for (double radius = 1.0; radius <= 4.0; radius += 0.5)
                {
                    DemoCircleSize(radius, allElements);
                    Console.WriteLine();
                }

                ShowCombinationStatistics(allElements);
            }
        }

        /// <summary>
        /// Runs spell prediction demo for combinations with 5 or fewer talismans
        /// </summary>
        private static void RunSpellPredictionDemo(List<ElementType> allElements)
        {
            Console.WriteLine("🔮 SPELL PREDICTION SYSTEM");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Analyzing all talisman combinations with ≤5 talismans to predict spell types...");
            Console.WriteLine("Including combinations with repeating talismans for enhanced analysis...");
            Console.WriteLine();

            var spellPredictions = new List<SpellPrediction>();
            var totalCombinations = 0;

            // Generate combinations for 1-5 talismans (with repetition allowed)
            for (int talismanCount = 1; talismanCount <= 5; talismanCount++)
            {
                Console.WriteLine($"🔍 Analyzing {talismanCount} talisman combinations...");
                
                // Generate both unique and repeating combinations
                var uniqueCombinations = GenerateUniqueElementCombinations(allElements, talismanCount);
                var repeatingCombinations = GenerateRepeatingElementCombinations(allElements, talismanCount, 100); // Limit to 100 per count for performance
                
                // Generate center talisman combinations (for 2+ talismans)
                var centerCombinations = new List<List<ElementType>>();
                if (talismanCount >= 2)
                {
                    centerCombinations = GenerateCenterTalismanCombinations(allElements, talismanCount, 50); // Limit for performance
                }
                
                var allCombinations = uniqueCombinations.Concat(repeatingCombinations).Concat(centerCombinations).ToList();
                totalCombinations += allCombinations.Count;
                
                foreach (var combination in allCombinations)
                {
                    SpellPrediction? prediction;
                    
                    // Check if this is a center talisman combination (from our center generation)
                    if (centerCombinations.Contains(combination))
                    {
                        prediction = PredictSpellFromCombinationWithCenter(combination);
                    }
                    else
                    {
                        prediction = PredictSpellFromCombination(combination);
                    }
                    
                    if (prediction != null)
                    {
                        spellPredictions.Add(prediction);
                    }
                }
                
                var centerCount = centerCombinations.Count;
                Console.WriteLine($"   Generated {allCombinations.Count} combinations (unique + repeating + {centerCount} center)");
            }

            Console.WriteLine();
            Console.WriteLine($"📊 ANALYSIS COMPLETE");
            Console.WriteLine($"   Total combinations analyzed: {totalCombinations:N0}");
            Console.WriteLine($"   Valid spell predictions: {spellPredictions.Count:N0}");
            Console.WriteLine();

            // Show results
            ShowSpellPredictionResults(spellPredictions);
            
            // Generate markdown documentation
            GenerateSpellMarkdown(spellPredictions);
            
            // Generate JSON documentation
            GenerateSpellJson(spellPredictions);
            
            // Check for center talisman usage
            CheckCenterTalismanUsage(spellPredictions);
        }

        /// <summary>
        /// Demonstrates all combinations for a specific circle size
        /// </summary>
        private static void DemoCircleSize(double radius, List<ElementType> allElements)
        {
            Console.WriteLine($"⚪ CIRCLE RADIUS: {radius:F1} units");
            Console.WriteLine(new string('─', 50));

            var testCircle = new MagicCircle($"Test Circle R{radius:F1}", radius);
            int maxTalismans = Math.Min(8, testCircle.CalculateMaxTalismans());
            
            Console.WriteLine($"Maximum Talismans: {maxTalismans}");
            Console.WriteLine($"Circle Circumference: {2 * Math.PI * radius:F1} units");
            Console.WriteLine();

            // Generate combinations for different talisman counts
            for (int talismanCount = 1; talismanCount <= maxTalismans; talismanCount++)
            {
                DemoTalismanCount(radius, talismanCount, allElements);
            }
        }

        /// <summary>
        /// Demonstrates combinations for a specific number of talismans
        /// </summary>
        private static void DemoTalismanCount(double radius, int talismanCount, List<ElementType> allElements)
        {
            Console.WriteLine($"  🔸 {talismanCount} Talisman{(talismanCount > 1 ? "s" : "")}:");

            if (talismanCount <= 4) // Show detailed combinations for small counts
            {
                var combinations = GenerateElementCombinations(allElements, talismanCount);
                ShowDetailedCombinations(radius, combinations, Math.Min(10, combinations.Count));
            }
            else // Show statistical overview for larger counts
            {
                ShowCombinationOverview(radius, talismanCount, allElements);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Shows detailed analysis of specific combinations
        /// </summary>
        private static void ShowDetailedCombinations(double radius, List<List<ElementType>> combinations, int maxToShow)
        {
            Console.WriteLine($"    Total Combinations: {combinations.Count:N0}");
            // Skip showing individual combinations to avoid console clutter
            // All detailed information is available in the generated markdown file
        }

        /// <summary>
        /// Shows statistical overview for larger combination sets
        /// </summary>
        private static void ShowCombinationOverview(double radius, int talismanCount, List<ElementType> allElements)
        {
            // Calculate total possible combinations (with repetition)
            long totalCombinations = (long)Math.Pow(allElements.Count, talismanCount);
            
            Console.WriteLine($"    Total Combinations: {totalCombinations:N0} (too many to show individually)");
            
            // Sample some combinations for analysis
            var sampleCombinations = GenerateRandomSample(allElements, talismanCount, 100);
            AnalyzeCombinationSample(radius, sampleCombinations);
        }

        /// <summary>
        /// Analyzes a sample of combinations for patterns
        /// </summary>
        private static void AnalyzeCombinationSample(double radius, List<List<ElementType>> sample)
        {
            var results = new List<CircleAnalysis>();
            
            foreach (var combination in sample)
            {
                var circle = CreateTestCircle(radius, combination);
                var spellEffect = circle.CalculateSpellEffect();
                
                results.Add(new CircleAnalysis
                {
                    Combination = combination,
                    PowerOutput = circle.PowerOutput,
                    Stability = circle.Stability,
                    Efficiency = circle.Efficiency,
                    EffectType = spellEffect.Type,
                    DominantElement = circle.GetDominantElement()
                });
            }

            // Statistical analysis
            var avgPower = results.Average(r => r.PowerOutput);
            var avgStability = results.Average(r => r.Stability);
            var avgEfficiency = results.Average(r => r.Efficiency);
            var maxPower = results.Max(r => r.PowerOutput);
            var maxStability = results.Max(r => r.Stability);

            Console.WriteLine($"    Sample Analysis ({sample.Count} combinations):");
            Console.WriteLine($"      Average Power: {avgPower:F2} | Max: {maxPower:F2}");
            Console.WriteLine($"      Average Stability: {avgStability:F3} | Max: {maxStability:F3}");
            Console.WriteLine($"      Average Efficiency: {avgEfficiency:F3}");

            // Effect type distribution
            var effectGroups = results.GroupBy(r => r.EffectType).OrderByDescending(g => g.Count());
            Console.WriteLine($"      Most Common Effects: {string.Join(", ", effectGroups.Take(3).Select(g => $"{g.Key}({g.Count()})"))}");

            // Best combinations
            var bestPower = results.OrderByDescending(r => r.PowerOutput).First();
            var bestStability = results.OrderByDescending(r => r.Stability).First();
            
            Console.WriteLine($"      Highest Power: {FormatElementCombination(bestPower.Combination)} ({bestPower.PowerOutput:F2})");
            Console.WriteLine($"      Highest Stability: {FormatElementCombination(bestStability.Combination)} ({bestStability.Stability:F3})");
        }

        /// <summary>
        /// Shows overall statistics about all possible combinations
        /// </summary>
        private static void ShowCombinationStatistics(List<ElementType> allElements)
        {
            Console.WriteLine("📊 COMBINATION STATISTICS SUMMARY");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            var totalCombinationsAcrossAllSizes = 0L;
            
            Console.WriteLine("Total combinations by circle size and talisman count:");
            Console.WriteLine();
            Console.WriteLine("Radius | Max T. | 1T     | 2T      | 3T       | 4T        | 5T         | 6T          | 7T           | 8T");
            Console.WriteLine("-------|--------|--------|---------|----------|-----------|------------|-------------|-------------|------------");

            for (double radius = 1.0; radius <= 4.0; radius += 0.5)
            {
                var testCircle = new MagicCircle($"Test", radius);
                int maxT = Math.Min(8, testCircle.CalculateMaxTalismans());
                
                Console.Write($"{radius,4:F1}   |{maxT,6}  |");
                
                for (int t = 1; t <= 8; t++)
                {
                    if (t <= maxT)
                    {
                        long combinations = (long)Math.Pow(allElements.Count, t);
                        totalCombinationsAcrossAllSizes += combinations;
                        Console.Write($"{combinations,8:N0} |");
                    }
                    else
                    {
                        Console.Write("      - |");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Total possible combinations across all sizes: {totalCombinationsAcrossAllSizes:N0}");
            Console.WriteLine($"Available elements: {allElements.Count} ({allElements.Count(e => new Element(e).IsBaseElement())} base + {allElements.Count(e => !new Element(e).IsBaseElement())} derived)");
            
            // Element interaction statistics
            ShowElementInteractionStatistics(allElements);
        }

        /// <summary>
        /// Shows statistics about element interactions
        /// </summary>
        private static void ShowElementInteractionStatistics(List<ElementType> allElements)
        {
            Console.WriteLine();
            Console.WriteLine("Element Interaction Matrix:");
            Console.WriteLine();

            var generativeCount = 0;
            var destructiveCount = 0;
            var neutralCount = 0;

            Console.Write("        ");
            foreach (var element in allElements.Take(8)) // Limit display width
            {
                Console.Write($"{element.ToString().Substring(0, Math.Min(4, element.ToString().Length)),6}");
            }
            Console.WriteLine("...");

            foreach (var element1 in allElements.Take(8))
            {
                Console.Write($"{element1.ToString().Substring(0, Math.Min(7, element1.ToString().Length)),7} ");
                
                foreach (var element2 in allElements.Take(8))
                {
                    if (element1 == element2)
                    {
                        Console.Write("  =   ");
                        continue;
                    }

                    var relation = Element.GetElementRelation(element1, element2);
                    var symbol = relation switch
                    {
                        ElementRelation.Generates => "  +  ",
                        ElementRelation.Destroys => "  -  ",
                        ElementRelation.Neutral => "  ○  ",
                        _ => "  ?  "
                    };
                    Console.Write(symbol);

                    // Count for statistics
                    switch (relation)
                    {
                        case ElementRelation.Generates: generativeCount++; break;
                        case ElementRelation.Destroys: destructiveCount++; break;
                        case ElementRelation.Neutral: neutralCount++; break;
                    }
                }
                Console.WriteLine();
            }

            var totalRelations = generativeCount + destructiveCount + neutralCount;
            Console.WriteLine();
            Console.WriteLine($"Interaction Distribution:");
            Console.WriteLine($"  Generative (+): {generativeCount} ({(double)generativeCount / totalRelations:P1})");
            Console.WriteLine($"  Destructive (-): {destructiveCount} ({(double)destructiveCount / totalRelations:P1})");
            Console.WriteLine($"  Neutral (○): {neutralCount} ({(double)neutralCount / totalRelations:P1})");
        }

        /// <summary>
        /// Helper method to get all available element types
        /// </summary>
        private static List<ElementType> GetAllElementTypes()
        {
            return Enum.GetValues<ElementType>().ToList();
        }

        /// <summary>
        /// Generates all possible combinations of elements for a given count
        /// </summary>
        private static List<List<ElementType>> GenerateElementCombinations(List<ElementType> elements, int count)
        {
            if (count == 1)
            {
                return elements.Select(e => new List<ElementType> { e }).ToList();
            }

            var combinations = new List<List<ElementType>>();
            var smaller = GenerateElementCombinations(elements, count - 1);

            foreach (var element in elements)
            {
                foreach (var smallerCombination in smaller)
                {
                    var newCombination = new List<ElementType>(smallerCombination) { element };
                    combinations.Add(newCombination);
                }
            }

            return combinations;
        }

        /// <summary>
        /// Generates a random sample of combinations
        /// </summary>
        private static List<List<ElementType>> GenerateRandomSample(List<ElementType> elements, int count, int sampleSize)
        {
            var random = new Random(42); // Fixed seed for reproducible results
            var samples = new List<List<ElementType>>();

            for (int i = 0; i < sampleSize; i++)
            {
                var combination = new List<ElementType>();
                for (int j = 0; j < count; j++)
                {
                    combination.Add(elements[random.Next(elements.Count)]);
                }
                samples.Add(combination);
            }

            return samples;
        }

        /// <summary>
        /// Creates a test circle with the given element combination
        /// </summary>
        private static MagicCircle CreateTestCircle(double radius, List<ElementType> elements)
        {
            var circle = new MagicCircle($"Test_{radius:F1}", radius);

            foreach (var elementType in elements)
            {
                var element = new Element(elementType, 1.0);
                var talisman = new Talisman(element, $"{elementType}_Talisman");
                circle.AddTalisman(talisman);
            }

            return circle;
        }

        /// <summary>
        /// Formats an element combination for display
        /// </summary>
        private static string FormatElementCombination(List<ElementType> elements)
        {
            return string.Join("+", elements.Select(e => e.ToString().Substring(0, Math.Min(4, e.ToString().Length))));
        }

        /// <summary>
        /// Helper class for combination analysis
        /// </summary>
        private class CircleAnalysis
        {
            public List<ElementType> Combination { get; set; } = new();
            public double PowerOutput { get; set; }
            public double Stability { get; set; }
            public double Efficiency { get; set; }
            public SpellEffectType EffectType { get; set; }
            public ElementType DominantElement { get; set; }
        }

        /// <summary>
        /// Generates unique element combinations (no duplicates within combination)
        /// </summary>
        private static List<List<ElementType>> GenerateUniqueElementCombinations(List<ElementType> elements, int count)
        {
            var combinations = new List<List<ElementType>>();
            GenerateUniqueCombinationsRecursive(elements, count, new List<ElementType>(), combinations, 0);
            return combinations;
        }

        private static void GenerateUniqueCombinationsRecursive(
            List<ElementType> elements, 
            int remainingCount, 
            List<ElementType> current, 
            List<List<ElementType>> results,
            int startIndex)
        {
            if (remainingCount == 0)
            {
                results.Add(new List<ElementType>(current));
                return;
            }

            for (int i = startIndex; i < elements.Count; i++)
            {
                current.Add(elements[i]);
                GenerateUniqueCombinationsRecursive(elements, remainingCount - 1, current, results, i + 1); // No repeats
                current.RemoveAt(current.Count - 1);
            }
        }

        /// <summary>
        /// Generates combinations with repeating elements allowed
        /// </summary>
        private static List<List<ElementType>> GenerateRepeatingElementCombinations(List<ElementType> elements, int count, int maxResults)
        {
            var combinations = new List<List<ElementType>>();
            var random = new Random(42); // Fixed seed for reproducible results
            
            // Generate random combinations with repeats
            for (int i = 0; i < maxResults; i++)
            {
                var combination = new List<ElementType>();
                for (int j = 0; j < count; j++)
                {
                    combination.Add(elements[random.Next(elements.Count)]);
                }
                
                // Check if this combination is unique (avoid duplicates)
                var combinationKey = string.Join(",", combination.OrderBy(e => e));
                if (!combinations.Any(c => string.Join(",", c.OrderBy(e => e)) == combinationKey))
                {
                    combinations.Add(combination);
                }
            }
            
            return combinations;
        }

        /// <summary>
        /// Generates combinations with center talismans using @element syntax
        /// </summary>
        private static List<List<ElementType>> GenerateCenterTalismanCombinations(List<ElementType> elements, int totalCount, int maxResults)
        {
            var combinations = new List<List<ElementType>>();
            var random = new Random(42); // Fixed seed for reproducible results
            
            if (totalCount < 2) return combinations; // Need at least 2 talismans (1 center + 1 perimeter)
            
            var perimeterCount = totalCount - 1; // Reserve 1 slot for center talisman
            
            // Generate different center talisman combinations
            foreach (var centerElement in elements)
            {
                // Generate perimeter combinations for this center element
                var perimeterCombinations = GenerateRepeatingElementCombinations(elements, perimeterCount, maxResults / elements.Count);
                
                foreach (var perimeterCombo in perimeterCombinations.Take(maxResults / elements.Count))
                {
                    // Create combination with center talisman marked specially
                    var combination = new List<ElementType> { centerElement }; // Center element first
                    combination.AddRange(perimeterCombo); // Then perimeter elements
                    
                    // Mark this as a center talisman combination by storing it in a special way
                    // We'll use a special property in the SpellPrediction to track this
                    combinations.Add(combination);
                    
                    if (combinations.Count >= maxResults) break;
                }
                
                if (combinations.Count >= maxResults) break;
            }
            
            return combinations.Take(maxResults).ToList();
        }

        /// <summary>
        /// Generates a markdown documentation file with all spell predictions
        /// </summary>
        private static void GenerateSpellMarkdown(List<SpellPrediction> predictions)
        {
            Console.WriteLine();
            Console.WriteLine("📝 GENERATING SPELL DOCUMENTATION");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            
            var markdownContent = GenerateMarkdownContent(predictions);
            var filePath = "SpellPredictions.md";
            var fullPath = Path.GetFullPath(filePath);
            
            try
            {
                File.WriteAllText(filePath, markdownContent);
                Console.WriteLine($"✅ Spell documentation successfully generated!");
                Console.WriteLine($"📄 File location: {fullPath}");
                Console.WriteLine($"📊 Total spells documented: {predictions.Count:N0}");
                
                // Show file size
                var fileInfo = new FileInfo(filePath);
                Console.WriteLine($"📋 File size: {fileInfo.Length / 1024.0:F1} KB");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generating markdown: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a JSON documentation file with all spell predictions
        /// </summary>
        private static void GenerateSpellJson(List<SpellPrediction> predictions)
        {
            Console.WriteLine();
            Console.WriteLine("📄 GENERATING JSON SPELL DATABASE");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            
            var jsonContent = GenerateJsonContent(predictions);
            var filePath = "SpellDatabase.json";
            var fullPath = Path.GetFullPath(filePath);
            
            try
            {
                File.WriteAllText(filePath, jsonContent);
                Console.WriteLine($"✅ JSON spell database successfully generated!");
                Console.WriteLine($"📄 File location: {fullPath}");
                Console.WriteLine($"📊 Total spells in database: {predictions.Count:N0}");
                
                // Show file size
                var fileInfo = new FileInfo(filePath);
                Console.WriteLine($"📋 File size: {fileInfo.Length / 1024.0:F1} KB");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generating JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates the JSON content for spell database
        /// </summary>
        private static string GenerateJsonContent(List<SpellPrediction> predictions)
        {
            var database = new
            {
                metadata = new
                {
                    title = "WuLang Spellcraft - Spell Database",
                    version = "1.0",
                    generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    total_spells = predictions.Count,
                    description = "Systematic documentation of all predicted spells from talisman combinations"
                },
                statistics = new
                {
                    categories = predictions.GroupBy(p => p.SpellCategory)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    effect_types = predictions.GroupBy(p => p.EffectType)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    power_analysis = new
                    {
                        average = Math.Round(predictions.Average(p => p.Power), 2),
                        maximum = Math.Round(predictions.Max(p => p.Power), 2),
                        minimum = Math.Round(predictions.Min(p => p.Power), 2)
                    },
                    talisman_count_distribution = predictions.GroupBy(p => p.Elements.Count)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count())
                },
                spells = predictions.Select(p => new
                {
                    id = GenerateSpellId(p),
                    name = p.SpellName,
                    description = p.SpellDescription,
                    category = p.SpellCategory.ToString(),
                    effect_type = p.EffectType.ToString(),
                    elements = p.Elements.Select(e => e.ToString()).ToArray(),
                    element_count = p.Elements.Count,
                    circle_notation = GenerateCircleNotation(p.Elements, p.HasCenterTalisman, p.CenterElement),
                    stats = new
                    {
                        power = Math.Round(p.Power, 2),
                        range = Math.Round(p.Range, 1),
                        duration = Math.Round(p.Duration, 1),
                        stability = Math.Round(p.Stability, 3),
                        efficiency = Math.Round(p.Efficiency, 3),
                        casting_time = Math.Round(p.CastingTime, 1),
                        complexity_score = Math.Round(p.ComplexityScore, 2)
                    },
                    element_composition = p.Elements.GroupBy(e => e)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    has_repeating_elements = p.Elements.GroupBy(e => e).Any(g => g.Count() > 1),
                    has_center_talisman = p.HasCenterTalisman,
                    center_element = p.CenterElement?.ToString()
                }).OrderBy(s => s.category).ThenByDescending(s => s.stats.power).ToArray()
            };

            return System.Text.Json.JsonSerializer.Serialize(database, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower
            });
        }

        /// <summary>
        /// Generates a unique ID for a spell based on its properties
        /// </summary>
        private static string GenerateSpellId(SpellPrediction prediction)
        {
            var elementString = string.Join("", prediction.Elements.Select(e => e.ToString()[0]));
            var categoryCode = prediction.SpellCategory.ToString()[0];
            var effectCode = prediction.EffectType.ToString()[0];
            var powerLevel = Math.Floor(prediction.Power).ToString();
            
            return $"{categoryCode}{effectCode}_{elementString}_{powerLevel}";
        }

        /// <summary>
        /// Checks for center talisman usage in spells
        /// </summary>
        private static void CheckCenterTalismanUsage(List<SpellPrediction> predictions)
        {
            Console.WriteLine();
            Console.WriteLine("🎯 CENTER TALISMAN ANALYSIS");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            
            // Count actual center talisman spells
            var centerTalismanSpells = predictions.Where(p => p.HasCenterTalisman).ToList();
            
            // Check if any of our generated spells could benefit from center talismans
            var potentialCenterSpells = predictions.Where(p => 
                !p.HasCenterTalisman && // Don't already have center
                p.Elements.Count >= 3 && // Need at least 3 elements to benefit from center
                p.Stability < 0.7 && // Low stability might benefit from center stabilization
                p.Power > 2.0 // High power spells often use center talismans
            ).ToList();
            
            Console.WriteLine($"📊 Current Analysis Results:");
            Console.WriteLine($"   Total spells analyzed: {predictions.Count:N0}");
            Console.WriteLine($"   Spells using center talismans: {centerTalismanSpells.Count:N0}");
            Console.WriteLine($"   Spells that could benefit from center: {potentialCenterSpells.Count:N0}");
            Console.WriteLine();
            
            if (centerTalismanSpells.Any())
            {
                Console.WriteLine("✨ Center Talisman Spells Generated:");
                var topCenterSpells = centerTalismanSpells
                    .OrderByDescending(p => p.Power)
                    .Take(5);
                    
                foreach (var spell in topCenterSpells)
                {
                    Console.WriteLine($"   • {spell.SpellName}");
                    Console.WriteLine($"     Center: @{spell.CenterElement} | Perimeter: {string.Join("+", spell.Elements.Skip(1))}");
                    Console.WriteLine($"     Stats: Power {spell.Power:F1}, Stability {spell.Stability:F3}");
                    Console.WriteLine($"     Notation: {GenerateCircleNotation(spell.Elements, spell.HasCenterTalisman, spell.CenterElement)}");
                    Console.WriteLine();
                }
            }
            
            if (potentialCenterSpells.Any())
            {
                Console.WriteLine("🔮 Top candidates for center talisman enhancement:");
                var topCandidates = potentialCenterSpells
                    .OrderByDescending(p => p.Power)
                    .ThenBy(p => p.Stability)
                    .Take(5);
                    
                foreach (var spell in topCandidates)
                {
                    Console.WriteLine($"   • {spell.SpellName}");
                    Console.WriteLine($"     Elements: {string.Join("+", spell.Elements)}");
                    Console.WriteLine($"     Current: Power {spell.Power:F1}, Stability {spell.Stability:F3}");
                    Console.WriteLine($"     Potential center element: {SuggestCenterElement(spell.Elements)}");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Suggests a center element that would stabilize the given combination
        /// </summary>
        private static ElementType SuggestCenterElement(List<ElementType> elements)
        {
            // Find the most common element or an element that would provide balance
            var elementCounts = elements.GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());
            
            // If there's a dominant element, suggest its complementary element for balance
            var dominantElement = elementCounts.OrderByDescending(kv => kv.Value).First().Key;
            
            // Simple logic: suggest a balancing element
            return dominantElement switch
            {
                ElementType.Fire => ElementType.Water,
                ElementType.Water => ElementType.Fire,
                ElementType.Earth => ElementType.Wind,
                ElementType.Metal => ElementType.Wood,
                ElementType.Wood => ElementType.Metal,
                ElementType.Lightning => ElementType.Earth,
                ElementType.Wind => ElementType.Earth,
                _ => ElementType.Void // Void as a neutral balancing element
            };
        }

        /// <summary>
        /// Generates the markdown content for spell documentation
        /// </summary>
        private static string GenerateMarkdownContent(List<SpellPrediction> predictions)
        {
            var markdown = new StringBuilder();
            
            // Header
            markdown.AppendLine("# WuLang Spellcraft - Spell Prediction Compendium");
            markdown.AppendLine();
            markdown.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            markdown.AppendLine($"Total Spells Analyzed: {predictions.Count:N0}");
            markdown.AppendLine();
            markdown.AppendLine("## Table of Contents");
            markdown.AppendLine("- [Statistics](#statistics)");
            markdown.AppendLine("- [Combat Spells](#combat-spells)");
            markdown.AppendLine("- [Defense Spells](#defense-spells)");
            markdown.AppendLine("- [Restoration Spells](#restoration-spells)");
            markdown.AppendLine("- [Enhancement Spells](#enhancement-spells)");
            markdown.AppendLine("- [Utility Spells](#utility-spells)");
            markdown.AppendLine("- [Transmutation Spells](#transmutation-spells)");
            markdown.AppendLine();
            
            // Statistics section
            GenerateStatisticsSection(markdown, predictions);
            
            // Spells by category
            var categories = predictions.GroupBy(p => p.SpellCategory).OrderBy(g => g.Key);
            
            foreach (var categoryGroup in categories)
            {
                GenerateCategorySection(markdown, categoryGroup.Key, categoryGroup.ToList());
            }
            
            return markdown.ToString();
        }

        /// <summary>
        /// Generates the statistics section of the markdown
        /// </summary>
        private static void GenerateStatisticsSection(StringBuilder markdown, List<SpellPrediction> predictions)
        {
            markdown.AppendLine("## Statistics");
            markdown.AppendLine();
            
            // Category distribution
            var categoryStats = predictions.GroupBy(p => p.SpellCategory)
                .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = (double)g.Count() / predictions.Count * 100 })
                .OrderByDescending(s => s.Count);
            
            markdown.AppendLine("### Spell Categories Distribution");
            markdown.AppendLine("| Category | Count | Percentage |");
            markdown.AppendLine("|----------|-------|------------|");
            foreach (var stat in categoryStats)
            {
                markdown.AppendLine($"| {stat.Category} | {stat.Count} | {stat.Percentage:F1}% |");
            }
            markdown.AppendLine();
            
            // Effect types
            var effectStats = predictions.GroupBy(p => p.EffectType)
                .Select(g => new { Effect = g.Key, Count = g.Count() })
                .OrderByDescending(s => s.Count);
            
            markdown.AppendLine("### Effect Types Distribution");
            markdown.AppendLine("| Effect Type | Count |");
            markdown.AppendLine("|-------------|-------|");
            foreach (var stat in effectStats)
            {
                markdown.AppendLine($"| {stat.Effect} | {stat.Count} |");
            }
            markdown.AppendLine();
            
            // Power analysis
            var avgPower = predictions.Average(p => p.Power);
            var maxPower = predictions.Max(p => p.Power);
            var minPower = predictions.Min(p => p.Power);
            
            markdown.AppendLine("### Power Analysis");
            markdown.AppendLine($"- **Average Power**: {avgPower:F2}");
            markdown.AppendLine($"- **Maximum Power**: {maxPower:F2}");
            markdown.AppendLine($"- **Minimum Power**: {minPower:F2}");
            markdown.AppendLine();
        }

        /// <summary>
        /// Generates a category section of the markdown
        /// </summary>
        private static void GenerateCategorySection(StringBuilder markdown, SpellCategory category, List<SpellPrediction> spells)
        {
            markdown.AppendLine($"## {category} Spells");
            markdown.AppendLine();
            markdown.AppendLine($"Total spells in category: {spells.Count}");
            markdown.AppendLine();
            
            // Sort by power * stability for best spells first
            var sortedSpells = spells.OrderByDescending(s => s.Power * s.Stability).Take(50).ToList(); // Limit to top 50 per category
            
            foreach (var spell in sortedSpells)
            {
                GenerateSpellEntry(markdown, spell);
            }
        }

        /// <summary>
        /// Generates a single spell entry in markdown
        /// </summary>
        private static void GenerateSpellEntry(StringBuilder markdown, SpellPrediction spell)
        {
            markdown.AppendLine($"### {spell.SpellName}");
            markdown.AppendLine();
            
            // Circle notation
            var circleNotation = GenerateCircleNotation(spell.Elements, spell.HasCenterTalisman, spell.CenterElement);
            markdown.AppendLine($"**Circle Notation**: `{circleNotation}`");
            markdown.AppendLine();
            
            // Element composition with repetition counts
            var elementCounts = GetElementCounts(spell.Elements);
            markdown.AppendLine("**Element Composition**:");
            foreach (var kvp in elementCounts.OrderByDescending(x => x.Value))
            {
                var element = new Element(kvp.Key);
                var runeSymbol = GetElementRuneSymbol(kvp.Key);
                var powerLevel = CalculateElementPowerLevel(kvp.Key, kvp.Value);
                markdown.AppendLine($"- {element.Name} ({element.ChineseName}) {runeSymbol} x{kvp.Value} - Power Level: {powerLevel:F1}");
            }
            markdown.AppendLine();
            
            // Spell details
            markdown.AppendLine("**Spell Properties**:");
            markdown.AppendLine($"- **Effect Type**: {spell.EffectType}");
            markdown.AppendLine($"- **Description**: {spell.SpellDescription}");
            markdown.AppendLine($"- **Power**: {spell.Power:F2}");
            markdown.AppendLine($"- **Range**: {spell.Range:F1}m");
            markdown.AppendLine($"- **Duration**: {spell.Duration:F1}s");
            markdown.AppendLine($"- **Stability**: {spell.Stability:F3}");
            markdown.AppendLine($"- **Efficiency**: {spell.Efficiency:F3}");
            markdown.AppendLine($"- **Casting Time**: {spell.CastingTime:F2}s");
            markdown.AppendLine($"- **Complexity Score**: {spell.ComplexityScore:F2}");
            markdown.AppendLine();
        }

        /// <summary>
        /// Generates circle notation format for the spell
        /// </summary>
        private static string GenerateCircleNotation(List<ElementType> elements, bool hasCenterTalisman = false, ElementType? centerElement = null)
        {
            var notation = new StringBuilder();
            
            // Standard CNF format starts with C<radius>
            notation.Append("C4 "); // Using radius 4 as standard for spell predictions
            
            // Handle center talisman notation
            if (hasCenterTalisman && centerElement.HasValue)
            {
                // For center talisman combinations, first element is center, rest are perimeter
                var perimeterElements = elements.Skip(1).ToList();
                var elementCounts = GetElementCounts(perimeterElements);
                var notationParts = new List<string>();
                
                foreach (var kvp in elementCounts)
                {
                    var symbol = GetElementCnfSymbol(kvp.Key);
                    if (kvp.Value > 1)
                    {
                        notationParts.Add($"{symbol}*{kvp.Value}");
                    }
                    else
                    {
                        notationParts.Add(symbol);
                    }
                }
                
                // Add center talisman notation with @ prefix  
                var centerSymbol = GetElementCnfSymbol(centerElement.Value);
                notation.Append(string.Join("", notationParts));
                notation.Append($"@{centerSymbol}");
            }
            else
            {
                // Regular perimeter-only notation
                var elementCounts = GetElementCounts(elements);
                var notationParts = new List<string>();
                
                foreach (var kvp in elementCounts)
                {
                    var symbol = GetElementCnfSymbol(kvp.Key);
                    if (kvp.Value > 1)
                    {
                        notationParts.Add($"{symbol}*{kvp.Value}");
                    }
                    else
                    {
                        notationParts.Add(symbol);
                    }
                }
                
                notation.Append(string.Join("", notationParts));
            }
            
            return notation.ToString();
        }

        /// <summary>
        /// Gets element counts for repeated elements
        /// </summary>
        private static Dictionary<ElementType, int> GetElementCounts(List<ElementType> elements)
        {
            var counts = new Dictionary<ElementType, int>();
            foreach (var element in elements)
            {
                counts[element] = counts.ContainsKey(element) ? counts[element] + 1 : 1;
            }
            return counts;
        }

        /// <summary>
        /// Gets the CNF symbol for an element
        /// </summary>
        private static string GetElementCnfSymbol(ElementType element)
        {
            return element switch
            {
                ElementType.Water => "W",
                ElementType.Fire => "F",
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

        /// <summary>
        /// Gets the rune symbol for an element
        /// </summary>
        private static string GetElementRuneSymbol(ElementType element)
        {
            return element switch
            {
                ElementType.Water => "≋",
                ElementType.Fire => "▲",
                ElementType.Earth => "■",
                ElementType.Metal => "◇",
                ElementType.Wood => "※",
                ElementType.Lightning => "⚡",
                ElementType.Wind => "○",
                ElementType.Light => "☀",
                ElementType.Dark => "●",
                ElementType.Forge => "⚒",
                ElementType.Chaos => "※",
                ElementType.Void => "○",
                _ => "?"
            };
        }

        /// <summary>
        /// Calculates power level based on element type and repetition count
        /// </summary>
        private static double CalculateElementPowerLevel(ElementType element, int count)
        {
            var basePower = element switch
            {
                ElementType.Fire => 1.5,
                ElementType.Metal => 1.4,
                ElementType.Lightning => 1.3,
                ElementType.Earth => 1.2,
                ElementType.Water => 1.1,
                ElementType.Wood => 1.0,
                ElementType.Light => 1.6,
                ElementType.Dark => 1.3,
                ElementType.Forge => 1.8,
                ElementType.Chaos => 1.7,
                ElementType.Void => 1.9,
                ElementType.Wind => 1.1,
                _ => 1.0
            };
            
            // Diminishing returns for repetition
            var repetitionMultiplier = count == 1 ? 1.0 : 1.0 + (count - 1) * 0.3;
            return basePower * repetitionMultiplier;
        }

        /// <summary>
        /// Predicts what type of spell a talisman combination would create
        /// </summary>
        private static SpellPrediction? PredictSpellFromCombination(List<ElementType> elements)
        {
            try
            {
                var testCircle = CreateTestCircle(4.0, elements);
                var spellEffect = testCircle.CalculateSpellEffect();
                
                if (testCircle.Stability <= 0 || testCircle.PowerOutput <= 0)
                    return null;

                var spellName = GenerateSpellName(elements, spellEffect);
                var spellDescription = GenerateSpellDescription(elements, spellEffect, testCircle);
                var spellCategory = CategorizeSpell(spellEffect, elements);

                return new SpellPrediction
                {
                    Elements = new List<ElementType>(elements),
                    SpellName = spellName,
                    SpellDescription = spellDescription,
                    SpellCategory = spellCategory,
                    EffectType = spellEffect.Type,
                    Power = spellEffect.Power,
                    Range = spellEffect.Range,
                    Duration = spellEffect.Duration,
                    Stability = testCircle.Stability,
                    Efficiency = testCircle.Efficiency,
                    CastingTime = testCircle.CastingTime,
                    ComplexityScore = testCircle.ComplexityScore,
                    HasCenterTalisman = false,
                    CenterElement = null
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Predicts what type of spell a talisman combination with center talisman would create
        /// </summary>
        private static SpellPrediction? PredictSpellFromCombinationWithCenter(List<ElementType> elements)
        {
            try
            {
                if (elements.Count < 2) return null;
                
                // First element is the center talisman, rest are perimeter
                var centerElement = elements[0];
                var perimeterElements = elements.Skip(1).ToList();
                
                // Create a test circle with center talisman
                var testCircle = CreateTestCircle(4.0, perimeterElements);
                var centerTalisman = new Talisman(new Element(centerElement));
                testCircle.SetCenterTalisman(centerTalisman);
                
                var spellEffect = testCircle.CalculateSpellEffect();
                var spellName = GenerateCenterSpellName(centerElement, perimeterElements, spellEffect);
                var spellDescription = GenerateSpellDescription(elements, spellEffect, testCircle);
                var spellCategory = CategorizeSpell(spellEffect, elements);

                return new SpellPrediction
                {
                    Elements = new List<ElementType>(elements),
                    SpellName = spellName,
                    SpellDescription = spellDescription,
                    SpellCategory = spellCategory,
                    EffectType = spellEffect.Type,
                    Power = spellEffect.Power * 1.3, // Center talisman provides power boost
                    Range = spellEffect.Range * 1.2, // Improved range
                    Duration = spellEffect.Duration * 1.15, // Improved duration
                    Stability = Math.Min(1.0, testCircle.Stability * 1.4), // Significant stability boost
                    Efficiency = testCircle.Efficiency * 1.1, // Slight efficiency improvement
                    CastingTime = testCircle.CastingTime * 1.2, // Longer casting time due to complexity
                    ComplexityScore = testCircle.ComplexityScore * 1.3, // Higher complexity
                    HasCenterTalisman = true,
                    CenterElement = centerElement
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a spell name for center talisman combinations
        /// </summary>
        private static string GenerateCenterSpellName(ElementType centerElement, List<ElementType> perimeterElements, SpellEffect effect)
        {
            var centerName = centerElement.ToString();
            var effectName = effect.Type.ToString();
            var perimeterCount = perimeterElements.Count;

            // Special center combinations
            if (centerElement == ElementType.Void && perimeterElements.Contains(ElementType.Light))
                return "Void-Heart Radiance";
            if (centerElement == ElementType.Fire && perimeterElements.Contains(ElementType.Water))
                return "Steam-Core Veil";
            if (centerElement == ElementType.Earth && perimeterElements.Any(e => e == ElementType.Metal))
                return "Stone-Heart Fortress";
            if (centerElement == ElementType.Lightning && perimeterElements.Count >= 3)
                return "Storm-Core Manifestation";
            if (centerElement == ElementType.Wood && perimeterElements.Contains(ElementType.Fire))
                return "Life-Core Inferno";

            // Generic center naming
            return $"{centerName}-Centered {effectName}";
        }

        /// <summary>
        /// Generates a spell name based on elements and effect
        /// </summary>
        private static string GenerateSpellName(List<ElementType> elements, SpellEffect effect)
        {
            var primaryElement = elements.First();
            var effectName = effect.Type.ToString();

            if (elements.Count == 1)
            {
                return $"{primaryElement} {effectName}";
            }

            // Special combinations
            if (elements.Contains(ElementType.Fire) && elements.Contains(ElementType.Metal))
                return "Forge Blast";
            if (elements.Contains(ElementType.Water) && elements.Contains(ElementType.Earth))
                return "Mud Guardian";
            if (elements.Contains(ElementType.Fire) && elements.Contains(ElementType.Wood))
                return "Burning Growth";
            if (elements.Contains(ElementType.Light) && elements.Contains(ElementType.Void))
                return "Reality Shift";
            if (elements.Contains(ElementType.Fire) && elements.Contains(ElementType.Water))
                return "Steam Veil";
            if (elements.Contains(ElementType.Metal) && elements.Contains(ElementType.Wood))
                return "Cutting Vines";
            if (elements.Contains(ElementType.Earth) && elements.Contains(ElementType.Metal))
                return "Stone Spear";

            // Generic naming
            var elementString = string.Join("-", elements.Take(2));
            return $"{elementString} {effectName}";
        }

        /// <summary>
        /// Generates a spell description based on elements and properties
        /// </summary>
        private static string GenerateSpellDescription(List<ElementType> elements, SpellEffect effect, MagicCircle circle)
        {
            var descriptions = new List<string>();

            // Effect description
            descriptions.Add(effect.Type switch
            {
                SpellEffectType.Projectile => "Launches a directed energy projectile",
                SpellEffectType.Flow => "Creates flowing, adaptive magical force",
                SpellEffectType.Barrier => "Creates a protective barrier",
                SpellEffectType.Enhancement => "Enhances the target's capabilities",
                SpellEffectType.Growth => "Promotes growth and restoration",
                SpellEffectType.Hybrid => "Combines multiple magical effects",
                _ => "Manifests magical energy"
            });

            // Element-specific effects
            foreach (var element in elements.Take(3))
            {
                descriptions.Add(element switch
                {
                    ElementType.Fire => "with intense heat and energy",
                    ElementType.Water => "with flowing, adaptive force",
                    ElementType.Earth => "with solid, grounding power",
                    ElementType.Metal => "with precise, cutting edge",
                    ElementType.Wood => "with growing, life-giving energy",
                    ElementType.Light => "with illuminating radiance",
                    ElementType.Void => "with reality-bending force",
                    ElementType.Forge => "with transformative power",
                    _ => "with elemental force"
                });
            }

            // Power and range info
            descriptions.Add($"Power: {effect.Power:F1}, Range: {effect.Range:F1}m");

            return string.Join(" ", descriptions) + ".";
        }

        /// <summary>
        /// Categorizes the spell based on its properties and element repetition
        /// </summary>
        private static SpellCategory CategorizeSpell(SpellEffect effect, List<ElementType> elements)
        {
            // Get element counts for repetition analysis
            var elementCounts = GetElementCounts(elements);
            var maxRepeats = elementCounts.Values.Max();
            var dominantElements = elementCounts.Where(kvp => kvp.Value == maxRepeats).Select(kvp => kvp.Key).ToList();
            
            // Base categorization on effect type
            var category = effect.Type switch
            {
                SpellEffectType.Projectile => SpellCategory.Combat,
                SpellEffectType.Flow => elements.Any(e => e == ElementType.Fire || e == ElementType.Metal) 
                    ? SpellCategory.Combat : SpellCategory.Utility,
                SpellEffectType.Enhancement => SpellCategory.Enhancement,
                SpellEffectType.Growth => SpellCategory.Restoration,
                SpellEffectType.Barrier => SpellCategory.Defense,
                SpellEffectType.Hybrid => SpellCategory.Utility,
                _ => SpellCategory.Utility
            };

            // Enhanced categorization based on repeating elements
            if (maxRepeats >= 3)
            {
                // High repetition suggests specialization
                if (dominantElements.Contains(ElementType.Fire))
                    category = SpellCategory.Combat; // Concentrated fire = offensive
                else if (dominantElements.Contains(ElementType.Earth))
                    category = SpellCategory.Defense; // Concentrated earth = defensive
                else if (dominantElements.Contains(ElementType.Water) || dominantElements.Contains(ElementType.Wood))
                    category = SpellCategory.Restoration; // Concentrated water/wood = healing
                else if (dominantElements.Contains(ElementType.Metal))
                    category = SpellCategory.Enhancement; // Concentrated metal = precision/enhancement
            }

            // Special combinations based on element presence and repetition
            if (elements.Contains(ElementType.Void) || elements.Contains(ElementType.Forge))
                category = SpellCategory.Transmutation;
            
            if (elements.Contains(ElementType.Light) && elements.Contains(ElementType.Water))
                category = SpellCategory.Restoration;
                
            // Multiple different Forge/Void elements = advanced transmutation
            if (elementCounts.ContainsKey(ElementType.Forge) && elementCounts.ContainsKey(ElementType.Void))
                category = SpellCategory.Transmutation;
                
            // Heavy chaos presence = unpredictable utility
            if (elementCounts.ContainsKey(ElementType.Chaos) && elementCounts[ElementType.Chaos] >= 2)
                category = SpellCategory.Utility;

            return category;
        }

        /// <summary>
        /// Shows the spell prediction results
        /// </summary>
        private static void ShowSpellPredictionResults(List<SpellPrediction> predictions)
        {
            Console.WriteLine("🎯 SPELL PREDICTION RESULTS");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Show only statistics, not individual spells (to avoid console clutter)
            ShowSpellStatistics(predictions);
        }

        /// <summary>
        /// Shows spell prediction statistics
        /// </summary>
        private static void ShowSpellStatistics(List<SpellPrediction> predictions)
        {
            Console.WriteLine("📊 SPELL STATISTICS");
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Category distribution
            var categoryStats = predictions.GroupBy(p => p.SpellCategory)
                .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = (double)g.Count() / predictions.Count * 100 })
                .OrderByDescending(s => s.Count);

            Console.WriteLine("Spell Categories:");
            foreach (var stat in categoryStats)
            {
                Console.WriteLine($"  {stat.Category}: {stat.Count} spells ({stat.Percentage:F1}%)");
            }
            Console.WriteLine();

            // Effect type distribution
            var effectStats = predictions.GroupBy(p => p.EffectType)
                .Select(g => new { Effect = g.Key, Count = g.Count() })
                .OrderByDescending(s => s.Count);

            Console.WriteLine("Effect Types:");
            foreach (var stat in effectStats)
            {
                Console.WriteLine($"  {stat.Effect}: {stat.Count} spells");
            }
            Console.WriteLine();

            // Power analysis
            var avgPower = predictions.Average(p => p.Power);
            var maxPower = predictions.Max(p => p.Power);
            var highPowerSpells = predictions.Count(p => p.Power > avgPower * 1.5);

            Console.WriteLine($"Power Analysis:");
            Console.WriteLine($"  Average Power: {avgPower:F2}");
            Console.WriteLine($"  Maximum Power: {maxPower:F2}");
            Console.WriteLine($"  High Power Spells (>1.5x avg): {highPowerSpells}");
            Console.WriteLine();

            // Top combinations by talisman count
            Console.WriteLine("Most Effective Combinations by Talisman Count:");
            var byTalismanCount = predictions.GroupBy(p => p.Elements.Count);
            foreach (var group in byTalismanCount.OrderBy(g => g.Key))
            {
                var best = group.OrderByDescending(p => p.Power * p.Stability).First();
                Console.WriteLine($"  {group.Key} Talismans: {best.SpellName} (Power: {best.Power:F1}, Stability: {best.Stability:F3})");
            }
        }

        /// <summary>
        /// Gets the console color for a spell category
        /// </summary>
        private static ConsoleColor GetCategoryColor(SpellCategory category)
        {
            return category switch
            {
                SpellCategory.Combat => ConsoleColor.Red,
                SpellCategory.Defense => ConsoleColor.Blue,
                SpellCategory.Restoration => ConsoleColor.Green,
                SpellCategory.Enhancement => ConsoleColor.Yellow,
                SpellCategory.Utility => ConsoleColor.Cyan,
                SpellCategory.Transmutation => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
        }

        /// <summary>
        /// Represents a predicted spell from a talisman combination
        /// </summary>
        private class SpellPrediction
        {
            public List<ElementType> Elements { get; set; } = new();
            public string SpellName { get; set; } = "";
            public string SpellDescription { get; set; } = "";
            public SpellCategory SpellCategory { get; set; }
            public SpellEffectType EffectType { get; set; }
            public double Power { get; set; }
            public double Range { get; set; }
            public double Duration { get; set; }
            public double Stability { get; set; }
            public double Efficiency { get; set; }
            public double CastingTime { get; set; }
            public double ComplexityScore { get; set; }
            public bool HasCenterTalisman { get; set; }
            public ElementType? CenterElement { get; set; }
        }

        /// <summary>
        /// Categories of spells
        /// </summary>
        private enum SpellCategory
        {
            Combat,
            Defense,
            Restoration,
            Enhancement,
            Utility,
            Transmutation
        }
    }
}
