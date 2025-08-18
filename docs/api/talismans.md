# Talisman System Reference

## Overview

Talismans are individual spell components that contain elemental power and serve as the building blocks for magic circles. Each talisman has a primary element and can contain up to 3 secondary elements, creating complex interactions that affect power, stability, and casting outcomes.

## Core Talisman Properties

### Basic Properties
- **Primary Element**: The main elemental force (required)
- **Secondary Elements**: Up to 3 additional elements (optional)
- **Power Level**: Calculated from elemental interactions
- **Stability**: Ranging from 0.0 to 1.0 based on elemental harmony
- **Position**: 3D coordinates (X, Y, Z) for circle placement
- **Pattern**: Visual representation based on primary element

### Stability System

The stability system is the core of talisman mechanics, determining casting success and risks:

#### Stability Levels
1. **Perfect Stability (0.9-1.0)** ✨
   - 20% chance for Enhanced Success
   - 80% normal Success
   - 1.1-1.2x power multiplier
   - Possible energy cost reduction

2. **High Stability (0.7-0.89)** ✅
   - 95% Success rate
   - 5% Fizzle chance
   - 1.05x power bonus
   - Reliable performance

3. **Moderate Stability (0.5-0.69)** ⚠️
   - 75% Success rate
   - 20% Fizzle chance
   - 5% Wild magic
   - ±20% power variation

4. **Low Stability (0.3-0.49)** ⚠️⚠️
   - 50% Success rate
   - 25% Fizzle chance
   - 15% Backfire
   - 5% Element inversion
   - 5% Stability damage

5. **Critical Instability (0.1-0.29)** ⚠️⚠️⚠️
   - 25% Success rate
   - 25% Fizzle chance
   - 20% Backfire
   - 15% Catastrophic failure
   - 15% Talisman destruction

6. **Complete Instability (0.0-0.09)** 💀
   - 0% Success rate
   - 100% Talisman destruction
   - Explosive magical release
   - Severe consequences

## Talisman Creation

### Basic Creation
```csharp
// Create with primary element only
var waterElement = new Element(ElementType.Water, 1.5);
var waterTalisman = new Talisman(waterElement, "Azure Flow");

// Create with auto-generated name
var fireTalisman = new Talisman(new Element(ElementType.Fire));
```

### Adding Secondary Elements
```csharp
var talisman = new Talisman(new Element(ElementType.Fire));

// Add compatible secondary elements
bool success1 = talisman.AddSecondaryElement(new Element(ElementType.Wood));
bool success2 = talisman.AddSecondaryElement(new Element(ElementType.Metal));
bool success3 = talisman.AddSecondaryElement(new Element(ElementType.Earth));

// This would fail (limit of 3 secondary elements)
bool failure = talisman.AddSecondaryElement(new Element(ElementType.Water));
```

## Stability Calculation

Stability is calculated based on elemental relationships:

### Pure Element Talismans
- **Single Element**: Always 1.0 stability (perfect)
- **No Conflicts**: Pure elements are inherently stable

### Multi-Element Talismans
Stability calculation considers:
1. **Primary vs Secondary**: Each secondary element's relationship to primary
2. **Secondary vs Secondary**: Relationships between all secondary elements
3. **Harmony Ratio**: Generative relationships increase stability
4. **Conflict Penalty**: Destructive relationships decrease stability

```csharp
// Examples of stability outcomes:
// Fire + Wood = High stability (Wood generates Fire)
// Fire + Water = Low stability (Water destroys Fire)
// Fire + Wood + Metal = Moderate (Wood helps, Metal conflicts)
```

## Casting Mechanics

### Casting Attempt
```csharp
var result = talisman.AttemptCasting(spellEnergyCost: 50.0);

// Check outcome
switch (result.Outcome)
{
    case CastingOutcome.Success:
        Console.WriteLine($"Success! Power: {result.PowerMultiplier}x");
        break;
    case CastingOutcome.Backfire:
        Console.WriteLine($"Backfire! Energy rebounds: {result.PowerMultiplier}x");
        break;
    case CastingOutcome.TalismanDestruction:
        Console.WriteLine("Talisman destroyed!");
        // Talisman is now unusable
        break;
}
```

### Casting Results
The `CastingResult` contains:
- **Outcome**: Type of result (Success, Fizzle, Backfire, etc.)
- **PowerMultiplier**: Effect on spell power (0.0 to 2.0+)
- **EnergyConsumed**: Actual energy cost
- **StabilityDamage**: Permanent stability loss
- **TalismanDestroyed**: Whether talisman is destroyed
- **SecondaryEffects**: Additional consequences

## Talisman Interactions

### Resonance Calculation
Talismans interact with each other based on:
- **Elemental Relationships**: Generative/destructive cycles
- **Distance**: Closer talismans have stronger interactions
- **Power Levels**: Higher power creates stronger resonance

```csharp
var interaction = talisman1.GetInteractionWith(talisman2);

Console.WriteLine($"Resonance: {interaction.Resonance:F2}");
Console.WriteLine($"Energy Flow: {interaction.EnergyFlow:F1}");
Console.WriteLine($"Stable: {interaction.IsStable}");
```

### Energy Flow
Energy flow between talismans considers:
- **Distance Factor**: Decreases with spatial separation
- **Elemental Compatibility**: Enhanced by generative relationships
- **Stability**: More stable talismans transfer energy better

## Talisman Patterns

Each talisman has a visual pattern based on its primary element:

### Base Element Patterns
- **Water**: Circle with wave symbols (~, ≋, ◦)
- **Fire**: Triangle with flame symbols (▲, ◊, ※)
- **Earth**: Square with solid symbols (■, ⬜, ◾)
- **Metal**: Diamond with metallic symbols (◇, ⬟, ⟐)
- **Wood**: Hexagon with organic symbols (※, ⟡, ⬢)

### Derived Element Patterns
- **Lightning**: Zigzag with electric symbols (⚡, ⟲, ※)
- **Wind**: Spiral with flow symbols (○, ◐, ◑)
- **Light**: Star with radiant symbols (☀, ✦, ◊)
- **Dark**: Void with shadow symbols (●, ◉, ■)
- **Forge**: Anvil with craft symbols (⚒, ◈, ⬟)

## Position and Placement

### 3D Positioning
```csharp
// Set talisman position
talisman.X = 5.0;  // East-West
talisman.Y = 3.0;  // North-South
talisman.Z = 1.0;  // Layer/Height

// Position affects:
// - Energy flow calculations
// - Circle arrangement
// - Resonance strength
```

### Circle Integration
When adding to magic circles:
- **Automatic Positioning**: Circles can auto-arrange talismans
- **Manual Placement**: Specify exact angles and positions
- **Capacity Limits**: Based on circle radius and talisman spacing

## Advanced Features

### Stability Damage
Permanent stability loss from failed castings:
```csharp
// Stability damage accumulates over time
var initialStability = talisman.Stability; // e.g., 0.8
// After several failed castings with damage
var currentStability = talisman.Stability; // e.g., 0.6

// Damage is permanent and affects base stability calculation
```

### Talisman Destruction
When a talisman is destroyed:
- Cannot be repaired or restored
- Releases explosive magical energy
- May damage nearby talismans or circles
- Lost permanently from the system

## Best Practices

### Stability Optimization
1. **Start Simple**: Use single elements for reliable casting
2. **Test Combinations**: Add secondary elements gradually
3. **Monitor Stability**: Keep stability above 0.5 for regular use
4. **Avoid Low Stability**: Never use talismans below 0.3 in important spells

### Power Optimization
1. **Leverage Synergy**: Use generative relationships between elements
2. **Balance Conflicts**: Minimize destructive relationships
3. **Position Matters**: Place compatible talismans near each other
4. **Secondary Elements**: Use strategically for power boosts

### Risk Management
1. **Backup Talismans**: Keep stable copies of important talismans
2. **Stability Monitoring**: Check stability regularly
3. **Graduated Testing**: Test complex talismans in safe environments
4. **Emergency Protocols**: Have procedures for talisman failures

## Common Patterns

### Reliable Combinations
- **Fire + Wood**: Strong generative synergy
- **Water + Metal**: Good flow and structure
- **Earth + Metal**: Excellent stability
- **Light + Wood**: Life-enhancing combination

### Risky Combinations
- **Water + Fire**: Direct opposition
- **Metal + Fire**: Metal melts under fire
- **Wood + Metal**: Metal cuts wood
- **Any + Chaos**: Unpredictable outcomes

### Specialized Talismans
- **Pure Elements**: Maximum stability for reliable effects
- **Forge Talismans**: Essential for artifact creation
- **Chaos Talismans**: For unpredictable wild magic
- **Void Talismans**: For neutralization and balance
