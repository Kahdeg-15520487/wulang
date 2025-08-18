# Artifact System Reference

## Overview

Artifacts in WuLang Spellcraft are permanent magical items that enhance circles, store spells, or provide special capabilities. They are created through the Forge element and come in multiple types with varying rarities and functions.

## Artifact Types

### Elemental Artifacts
Created by combining the Forge element with base or derived elements:

#### Base Element Artifacts
- **Chalice of Flow** (Forge + Water)
  - Increases water element efficiency by 50%
  - Provides 25% adaptation bonus
  - Enhances spell flexibility

- **Crucible of Power** (Forge + Fire)
  - Amplifies all elemental energies by 50%
  - 10% instability risk
  - Powerful but dangerous

- **Foundation Stone** (Forge + Earth)
  - Increases circle stability by 50%
  - Adds 2 extra talisman capacity
  - Provides structural enhancement

- **Conductor's Ring** (Forge + Metal)
  - Doubles connection range
  - 25% energy transfer efficiency bonus
  - Enhances circle networking

- **Living Catalyst** (Forge + Wood)
  - Enables artifact evolution (5% growth rate)
  - Self-repair capabilities
  - Can improve over time

#### Derived Element Artifacts
- **Storm Core** (Forge + Lightning)
  - Channels electrical energy
  - Provides burst power capabilities

- **Ethereal Anchor** (Forge + Wind)
  - Enables atmospheric manipulation
  - Provides flow control

- **Beacon of Truth** (Forge + Light)
  - Illumination and revelation effects
  - Dispels darkness and illusions

- **Shadow Vault** (Forge + Dark)
  - Concealment and hidden storage
  - Protects against detection

- **Wildcard Relic** (Forge + Chaos)
  - Unpredictable effects
  - High risk, high reward

- **Null Anchor** (Forge + Void)
  - Neutralization capabilities
  - Balancing and stabilization

### Spell-Imbued Artifacts
Store complete spell patterns for later casting:

- **Spell Wands**: Primary spell storage artifacts
- **Spell Orbs**: Alternative storage form
- **Spell Scrolls**: Temporary, single-use storage

### Composite Artifacts
- **Artifact Sets**: Multiple artifacts working together
- **Evolved Artifacts**: Artifacts that have grown/changed over time

## Artifact Properties

### Core Properties
```csharp
public class Artifact
{
    public string Name { get; set; }
    public ArtifactType Type { get; }
    public ElementType PrimaryElement { get; }
    public ArtifactRarity Rarity { get; set; }
    
    public double PowerLevel { get; set; }
    public double Stability { get; set; }
    public double Durability { get; set; }
    
    public int MaxUses { get; set; }
    public int CurrentUses { get; set; }
    
    public double EnergyCapacity { get; set; }
    public double CurrentEnergy { get; set; }
    public double EnergyEfficiency { get; set; }
}
```

### Rarity System
- **Common**: Basic elemental artifacts
- **Uncommon**: Enhanced or derived artifacts
- **Rare**: Complex spell-imbued artifacts
- **Epic**: Multi-spell or evolved artifacts
- **Legendary**: Unique or extremely powerful artifacts

## Artifact Creation

### Elemental Artifact Creation
```csharp
// Create elemental artifact in a magic circle
var circle = new MagicCircle("Forge Circle");
// Add forge and target element talismans

var artifact = circle.CreateElementalArtifact(ElementType.Forge, ElementType.Fire);
// Creates a Crucible of Power artifact
```

### Spell-Imbued Artifact Creation
```csharp
// Create from existing magic circle
var spellCircle = new MagicCircle("Fireball Pattern");
// Configure circle with desired spell

var spellArtifact = spellCircle.CreateSpellArtifact("Fireball Wand");
// Circle is copied into the artifact for later casting
```

### Advanced Creation
```csharp
// Create with base artifact enhancement
var baseArtifact = new ElementalArtifact(/*...*/);
var enhancedSpell = circle.CreateSpellArtifact("Enhanced Wand", baseArtifact);
// Base artifact properties enhance the spell artifact
```

## Artifact Usage

### Basic Usage
```csharp
// Check if artifact can be used
if (artifact.CanUse())
{
    bool success = artifact.TryUse(energyRequired: 25.0);
    if (success)
    {
        // Artifact effect applied
        Console.WriteLine("Artifact activated successfully!");
    }
}

// Check artifact condition
double condition = artifact.GetCondition(); // 0.0 to 1.0
Console.WriteLine($"Artifact condition: {condition:P0}");
```

### Spell Artifact Casting
```csharp
var spellArtifact = /* get spell artifact */;

// Cast stored spell
var effect = spellArtifact.CastStoredSpell(energyInput: 50.0);
if (effect != null)
{
    Console.WriteLine($"Spell cast: {effect.Type} with power {effect.Power:F1}");
}
else
{
    Console.WriteLine("Insufficient energy or artifact depleted");
}
```

### Maintenance
```csharp
// Recharge artifact energy
artifact.Recharge(energy: 30.0);

// Repair artifact
artifact.Repair(durabilityRestore: 25.0, usesRestore: 3);

// Special case: Living Catalyst self-repair
// Automatically repairs during use if conditions are met
```

## Circle Integration

### Attaching Artifacts
```csharp
var circle = new MagicCircle("Enhanced Circle");
var foundationStone = new ElementalArtifact(ArtifactType.FoundationStone, /*...*/);

// Attach artifact to circle
bool attached = circle.AttachArtifact(foundationStone);

// Artifacts modify circle properties:
// - Foundation Stone: +50% stability, +2 talisman capacity
// - Conductor's Ring: 2x connection range, +25% transfer efficiency
// - Crucible of Power: +50% power amplification, +10% instability
```

### Artifact Effects on Circles
Different artifacts provide different benefits:

#### Power Enhancement
- **Crucible of Power**: 50% power amplification
- **Living Catalyst**: Gradual power growth over time

#### Stability Enhancement
- **Foundation Stone**: 50% stability bonus
- **Null Anchor**: Neutralization and balance

#### Capacity Enhancement
- **Foundation Stone**: +2 talisman capacity
- **Conductor's Ring**: Enhanced connection capabilities

#### Efficiency Enhancement
- **Chalice of Flow**: 50% water efficiency, 25% adaptation
- **Conductor's Ring**: 25% energy transfer efficiency

## Advanced Features

### Artifact Evolution
```csharp
// Living Catalyst can evolve over time
var catalyst = new ElementalArtifact(ArtifactType.LivingCatalyst, /*...*/);

// During use, if conditions are right:
// - Self-repair occurs
// - Power level gradually increases
// - Potentially develops new capabilities
```

### Artifact Sets
```csharp
// Future feature: Multiple artifacts working together
// - Synergistic effects between compatible artifacts
// - Set bonuses for complete collections
// - Enhanced power when artifacts resonate
```

### Artifact Properties
```csharp
// Access custom properties
if (artifact.Properties.ContainsKey("AdaptationBonus"))
{
    var bonus = (double)artifact.Properties["AdaptationBonus"];
    // Use bonus in calculations
}

// Common properties:
// - PowerAmplification: Multiplier for power effects
// - StabilityBonus: Added stability value
// - CapacityIncrease: Additional talisman slots
// - ConnectionRange: Enhanced connection distance
// - EnergyTransferEfficiency: Improved energy flow
```

## Artifact Management

### Condition Monitoring
```csharp
// Monitor artifact health
double condition = artifact.GetCondition();

if (condition < 0.3)
{
    Console.WriteLine("Artifact needs repair!");
    artifact.Repair();
}

if (artifact.CurrentUses < 2)
{
    Console.WriteLine("Artifact nearly depleted!");
}
```

### Energy Management
```csharp
// Monitor energy levels
double energyPercent = artifact.CurrentEnergy / artifact.EnergyCapacity;

if (energyPercent < 0.2)
{
    // Recharge from external source
    artifact.Recharge(50.0);
}

// Calculate energy efficiency
double efficiency = artifact.EnergyEfficiency;
double actualCost = baseCost / efficiency;
```

### Usage Tracking
```csharp
// Track usage patterns
int usesRemaining = artifact.CurrentUses;
int totalUses = artifact.MaxUses;
double usagePercent = (double)usesRemaining / totalUses;

// Plan artifact replacement or repair
if (usagePercent < 0.1)
{
    // Consider creating replacement artifact
}
```

## Best Practices

### Artifact Selection
1. **Match Purpose**: Choose artifacts that complement your circle's function
2. **Balance Power vs Stability**: Powerful artifacts often increase risk
3. **Consider Efficiency**: Some artifacts provide better long-term value
4. **Plan for Maintenance**: Factor in repair and recharge needs

### Usage Strategy
1. **Monitor Condition**: Keep artifacts in good repair
2. **Energy Management**: Don't let artifacts run empty
3. **Strategic Deployment**: Use powerful artifacts when they matter most
4. **Backup Plans**: Have redundant artifacts for critical functions

### Creation Strategy
1. **Start Simple**: Begin with basic elemental artifacts
2. **Experiment Safely**: Test new combinations in safe environments
3. **Document Results**: Track successful artifact configurations
4. **Iterate and Improve**: Refine artifacts based on experience

## Common Combinations

### Defensive Setup
- **Foundation Stone**: Enhanced stability and capacity
- **Null Anchor**: Neutralization capabilities
- **Shadow Vault**: Concealment and protection

### Offensive Setup
- **Crucible of Power**: Maximum power amplification
- **Storm Core**: Electrical burst capabilities
- **Beacon of Truth**: Piercing illumination

### Support Setup
- **Conductor's Ring**: Enhanced networking
- **Chalice of Flow**: Adaptive capabilities
- **Living Catalyst**: Self-improving support

### Experimental Setup
- **Wildcard Relic**: Unpredictable effects
- **Ethereal Anchor**: Atmospheric manipulation
- **Evolved Artifacts**: Unique capabilities

## Troubleshooting

### Common Issues
1. **Artifact Won't Activate**
   - Check energy levels
   - Verify durability > 0
   - Confirm remaining uses > 0

2. **Poor Performance**
   - Repair if condition is low
   - Recharge if energy is depleted
   - Check compatibility with circle

3. **Rapid Degradation**
   - Review usage patterns
   - Consider artifact type limitations
   - May need different artifact for heavy use

4. **Unexpected Effects**
   - Some artifacts (Chaos-based) are inherently unpredictable
   - Check for artifact interactions
   - Review circle compatibility
