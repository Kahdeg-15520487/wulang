using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Demo;

/// <summary>
/// Interactive demonstration of circle composition in the GUI environment
/// </summary>
public static class InteractiveCompositionDemo
{
    /// <summary>
    /// Creates a defensive layered composition example
    /// </summary>
    public static MagicCircle CreateDefensiveLayeredExample()
    {
        var mainCircle = new MagicCircle("Defensive Fortress", 8.0);
        
        // Main circle - Earth-based defense
        var earthTalismans = new[]
        {
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Stone Barrier") { PowerLevel = 3.0 },
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Earthen Wall") { PowerLevel = 2.8 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Iron Shield") { PowerLevel = 3.2 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Steel Reinforcement") { PowerLevel = 2.9 }
        };
        
        foreach (var talisman in earthTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Nested circle - Metal reinforcement
        var nestedCircle = new MagicCircle("Metal Core", 4.0);
        var metalTalismans = new[]
        {
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Adamantine Core") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Reinforced Lattice") { PowerLevel = 3.5 },
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Crystalline Support") { PowerLevel = 3.0 }
        };
        
        foreach (var talisman in metalTalismans)
        {
            nestedCircle.AddTalisman(talisman);
        }
        
        mainCircle.NestCircle(nestedCircle, 0.6);
        
        return mainCircle;
    }
    
    /// <summary>
    /// Creates an elemental network composition example
    /// </summary>
    public static MagicCircle CreateElementalNetworkExample()
    {
        var mainCircle = new MagicCircle("Fire Storm", 6.0);
        
        // Main circle - Fire element
        var fireTalismans = new[]
        {
            new Talisman(new Element(ElementType.Fire) { IsActive = true }, "Inferno Core") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Fire) { IsActive = true }, "Flame Burst") { PowerLevel = 3.5 },
            new Talisman(new Element(ElementType.Light) { IsActive = true }, "Solar Flare") { PowerLevel = 3.8 }
        };
        
        foreach (var talisman in fireTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Wind amplifier circle
        var windCircle = new MagicCircle("Wind Amplifier", 4.0);
        windCircle.CenterX = 12.0; // Position it to the right
        var windTalismans = new[]
        {
            new Talisman(new Element(ElementType.Wind) { IsActive = true }, "Gale Force") { PowerLevel = 3.0 },
            new Talisman(new Element(ElementType.Wind) { IsActive = true }, "Cyclone Spin") { PowerLevel = 2.8 }
        };
        
        foreach (var talisman in windTalismans)
        {
            windCircle.AddTalisman(talisman);
        }
        
        // Lightning trigger circle
        var lightningCircle = new MagicCircle("Lightning Trigger", 3.5);
        lightningCircle.CenterX = -10.0; // Position it to the left
        lightningCircle.CenterY = 8.0;
        var lightningTalismans = new[]
        {
            new Talisman(new Element(ElementType.Lightning) { IsActive = true }, "Thunder Strike") { PowerLevel = 4.5 },
            new Talisman(new Element(ElementType.Lightning) { IsActive = true }, "Electric Arc") { PowerLevel = 3.2 }
        };
        
        foreach (var talisman in lightningTalismans)
        {
            lightningCircle.AddTalisman(talisman);
        }
        
        // Create resonance connection (Fire + Wind = enhanced combustion)
        var windConnection = mainCircle.ConnectTo(windCircle, ConnectionType.Resonance);
        windConnection.Strength = 1.8;
        
        // Create trigger connection (Lightning triggers the fire storm)
        var lightningConnection = mainCircle.ConnectTo(lightningCircle, ConnectionType.Trigger);
        lightningConnection.Strength = 1.5;
        
        return mainCircle;
    }
    
    /// <summary>
    /// Creates a unified composition example combining nesting and networking
    /// </summary>
    public static MagicCircle CreateUnifiedCompositionExample()
    {
        var mainCircle = new MagicCircle("Unified Nexus", 10.0);
        
        // Main circle - Chaos element for maximum flexibility
        var chaosTalismans = new[]
        {
            new Talisman(new Element(ElementType.Chaos) { IsActive = true }, "Primordial Chaos") { PowerLevel = 5.0 },
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Void Anchor") { PowerLevel = 4.8 },
            new Talisman(new Element(ElementType.Chaos) { IsActive = true }, "Reality Flux") { PowerLevel = 4.5 }
        };
        
        foreach (var talisman in chaosTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Nested stabilizing circle
        var stabilizingCircle = new MagicCircle("Void Core", 5.0);
        var voidTalismans = new[]
        {
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Balance Point") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Null Field") { PowerLevel = 3.8 }
        };
        
        foreach (var talisman in voidTalismans)
        {
            stabilizingCircle.AddTalisman(talisman);
        }
        
        mainCircle.NestCircle(stabilizingCircle, 0.5);
        
        // Connected elemental support circles
        var elements = new[] { ElementType.Fire, ElementType.Water, ElementType.Earth, ElementType.Metal, ElementType.Wood };
        var connectionTypes = new[] { ConnectionType.Direct, ConnectionType.Resonance, ConnectionType.Flow, ConnectionType.Direct, ConnectionType.Resonance };
        
        for (int i = 0; i < elements.Length; i++)
        {
            var supportCircle = new MagicCircle($"{elements[i]} Support", 3.0);
            
            // Position in a circle around the main circle
            var angle = i * 2 * Math.PI / elements.Length;
            supportCircle.CenterX = mainCircle.CenterX + 16 * Math.Cos(angle);
            supportCircle.CenterY = mainCircle.CenterY + 16 * Math.Sin(angle);
            
            var elementTalisman = new Talisman(new Element(elements[i]) { IsActive = true }, $"{elements[i]} Essence")
            {
                PowerLevel = 3.0 + i * 0.2
            };
            supportCircle.AddTalisman(elementTalisman);
            
            var connection = mainCircle.ConnectTo(supportCircle, connectionTypes[i]);
            connection.Strength = 1.2 + (i * 0.1);
        }
        
        return mainCircle;
    }
    
    /// <summary>
    /// Provides analysis and recommendations for composition examples
    /// </summary>
    public static string AnalyzeComposition(MagicCircle composition)
    {
        var analysis = $"COMPOSITION ANALYSIS: {composition.Name}\n";
        analysis += new string('=', 50) + "\n\n";
        
        analysis += $"Type: {composition.CompositionType}\n";
        analysis += $"Complexity: {composition.CalculateComplexityScore():F2}\n";
        analysis += $"Base Power: {composition.PowerOutput:F1}\n";
        analysis += $"Composition Power: {composition.GetCompositionPowerOutput():F1}\n";
        analysis += $"Casting Time: {composition.CalculateCastingTime():F1}s\n";
        analysis += $"Stability: {composition.Stability:F2}\n\n";
        
        analysis += $"Nested Circles: {composition.NestedCircles.Count}\n";
        analysis += $"Connections: {composition.Connections.Count}\n\n";
        
        if (composition.NestedCircles.Any())
        {
            analysis += "NESTED STRUCTURE:\n";
            foreach (var nested in composition.NestedCircles)
            {
                analysis += $"  - {nested.Name} (Radius: {nested.Radius:F1}, Talismans: {nested.Talismans.Count})\n";
            }
            analysis += "\n";
        }
        
        if (composition.Connections.Any())
        {
            analysis += "CONNECTION NETWORK:\n";
            foreach (var connection in composition.Connections)
            {
                analysis += $"  - {connection}\n";
            }
            analysis += "\n";
        }
        
        // Generate strategic recommendations
        analysis += "STRATEGIC ASSESSMENT:\n";
        var complexity = composition.CalculateComplexityScore();
        var stability = composition.Stability;
        
        if (complexity > 3.0)
        {
            analysis += "⚠️  High complexity - consider simplifying for reliable casting\n";
        }
        else if (complexity < 1.5)
        {
            analysis += "✅ Low complexity - room for additional features\n";
        }
        else
        {
            analysis += "✅ Balanced complexity - good risk/reward ratio\n";
        }
        
        if (stability < 0.6)
        {
            analysis += "⚠️  Low stability - balance elements or reduce connections\n";
        }
        else if (stability > 0.8)
        {
            analysis += "✅ High stability - very reliable casting\n";
        }
        else
        {
            analysis += "✅ Good stability - acceptable risk level\n";
        }
        
        var powerBonus = composition.GetCompositionPowerOutput() - composition.PowerOutput;
        if (powerBonus > composition.PowerOutput * 0.5)
        {
            analysis += "🚀 Excellent power amplification from composition\n";
        }
        else if (powerBonus > 0)
        {
            analysis += "✅ Moderate power gain from composition\n";
        }
        else
        {
            analysis += "⚠️  No power gain - composition may be inefficient\n";
        }
        
        return analysis;
    }
}
