using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Globalization;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
    /// <summary>
    /// Handles serialization and deserialization of spell configurations
    /// </summary>
    public static class SpellSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = 
            {
                new JsonStringEnumConverter(),
                new ElementConverter(),
                new TalismanConverter(),
                new MagicCircleConverter()
            }
        };

        #region JSON Serialization

        /// <summary>
        /// Serializes a magic circle to JSON string
        /// </summary>
        public static string SerializeCircle(MagicCircle circle)
        {
            var data = new MagicCircleData(circle);
            return JsonSerializer.Serialize(data, SerializerOptions);
        }

        /// <summary>
        /// Deserializes a magic circle from JSON string
        /// </summary>
        public static MagicCircle DeserializeCircle(string json)
        {
            var data = JsonSerializer.Deserialize<MagicCircleData>(json, SerializerOptions);
            return data?.ToMagicCircle() ?? throw new InvalidOperationException("Failed to deserialize magic circle");
        }

        /// <summary>
        /// Saves a magic circle to file
        /// </summary>
        public static async Task SaveCircleToFileAsync(MagicCircle circle, string filePath)
        {
            var json = SerializeCircle(circle);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Loads a magic circle from file
        /// </summary>
        public static async Task<MagicCircle> LoadCircleFromFileAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return DeserializeCircle(json);
        }

        /// <summary>
        /// Serializes a complete spell (multiple connected circles) to JSON
        /// </summary>
        public static string SerializeSpell(SpellConfiguration spell)
        {
            return JsonSerializer.Serialize(spell, SerializerOptions);
        }

        /// <summary>
        /// Deserializes a complete spell from JSON
        /// </summary>
        public static SpellConfiguration DeserializeSpell(string json)
        {
            return JsonSerializer.Deserialize<SpellConfiguration>(json, SerializerOptions) 
                ?? throw new InvalidOperationException("Failed to deserialize spell configuration");
        }

        /// <summary>
        /// Saves a complete spell configuration to file
        /// </summary>
        public static async Task SaveSpellToFileAsync(SpellConfiguration spell, string filePath)
        {
            var json = SerializeSpell(spell);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Loads a complete spell configuration from file
        /// </summary>
        public static async Task<SpellConfiguration> LoadSpellFromFileAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return DeserializeSpell(json);
        }

        #endregion

        #region Circle Notation Format (CNF) Support

        /// <summary>
        /// Serializes a magic circle to Circle Notation Format (CNF)
        /// </summary>
        public static string SerializeCircleToCnf(MagicCircle circle, CnfOptions? options = null)
        {
            options ??= new CnfOptions();
            
            var result = new StringBuilder();
            
            // Start with circle definition: C<radius>
            result.Append($"C{circle.Radius}");
            
            // Add space before talismans if we have any
            if (circle.CenterTalisman != null || circle.Talismans.Any())
            {
                result.Append(" ");
            }
            
            // Serialize center talisman first if it exists
            if (circle.CenterTalisman != null)
            {
                result.Append("@");
                result.Append(SerializeTalismanToCnf(circle.CenterTalisman, options));
                
                // Add space before ring talismans if we have any
                if (circle.Talismans.Any())
                {
                    result.Append(" ");
                }
            }
            
            // Serialize each ring talisman
            for (int i = 0; i < circle.Talismans.Count; i++)
            {
                var talisman = circle.Talismans[i];
                var elementStr = SerializeTalismanToCnf(talisman, options);
                result.Append(elementStr);
                
                // Add separator between elements (except for last one)
                if (i < circle.Talismans.Count - 1 && !options.UseCompactFormat)
                {
                    result.Append(" ");
                }
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Deserializes a magic circle from Circle Notation Format (CNF)
        /// </summary>
        public static MagicCircle DeserializeCircleFromCnf(string cnf)
        {
            var parser = new CnfParser();
            return parser.ParseCircle(cnf);
        }

        /// <summary>
        /// Deserializes a spell formation (multi-circle) from Circle Notation Format (CNF)
        /// </summary>
        public static Formation DeserializeFormationFromCnf(string cnf)
        {
            var parser = new MultiCircleCnfParser();
            return parser.ParseFormation(cnf);
        }

        /// <summary>
        /// Detects whether CNF contains single circle or multi-circle formation
        /// </summary>
        public static bool IsMultiCircleCnf(string cnf)
        {
            var multiParser = new MultiCircleCnfParser();
            return multiParser.IsMultiCircleFormat(cnf);
        }

        /// <summary>
        /// Saves a magic circle to file in CNF format
        /// </summary>
        public static async Task SaveCircleToCnfFileAsync(MagicCircle circle, string filePath, CnfOptions? options = null)
        {
            var cnf = SerializeCircleToCnf(circle, options);
            await File.WriteAllTextAsync(filePath, cnf);
        }

        /// <summary>
        /// Loads a magic circle from CNF file
        /// </summary>
        public static async Task<MagicCircle> LoadCircleFromCnfFileAsync(string filePath)
        {
            var cnf = await File.ReadAllTextAsync(filePath);
            return DeserializeCircleFromCnf(cnf);
        }

        private static string SerializeTalismanToCnf(Talisman talisman, CnfOptions options)
        {
            var result = new StringBuilder();
            
            // Get element symbol
            var symbol = ElementSymbols.GetSymbol(talisman.PrimaryElement.Type);
            result.Append(symbol);
            
            // Add power level if different from 1.0 and option is enabled
            if (options.IncludePowerLevels && Math.Abs(talisman.PrimaryElement.Energy - 1.0) > 0.001)
            {
                result.Append(talisman.PrimaryElement.Energy.ToString("0.##", CultureInfo.InvariantCulture));
            }
            
            // Add element state symbol if not normal
            if (talisman.PrimaryElement.State != ElementState.Normal)
            {
                var stateSymbol = talisman.PrimaryElement.State switch
                {
                    ElementState.Active => "*",
                    ElementState.Unstable => "?",
                    ElementState.Damaged => "!",
                    ElementState.Resonating => "~",
                    _ => ""
                };
                result.Append(stateSymbol);
            }
            
            // Add talisman ID if option is enabled and talisman has a meaningful name
            if (options.IncludeTalismanIds && !string.IsNullOrEmpty(talisman.Name))
            {
                // Use talisman name as ID if it looks like a valid CNF ID, otherwise use the ID
                var id = options.UseReadableIds && Core.Utilities.IsValidCnfId(talisman.Name) ? 
                    talisman.Name : 
                    talisman.Id;
                result.Append($":{id}");
            }
            
            return result.ToString();
        }

        #endregion
    }
}
