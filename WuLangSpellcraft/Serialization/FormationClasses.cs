using System;
using System.Collections.Generic;
using System.Linq;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
    /// <summary>
    /// Represents a multi-circle spell formation with ID-based connections
    /// </summary>
    public class SpellFormation
    {
        public Dictionary<string, MagicCircle> Circles { get; } = new();
        public List<FormationConnection> Connections { get; } = new();
        
        public void AddCircle(string id, MagicCircle circle)
        {
            Circles[id] = circle;
        }
        
        public void AddConnection(FormationConnection connection)
        {
            Connections.Add(connection);
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
