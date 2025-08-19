using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

namespace WuLangSpellcraft.Core.Serialization
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
            
            // Add space before elements if we have any
            if (circle.Talismans.Any())
            {
                result.Append(" ");
            }
            
            // Serialize each talisman
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
            
            // Add talisman ID if option is enabled and talisman has a meaningful name
            if (options.IncludeTalismanIds && !string.IsNullOrEmpty(talisman.Name))
            {
                // Use talisman name as ID if it looks like a valid CNF ID, otherwise use the ID
                var id = options.UseReadableIds && Utilities.IsValidCnfId(talisman.Name) ? 
                    talisman.Name : 
                    talisman.Id;
                result.Append($":{id}");
            }
            
            return result.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Serializable representation of an Element
    /// </summary>
    public class ElementData
    {
        public ElementType Type { get; set; }
        public double Energy { get; set; }
        public bool IsActive { get; set; }

        public ElementData() { }

        public ElementData(Element element)
        {
            Type = element.Type;
            Energy = element.Energy;
            IsActive = element.IsActive;
        }

        public Element ToElement()
        {
            return new Element(Type, Energy) { IsActive = IsActive };
        }
    }

    /// <summary>
    /// Serializable representation of a Talisman
    /// </summary>
    public class TalismanData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ElementData PrimaryElement { get; set; } = new();
        public List<ElementData> SecondaryElements { get; set; } = new();
        public double PowerLevel { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public TalismanData() { }

        public TalismanData(Talisman talisman)
        {
            Id = talisman.Id;
            Name = talisman.Name;
            PrimaryElement = new ElementData(talisman.PrimaryElement);
            SecondaryElements = talisman.SecondaryElements.Select(e => new ElementData(e)).ToList();
            PowerLevel = talisman.PowerLevel;
            X = talisman.X;
            Y = talisman.Y;
            Z = talisman.Z;
        }

        public Talisman ToTalisman()
        {
            var talisman = new Talisman(PrimaryElement.ToElement(), Name);
            
            // Restore secondary elements
            foreach (var secondaryData in SecondaryElements)
            {
                talisman.AddSecondaryElement(secondaryData.ToElement());
            }
            
            // Restore position
            talisman.X = X;
            talisman.Y = Y;
            talisman.Z = Z;
            
            return talisman;
        }
    }

    /// <summary>
    /// Serializable representation of a Circle Connection
    /// </summary>
    public class CircleConnectionData
    {
        public string SourceId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public ConnectionType Type { get; set; }
        public double Strength { get; set; }
        public bool IsActive { get; set; }

        public CircleConnectionData() { }

        public CircleConnectionData(CircleConnection connection)
        {
            SourceId = connection.Source.Id;
            TargetId = connection.Target.Id;
            Type = connection.Type;
            Strength = connection.Strength;
            IsActive = connection.IsActive;
        }
    }

    /// <summary>
    /// Serializable representation of a Magic Circle
    /// </summary>
    public class MagicCircleData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<TalismanData> Talismans { get; set; } = new();
        public double Radius { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Layer { get; set; }
        public bool IsActive { get; set; }

        public MagicCircleData() { }

        public MagicCircleData(MagicCircle circle)
        {
            Id = circle.Id;
            Name = circle.Name;
            Talismans = circle.Talismans.Select(t => new TalismanData(t)).ToList();
            Radius = circle.Radius;
            CenterX = circle.CenterX;
            CenterY = circle.CenterY;
            Layer = circle.Layer;
            IsActive = circle.IsActive;
        }

        public MagicCircle ToMagicCircle()
        {
            var circle = new MagicCircle(Id, Name, Radius)
            {
                CenterX = CenterX,
                CenterY = CenterY,
                Layer = Layer,
                IsActive = IsActive
            };

            // Add talismans to circle
            foreach (var talismanData in Talismans)
            {
                var talisman = talismanData.ToTalisman();
                // Position is already set in ToTalisman(), just add to circle
                var angle = Math.Atan2(talisman.Y - CenterY, talisman.X - CenterX);
                circle.AddTalisman(talisman, angle);
            }

            return circle;
        }
    }

    /// <summary>
    /// Complete spell configuration with multiple circles and connections
    /// </summary>
    public class SpellConfiguration
    {
        public string Id { get; set; } = Utilities.GenerateShortId();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<MagicCircleData> Circles { get; set; } = new();
        public List<CircleConnectionData> Connections { get; set; } = new();
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";
        public Dictionary<string, object> Metadata { get; set; } = new();

        public SpellConfiguration() { }

        public SpellConfiguration(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Adds a magic circle to the spell configuration
        /// </summary>
        public void AddCircle(MagicCircle circle)
        {
            Circles.Add(new MagicCircleData(circle));
            Modified = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a connection between circles
        /// </summary>
        public void AddConnection(CircleConnection connection)
        {
            Connections.Add(new CircleConnectionData(connection));
            Modified = DateTime.UtcNow;
        }

        /// <summary>
        /// Reconstructs the complete spell with all circles and connections
        /// </summary>
        public List<MagicCircle> ReconstructSpell()
        {
            var circles = new List<MagicCircle>();
            var circleMap = new Dictionary<string, MagicCircle>();

            // First, create all circles
            foreach (var circleData in Circles)
            {
                var circle = circleData.ToMagicCircle();
                circles.Add(circle);
                circleMap[circleData.Id] = circle;
            }

            // Then, recreate connections
            foreach (var connectionData in Connections)
            {
                if (circleMap.TryGetValue(connectionData.SourceId, out var source) &&
                    circleMap.TryGetValue(connectionData.TargetId, out var target))
                {
                    var connection = source.ConnectTo(target, connectionData.Type);
                    connection.Strength = connectionData.Strength;
                    connection.IsActive = connectionData.IsActive;
                }
            }

            return circles;
        }

        /// <summary>
        /// Creates a spell configuration from existing circles
        /// </summary>
        public static SpellConfiguration FromCircles(string name, List<MagicCircle> circles, string description = "")
        {
            var config = new SpellConfiguration(name, description);
            
            foreach (var circle in circles)
            {
                config.AddCircle(circle);
                
                // Add all connections from this circle
                foreach (var connection in circle.Connections)
                {
                    // Only add if we haven't already added this connection
                    var existing = config.Connections.FirstOrDefault(c => 
                        (c.SourceId == connection.Source.Id && c.TargetId == connection.Target.Id) ||
                        (c.SourceId == connection.Target.Id && c.TargetId == connection.Source.Id));
                        
                    if (existing == null)
                    {
                        config.AddConnection(connection);
                    }
                }
            }
            
            return config;
        }
    }

    #region JSON Converters

    public class ElementConverter : JsonConverter<Element>
    {
        public override Element Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<ElementData>(ref reader, options);
            return data?.ToElement() ?? throw new JsonException("Failed to deserialize Element");
        }

        public override void Write(Utf8JsonWriter writer, Element value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new ElementData(value), options);
        }
    }

    public class TalismanConverter : JsonConverter<Talisman>
    {
        public override Talisman Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<TalismanData>(ref reader, options);
            return data?.ToTalisman() ?? throw new JsonException("Failed to deserialize Talisman");
        }

        public override void Write(Utf8JsonWriter writer, Talisman value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TalismanData(value), options);
        }
    }

    public class MagicCircleConverter : JsonConverter<MagicCircle>
    {
        public override MagicCircle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<MagicCircleData>(ref reader, options);
            return data?.ToMagicCircle() ?? throw new JsonException("Failed to deserialize MagicCircle");
        }

        public override void Write(Utf8JsonWriter writer, MagicCircle value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new MagicCircleData(value), options);
        }
    }

    #endregion

    #region Circle Notation Format (CNF) Classes

    /// <summary>
    /// Options for Circle Notation Format serialization
    /// </summary>
    public class CnfOptions
    {
        // Formatting options
        public bool IncludePowerLevels { get; set; } = true;
        public bool IncludePositions { get; set; } = false;
        public bool IncludeElementStates { get; set; } = false;
        public bool IncludeTalismanIds { get; set; } = false;
        public bool UseCompactFormat { get; set; } = false;
        public bool UseReadableIds { get; set; } = true;
        
        // Layout options
        public bool MultiLine { get; set; } = false;
        public string IndentString { get; set; } = "  ";
        public int MaxLineLength { get; set; } = 80;
        
        // Content options
        public bool IncludeComments { get; set; } = false;
        public bool ValidateOnSerialize { get; set; } = true;
    }

    /// <summary>
    /// Element symbol mappings for CNF
    /// </summary>
    public static class ElementSymbols
    {
        public static char GetSymbol(ElementType type)
        {
            return Utilities.GetElementSymbol(type);
        }

        public static ElementType GetElementType(char symbol)
        {
            return Utilities.GetElementType(symbol);
        }

        public static bool IsValidSymbol(char symbol)
        {
            return Utilities.IsValidElementSymbol(symbol);
        }
    }

    /// <summary>
    /// Exception thrown when CNF parsing fails
    /// </summary>
    public class CnfException : Exception
    {
        public int Position { get; }
        
        public CnfException(string message, int position = -1) : base(message)
        {
            Position = position;
        }
        
        public CnfException(string message, int position, Exception innerException) 
            : base(message, innerException)
        {
            Position = position;
        }
    }

    /// <summary>
    /// Token types for CNF lexical analysis
    /// </summary>
    public enum CnfTokenType
    {
        Identifier,     // Element symbols (F, W, E, etc.), talisman IDs
        Number,         // Numbers for positions, strengths, power levels
        Colon,          // :
        Semicolon,      // ;
        LeftParen,      // (
        RightParen,     // )
        Comma,          // ,
        Equals,         // =
        EOF
    }

    /// <summary>
    /// Token for CNF lexical analysis
    /// </summary>
    public record CnfToken(CnfTokenType Type, string Value, int Position);

    /// <summary>
    /// Lexical analyzer for CNF
    /// </summary>
    public class CnfLexer
    {
        public IEnumerable<CnfToken> Tokenize(string input)
        {
            var position = 0;
            
            while (position < input.Length)
            {
                var current = input[position];
                
                if (char.IsWhiteSpace(current))
                {
                    position++;
                    continue;
                }
                
                switch (current)
                {
                    case ':':
                        yield return new CnfToken(CnfTokenType.Colon, ":", position++);
                        break;
                    case ';':
                        yield return new CnfToken(CnfTokenType.Semicolon, ";", position++);
                        break;
                    case '(':
                        yield return new CnfToken(CnfTokenType.LeftParen, "(", position++);
                        break;
                    case ')':
                        yield return new CnfToken(CnfTokenType.RightParen, ")", position++);
                        break;
                    case ',':
                        yield return new CnfToken(CnfTokenType.Comma, ",", position++);
                        break;
                    case '=':
                        yield return new CnfToken(CnfTokenType.Equals, "=", position++);
                        break;
                    default:
                        if (char.IsDigit(current) || current == '.')
                        {
                            var (number, newPos) = Utilities.ReadNumber(input, position);
                            yield return new CnfToken(CnfTokenType.Number, number, position);
                            position = newPos;
                        }
                        else if (char.IsLetter(current) || current == '_')
                        {
                            var (identifier, newPos) = Utilities.ReadIdentifier(input, position);
                            yield return new CnfToken(CnfTokenType.Identifier, identifier, position);
                            position = newPos;
                        }
                        else
                        {
                            throw new CnfException($"Unexpected character '{current}' at position {position}", position);
                        }
                        break;
                }
            }
            
            yield return new CnfToken(CnfTokenType.EOF, "", position);
        }
    }

    /// <summary>
    /// Parser for Circle Notation Format
    /// </summary>
    public class CnfParser
    {
        private List<CnfToken> _tokens = new();
        private int _currentToken = 0;

        public MagicCircle ParseCircle(string cnf)
        {
            var lexer = new CnfLexer();
            _tokens = lexer.Tokenize(cnf).ToList();
            _currentToken = 0;

            return ParseCircleDefinition();
        }

        private MagicCircle ParseCircleDefinition()
        {
            // Expect: C<radius> <elements>
            var token = Current();
            
            if (token.Type != CnfTokenType.Identifier || !token.Value.ToUpperInvariant().StartsWith("C"))
            {
                throw new CnfException($"Expected circle definition 'C<radius>' at position {token.Position}", token.Position);
            }

            // Extract radius
            var radiusStr = token.Value[1..]; // Remove 'C' prefix
            if (!double.TryParse(radiusStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var radius))
            {
                throw new CnfException($"Invalid radius '{radiusStr}' at position {token.Position}", token.Position);
            }

            Advance(); // Move past circle definition

            // Create circle
            var circle = new MagicCircle($"Circle R{radius}", radius);

            // Parse elements
            while (Current().Type != CnfTokenType.EOF)
            {
                var talisman = ParseTalisman();
                if (talisman != null)
                {
                    // Calculate position angle based on talisman index
                    var angle = (2 * Math.PI * circle.Talismans.Count) / 8; // Assume 8 positions for now
                    circle.AddTalisman(talisman, angle);
                }
            }

            return circle;
        }

        private Talisman? ParseTalisman()
        {
            var token = Current();
            
            if (token.Type != CnfTokenType.Identifier)
            {
                return null;
            }

            // Check if this is a compact format like "F2.5W1.2L0.8"
            if (token.Value.Length > 1)
            {
                // Look for element letters in the token
                var elementPositions = new List<int>();
                for (int i = 0; i < token.Value.Length; i++)
                {
                    if (ElementSymbols.IsValidSymbol(token.Value[i]))
                    {
                        elementPositions.Add(i);
                    }
                }

                // If we have multiple elements in one token, split and requeue
                if (elementPositions.Count > 1)
                {
                    var remainingTokens = new List<CnfToken>();
                    
                    for (int i = 0; i < elementPositions.Count; i++)
                    {
                        var start = elementPositions[i];
                        var end = i + 1 < elementPositions.Count ? elementPositions[i + 1] : token.Value.Length;
                        var elementPart = token.Value[start..end];
                        
                        remainingTokens.Add(new CnfToken(CnfTokenType.Identifier, elementPart, token.Position + start));
                    }
                    
                    // Replace current token and insert remaining ones
                    _tokens[_currentToken] = remainingTokens[0];
                    for (int i = 1; i < remainingTokens.Count; i++)
                    {
                        _tokens.Insert(_currentToken + i, remainingTokens[i]);
                    }
                    
                    // Re-parse with the first split token
                    token = Current();
                }
            }

            // Handle simple compact format (all element symbols, like "FWEMO")
            if (token.Value.Length > 1 && token.Value.All(ElementSymbols.IsValidSymbol))
            {
                // This is simple compact format - parse first element and modify token
                var firstSymbol = token.Value[0];
                var elementType = ElementSymbols.GetElementType(firstSymbol);
                
                // Update the token to contain the remaining symbols
                if (token.Value.Length > 1)
                {
                    _tokens[_currentToken] = token with { Value = token.Value[1..] };
                }
                else
                {
                    Advance();
                }
                
                // Create element and talisman for first symbol
                var element = new Element(elementType, 1.0);
                var talisman = new Talisman(element, $"Talisman {elementType}");
                
                return talisman;
            }

            // Regular parsing for single elements with potential power levels and IDs
            var elementSymbol = token.Value[0];
            if (!ElementSymbols.IsValidSymbol(elementSymbol))
            {
                throw new CnfException($"Invalid element symbol '{elementSymbol}' at position {token.Position}", token.Position);
            }

            var elementTypeRegular = ElementSymbols.GetElementType(elementSymbol);
            var powerLevel = 1.0;
            string? talismanId = null;

            // Check if there's a power level in the same token (e.g., "F2.5")
            if (token.Value.Length > 1)
            {
                var remaining = token.Value[1..];
                
                // Try to parse the entire remaining string as a number
                if (double.TryParse(remaining, NumberStyles.Float, CultureInfo.InvariantCulture, out var power))
                {
                    powerLevel = power;
                }
            }

            Advance(); // Move past element token

            // Check for power level as separate number token
            if (Current().Type == CnfTokenType.Number)
            {
                if (double.TryParse(Current().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var separatePower))
                {
                    powerLevel = separatePower;
                }
                Advance();
            }

            // Check for talisman ID
            if (Current().Type == CnfTokenType.Colon)
            {
                Advance(); // Skip colon
                
                if (Current().Type == CnfTokenType.Identifier)
                {
                    talismanId = Current().Value;
                    Advance();
                }
                else
                {
                    throw new CnfException($"Expected talisman ID after ':' at position {Current().Position}", Current().Position);
                }
            }

            // Create element and talisman
            var elementRegular = new Element(elementTypeRegular, powerLevel);
            var talismanName = talismanId ?? $"Talisman {elementTypeRegular}";
            var talismanRegular = new Talisman(elementRegular, talismanName);

            return talismanRegular;
        }

        private CnfToken Current()
        {
            return _currentToken < _tokens.Count ? _tokens[_currentToken] : new CnfToken(CnfTokenType.EOF, "", -1);
        }

        private void Advance()
        {
            if (_currentToken < _tokens.Count)
            {
                _currentToken++;
            }
        }
    }

    #endregion
}
