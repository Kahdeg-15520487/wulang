using System;
using System.Collections.Generic;
using System.Linq;

namespace WuLangSpellcraft.Core
{
    /// <summary>
    /// Represents a multi-circle spell formation with ID-based connections
    /// This is the main Formation entity that sits between Artifacts and MagicCircles
    /// </summary>
    public class Formation
    {
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, MagicCircle> Circles { get; }
        public List<FormationConnection> Connections { get; }
        
        // Formation-level properties
        public double TotalPowerOutput { get; private set; }
        public double OverallStability { get; private set; }
        public double FormationComplexity { get; private set; }
        public double CastingTime { get; private set; }
        public FormationType Type { get; private set; }
        
        // Metadata
        public DateTime CreatedAt { get; }
        public string CreatedBy { get; set; }
        public int Version { get; set; }

        public Formation(string name, string description = "")
        {
            Id = Utilities.GenerateShortId();
            Name = name;
            Description = description;
            Circles = new Dictionary<string, MagicCircle>();
            Connections = new List<FormationConnection>();
            CreatedAt = DateTime.Now;
            CreatedBy = "Unknown";
            Version = 1;
            Type = FormationType.Simple;
        }

        /// <summary>
        /// Adds a circle to this formation with a specific ID
        /// </summary>
        public void AddCircle(string id, MagicCircle circle)
        {
            Circles[id] = circle;
            RecalculateFormationProperties();
        }

        /// <summary>
        /// Adds a circle to this formation with auto-generated ID
        /// </summary>
        public string AddCircle(MagicCircle circle)
        {
            var id = circle.Id;
            AddCircle(id, circle);
            return id;
        }

        /// <summary>
        /// Removes a circle from this formation
        /// </summary>
        public bool RemoveCircle(string id)
        {
            if (Circles.Remove(id))
            {
                // Remove connections involving this circle
                Connections.RemoveAll(c => c.SourceId == id || c.TargetId == id);
                RecalculateFormationProperties();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a connection between circles in this formation
        /// </summary>
        public void AddConnection(string sourceId, string targetId, ConnectionType type, double strength = 1.0)
        {
            var connection = new FormationConnection
            {
                SourceId = sourceId,
                TargetId = targetId,
                Type = type,
                Strength = strength
            };
            Connections.Add(connection);
            RecalculateFormationProperties();
        }

        /// <summary>
        /// Adds a connection to this formation
        /// </summary>
        public void AddConnection(FormationConnection connection)
        {
            Connections.Add(connection);
            RecalculateFormationProperties();
        }

        /// <summary>
        /// Resolves all connections and creates actual CircleConnection objects
        /// </summary>
        public void ResolveConnections()
        {
            foreach (var formationConnection in Connections)
            {
                if (Circles.TryGetValue(formationConnection.SourceId, out var sourceCircle) &&
                    Circles.TryGetValue(formationConnection.TargetId, out var targetCircle))
                {
                    var connection = sourceCircle.ConnectTo(targetCircle, formationConnection.Type);
                    connection.Strength = formationConnection.Strength;
                }
                else
                {
                    // Handle talisman-to-talisman connections
                    ResolveTalismanConnection(formationConnection);
                }
            }
            
            RecalculateFormationProperties();
        }

        /// <summary>
        /// Calculates and updates all formation-level properties
        /// </summary>
        private void RecalculateFormationProperties()
        {
            if (Circles.Count == 0)
            {
                TotalPowerOutput = 0;
                OverallStability = 0;
                FormationComplexity = 0;
                CastingTime = 0;
                Type = FormationType.Empty;
                return;
            }

            // Calculate total power output
            TotalPowerOutput = Circles.Values.Sum(c => c.GetCompositionPowerOutput());

            // Calculate overall stability (weighted average)
            var totalCircleWeight = Circles.Values.Sum(c => c.Talismans.Count + 1);
            OverallStability = Circles.Values.Sum(c => c.GetCompositionStability() * (c.Talismans.Count + 1)) / totalCircleWeight;

            // Calculate formation complexity
            FormationComplexity = CalculateFormationComplexity();

            // Calculate casting time
            CastingTime = CalculateFormationCastingTime();

            // Determine formation type
            Type = DetermineFormationType();
        }

        /// <summary>
        /// Calculates the complexity score for the entire formation
        /// </summary>
        private double CalculateFormationComplexity()
        {
            var baseComplexity = Circles.Count * 2.0; // Base complexity per circle
            var connectionComplexity = Connections.Count * 1.5; // Complexity per connection
            var circleComplexity = Circles.Values.Sum(c => c.CalculateComplexityScore());

            // Formation synergy bonus/penalty
            var synergyFactor = CalculateSynergyFactor();
            
            return (baseComplexity + connectionComplexity + circleComplexity) * synergyFactor;
        }

        /// <summary>
        /// Calculates how well the circles work together
        /// </summary>
        private double CalculateSynergyFactor()
        {
            if (Circles.Count <= 1) return 1.0;

            var synergy = 1.0;
            var circles = Circles.Values.ToList();

            // Check elemental harmony between circles
            for (int i = 0; i < circles.Count; i++)
            {
                for (int j = i + 1; j < circles.Count; j++)
                {
                    var circle1 = circles[i];
                    var circle2 = circles[j];
                    
                    var element1 = circle1.GetDominantElement();
                    var element2 = circle2.GetDominantElement();
                    
                    var relation = Element.GetElementRelation(element1, element2);
                    synergy += relation switch
                    {
                        ElementRelation.Generates => 0.1,  // Positive synergy
                        ElementRelation.Destroys => -0.2,  // Negative synergy
                        ElementRelation.Neutral => 0.0,    // No change
                        _ => 0.0
                    };
                }
            }

            return Math.Max(0.5, Math.Min(2.0, synergy)); // Clamp between 0.5 and 2.0
        }

        /// <summary>
        /// Calculates the total casting time for the formation
        /// </summary>
        private double CalculateFormationCastingTime()
        {
            if (Circles.Count == 0) return 0;

            // For parallel circles, use the longest casting time
            // For sequential circles (connected), add them up with diminishing returns
            var maxSingleCastTime = Circles.Values.Max(c => c.CalculateCastingTime());
            var totalComplexity = FormationComplexity;
            
            // Base time is the slowest individual circle plus complexity overhead
            var baseTime = maxSingleCastTime + (totalComplexity * 0.2);
            
            // Formation type modifier
            var typeModifier = Type switch
            {
                FormationType.Simple => 1.0,
                FormationType.Parallel => 1.1,      // Slight coordination overhead
                FormationType.Sequential => 1.4,    // Must wait for each step
                FormationType.Hierarchical => 1.6,  // Complex coordination
                FormationType.Network => 1.8,       // Maximum coordination complexity
                FormationType.Hybrid => 2.0,        // Most complex
                _ => 1.0
            };

            return baseTime * typeModifier;
        }

        /// <summary>
        /// Determines the type of formation based on structure
        /// </summary>
        private FormationType DetermineFormationType()
        {
            if (Circles.Count == 0) return FormationType.Empty;
            if (Circles.Count == 1) return FormationType.Simple;

            var hasNested = Circles.Values.Any(c => c.NestedCircles.Count > 0);
            var hasStacked = Circles.Values.Any(c => c.Layer > 0);
            var hasConnections = Connections.Count > 0;

            return (hasNested, hasStacked, hasConnections) switch
            {
                (true, true, true) => FormationType.Hybrid,
                (true, false, true) => FormationType.Network,
                (false, true, true) => FormationType.Network,
                (true, true, false) => FormationType.Hierarchical,
                (false, false, true) => FormationType.Network,
                (true, false, false) => FormationType.Hierarchical,
                (false, true, false) => FormationType.Parallel,
                _ => FormationType.Sequential
            };
        }

        /// <summary>
        /// Gets a summary of this formation
        /// </summary>
        public string GetSummary()
        {
            return $"Formation '{Name}': {Circles.Count} circles, {Connections.Count} connections\n" +
                   $"Type: {Type}, Power: {TotalPowerOutput:F1}, Stability: {OverallStability:F2}\n" +
                   $"Complexity: {FormationComplexity:F1}, Cast Time: {CastingTime:F1}s";
        }

        /// <summary>
        /// Creates a deep copy of this formation
        /// </summary>
        public Formation Clone()
        {
            var clone = new Formation(Name + " (Copy)", Description)
            {
                CreatedBy = CreatedBy,
                Version = Version + 1
            };

            // Clone all circles
            foreach (var (id, circle) in Circles)
            {
                clone.AddCircle(id, circle.Clone());
            }

            // Clone all connections
            foreach (var connection in Connections)
            {
                clone.AddConnection(new FormationConnection
                {
                    SourceId = connection.SourceId,
                    TargetId = connection.TargetId,
                    Type = connection.Type,
                    Strength = connection.Strength
                });
            }

            return clone;
        }

        public override string ToString()
        {
            return $"{Name} ({Type}): {Circles.Count} circles, Power: {TotalPowerOutput:F1}";
        }

        private void ResolveTalismanConnection(FormationConnection formationConnection)
        {
            // Find talisman by ID across all circles
            Talisman? sourceTalisman = null;
            Talisman? targetTalisman = null;
            MagicCircle? sourceCircle = null;
            MagicCircle? targetCircle = null;
            
            foreach (var (circleId, circle) in Circles)
            {
                if (sourceTalisman == null)
                {
                    sourceTalisman = FindTalismanById(circle, formationConnection.SourceId);
                    if (sourceTalisman != null) sourceCircle = circle;
                }
                
                if (targetTalisman == null)
                {
                    targetTalisman = FindTalismanById(circle, formationConnection.TargetId);
                    if (targetTalisman != null) targetCircle = circle;
                }
            }
            
            // If we found both talismans, create a circle connection
            // (In a more advanced implementation, we might have talisman-specific connections)
            if (sourceTalisman != null && targetTalisman != null && sourceCircle != null && targetCircle != null)
            {
                var connection = sourceCircle.ConnectTo(targetCircle, formationConnection.Type);
                connection.Strength = formationConnection.Strength;
            }
        }
        
        private Talisman? FindTalismanById(MagicCircle circle, string talismanId)
        {
            // Check edge talismans
            var talisman = circle.Talismans.FirstOrDefault(t => t.Id == talismanId || t.Name == talismanId);
            if (talisman != null) return talisman;
            
            // Check center talisman
            if (circle.CenterTalisman != null && 
                (circle.CenterTalisman.Id == talismanId || circle.CenterTalisman.Name == talismanId))
            {
                return circle.CenterTalisman;
            }
            
            return null;
        }
    }

    /// <summary>
    /// Types of formations based on their structure and connection patterns
    /// </summary>
    public enum FormationType
    {
        Empty,          // No circles
        Simple,         // Single circle
        Parallel,       // Multiple independent circles (stacked)
        Sequential,     // Circles that activate in sequence
        Hierarchical,   // Nested circles with parent-child relationships
        Network,        // Interconnected circles with multiple connections
        Hybrid          // Complex combination of multiple patterns
    }

    /// <summary>
    /// Represents a connection in a formation before it's resolved to actual CircleConnection objects
    /// </summary>
    public class FormationConnection
    {
        public string SourceId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public ConnectionType Type { get; set; }
        public double Strength { get; set; } = 1.0;
    }
}
