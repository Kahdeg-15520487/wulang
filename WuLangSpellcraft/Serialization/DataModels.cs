using System;
using System.Collections.Generic;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
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
        public TalismanData? CenterTalisman { get; set; }
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
            CenterTalisman = circle.CenterTalisman != null ? new TalismanData(circle.CenterTalisman) : null;
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

            // Add center talisman if present
            if (CenterTalisman != null)
            {
                var centerTalisman = CenterTalisman.ToTalisman();
                circle.SetCenterTalisman(centerTalisman);
            }

            return circle;
        }
    }

    /// <summary>
    /// Complete spell configuration with multiple circles and connections
    /// </summary>
    public class SpellConfiguration
    {
        public string Id { get; set; } = Core.Utilities.GenerateShortId();
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
}
