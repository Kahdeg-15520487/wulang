using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;

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
        public Guid Id { get; set; }
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
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
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
        public Guid Id { get; set; }
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
            var circle = new MagicCircle(Name, Radius)
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
        public Guid Id { get; set; } = Guid.NewGuid();
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
            var circleMap = new Dictionary<Guid, MagicCircle>();

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
}
