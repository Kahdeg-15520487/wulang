# Magic Circle System Reference

## Overview

Magic Circles are the primary spell composition structures in WuLang Spellcraft. They arrange talismans in geometric patterns to create complex magical effects. The system supports multiple composition approaches including simple arrangements, 3D stacking, nested circles, networked connections, and unified combinations.

## Basic Circle Properties

### Core Properties
- **Radius**: Circle size (affects talisman capacity and range)
- **Position**: 3D coordinates (X, Y, Z) for circle placement
- **Layer**: Z-coordinate for vertical stacking
- **Talismans**: Collection of arranged talismans
- **IsActive**: Whether the circle is currently active

### Calculated Properties
- **Power Output**: Total power from all talismans and bonuses
- **Stability**: Average stability considering all interactions
- **Efficiency**: How well elements work together (0.0-1.0+)
- **Complexity Score**: Overall complexity metric
- **Casting Time**: Time required to cast based on complexity

## Composition Types

The system automatically detects and categorizes circle compositions:

### Simple Composition
- **Description**: Single circle with talismans
- **Use Case**: Basic spells, learning, reliable effects
- **Complexity**: Low (1.0-3.0)
- **Casting Time**: Fast (1-5 seconds)

### Stacked Composition
- **Description**: Multiple circles at different Z-layers
- **Use Case**: Layered effects, defense-in-depth
- **Complexity**: Medium (2.0-5.0)
- **Casting Time**: Moderate (3-8 seconds)

### Nested Composition
- **Description**: Smaller circles contained within larger ones
- **Use Case**: Focused power, protective arrangements
- **Complexity**: Medium-High (3.0-7.0)
- **Casting Time**: Moderate-Slow (5-12 seconds)

### Network Composition
- **Description**: Multiple circles connected by various link types
- **Use Case**: Distributed effects, complex interactions
- **Complexity**: High (4.0-10.0)
- **Casting Time**: Slow (8-20 seconds)

### Unified Composition
- **Description**: Combines stacking, nesting, and networking
- **Use Case**: Maximum power, ultimate spells
- **Complexity**: Very High (8.0-25.0+)
- **Casting Time**: Very Slow (15-60+ seconds)

## Circle Creation and Management

### Basic Creation
```csharp
// Create basic circle
var circle = new MagicCircle("Fire Storm", radius: 5.0);

// Set position
circle.CenterX = 10.0;
circle.CenterY = 15.0;
circle.Layer = 0.0;

// Add talismans
var fireTalisman = new Talisman(new Element(ElementType.Fire));
circle.AddTalisman(fireTalisman); // Auto-position

// Or specify angle
circle.AddTalisman(fireTalisman, angleRadians: Math.PI / 4);
```

### Talisman Management
```csharp
// Check capacity
int maxTalismans = circle.CalculateMaxTalismans(); // Based on radius

// Remove talisman
bool removed = circle.RemoveTalisman(fireTalisman);

// Get all interactions
var interactions = circle.GetTalismanInteractions();
foreach (var interaction in interactions)
{
    Console.WriteLine($"{interaction.Source.Name} -> {interaction.Target.Name}: {interaction.Resonance:F2}");
}
```

## Advanced Composition Features

### Circle Nesting
```csharp
// Create nested circle system
var mainCircle = new MagicCircle("Defensive Array", 10.0);
var innerCircle = new MagicCircle("Core Shield", 4.0);

// Nest the inner circle (60% scale)
bool nested = mainCircle.NestCircle(innerCircle, scale: 0.6);

// Access nested circles
foreach (var nested in mainCircle.NestedCircles)
{
    Console.WriteLine($"Nested: {nested.Name} at scale {nested.NestedScale}");
}

// Remove nesting
mainCircle.UnnestCircle(innerCircle);
```

### Circle Connections
```csharp
// Create network of circles
var circle1 = new MagicCircle("Fire Source");
var circle2 = new MagicCircle("Wind Amplifier");

// Create different connection types
var resonanceConnection = circle1.ConnectTo(circle2, ConnectionType.Resonance);
var directConnection = circle1.ConnectTo(circle2, ConnectionType.Direct);

// Connection properties
Console.WriteLine($"Strength: {resonanceConnection.Strength:F2}");
Console.WriteLine($"Active: {resonanceConnection.IsActive}");
```

### Connection Types
1. **Direct**: Simple energy transfer (90% efficiency)
2. **Resonance**: Harmonic amplification (120% efficiency, compatible elements)
3. **Flow**: Continuous energy circulation (70% efficiency)
4. **Trigger**: Conditional activation (50% continuous, full on trigger)

## Spell Effects and Calculations

### Spell Effect Generation
```csharp
var effect = circle.CalculateSpellEffect();

Console.WriteLine($"Type: {effect.Type}");
Console.WriteLine($"Element: {effect.Element}");
Console.WriteLine($"Power: {effect.Power:F1}");
Console.WriteLine($"Range: {effect.Range:F1}");
Console.WriteLine($"Duration: {effect.Duration:F1}");
```

### Effect Types by Element
- **Water**: Flow effects (streams, currents, fluid manipulation)
- **Fire**: Projectile effects (fireballs, energy bolts, explosions)
- **Earth**: Barrier effects (shields, walls, protective barriers)
- **Metal**: Enhancement effects (weapon enhancement, precision)
- **Wood**: Growth effects (healing, expansion, organic manipulation)
- **Hybrid**: Complex multi-element effects

### Power Calculation
Total power includes:
- Base talisman power
- Elemental interaction bonuses/penalties
- Composition bonuses (nested, connected)
- Efficiency multipliers
- Stability factors

## Complexity and Performance

### Complexity Scoring
Complexity is calculated from:
- **Base Complexity**: Number of talismans × 1.0
- **Radius Complexity**: Circle size × 0.1
- **Stacking Complexity**: Layer position × 0.3
- **Nesting Complexity**: Nested circles × 1.5 (recursive)
- **Connection Complexity**: Varies by connection type (0.5-1.5)
- **Stability Penalty**: (1.0 - stability) × 2.0

### Casting Time Calculation
```csharp
double castingTime = circle.CalculateCastingTime();

// Factors affecting casting time:
// - Base time: radius / 3.0 (minimum 1.0 second)
// - Complexity time: complexity × 0.4
// - Composition modifier: 1.0-2.5x based on type
// - Stability modifier: 1.0-2.0x (lower stability = slower)
```

### Performance Optimization
```csharp
// Get composition power output
double totalPower = circle.GetCompositionPowerOutput();

// Nested circles contribute 80% of their power
// Connected circles contribute based on connection efficiency
// Resonance connections can amplify power beyond 100%
```

## Artifact Integration

### Artifact Attachment
```csharp
// Create and attach artifacts
var chalice = new ElementalArtifact(ArtifactType.ChaliceOfFlow, 
    ElementType.Forge, ElementType.Water, "Azure Chalice");

bool attached = circle.AttachArtifact(chalice);

// Artifacts affect circle properties:
// - Power amplification
// - Stability bonuses
// - Capacity increases
// - Special effects
```

### Artifact Creation
```csharp
// Create elemental artifacts using Forge element
var artifact = circle.CreateElementalArtifact(ElementType.Forge, ElementType.Fire);

// Create spell-imbued artifacts (sacrifices circle)
var spellArtifact = circle.CreateSpellArtifact("Fireball Wand");
```

## Advanced Features

### Derived Element Support
```csharp
// Check possible derived elements
var possibleElements = circle.GetPossibleDerivedElements();

// Requires specific element combinations:
// Lightning: Fire + Metal
// Wind: Wood + Water
// Light: Fire + Wood
// Dark: Earth + Water
// Forge: Metal + Wood
// Chaos: All 5 base elements
```

### Circle Cloning
```csharp
// Create exact copy for spell artifacts
var clonedCircle = circle.Clone();

// Preserves:
// - All talisman arrangements
// - Element configurations
// - Power calculations
// - Positions and properties
```

### Serialization Support
```csharp
// Save circle configuration
string json = SpellSerializer.SerializeCircle(circle);
await SpellSerializer.SaveCircleToFileAsync(circle, "my_circle.json");

// Load circle configuration
var loadedCircle = await SpellSerializer.LoadCircleFromFileAsync("my_circle.json");
```

## Best Practices

### Circle Design
1. **Start Simple**: Begin with single circles before adding complexity
2. **Element Balance**: Use complementary elements for stability
3. **Size Appropriately**: Larger circles hold more talismans but cost more
4. **Monitor Complexity**: Keep complexity manageable for reliable casting

### Composition Strategy
1. **Layer Gradually**: Build complexity in stages
2. **Test Components**: Verify each component before combining
3. **Balance Power vs Stability**: Don't sacrifice reliability for raw power
4. **Plan Connections**: Design network topology carefully

### Performance Optimization
1. **Efficient Arrangements**: Maximize talisman capacity usage
2. **Strategic Connections**: Use appropriate connection types
3. **Stability Management**: Maintain adequate stability levels
4. **Complexity Control**: Avoid unnecessary complexity

## Common Patterns

### Defensive Circle
```csharp
// Earth-based main circle with metal reinforcement
var defensive = new MagicCircle("Stone Wall", 8.0);
// Add Earth talismans for barrier effects
// Nest Metal circle for reinforcement
```

### Offensive Network
```csharp
// Fire circle connected to Wind amplifier
var fire = new MagicCircle("Inferno Core", 6.0);
var wind = new MagicCircle("Gale Force", 5.0);
var connection = fire.ConnectTo(wind, ConnectionType.Resonance);
// Wind amplifies and spreads fire effects
```

### Balanced Nexus
```csharp
// Central Void circle with elemental supports
var nexus = new MagicCircle("Elemental Nexus", 10.0);
// Add Void talisman for balance
// Connect to specialized element circles
// Creates versatile, adaptable spell system
```

## Troubleshooting

### Common Issues
1. **Low Stability**: Too many conflicting elements
   - Solution: Balance elements or reduce conflicts

2. **High Complexity**: Overly complicated arrangements
   - Solution: Simplify composition or break into components

3. **Poor Efficiency**: Elements not working well together
   - Solution: Redesign with compatible element relationships

4. **Connection Failures**: Circles too far apart or incompatible
   - Solution: Adjust positioning or choose different connection types
