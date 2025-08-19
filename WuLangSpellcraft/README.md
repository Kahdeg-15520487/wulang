# WuLangSpellcraft Library

A C# library for creating magical programming experiences inspired by the Chinese Wu Xing (Five Elements) philosophy.

## Overview

WuLangSpellcraft provides a comprehensive framework for building visual programming languages based on traditional Chinese elemental philosophy. The library enables developers to create applications where code becomes spellcrafting through elemental harmony and sacred geometry.

## Core Concepts

### Five Elements (Wu Xing 五行)
- **Water (水)** - Flow control, streams, fluid dynamics
- **Fire (火)** - Actions, projectiles, transformations  
- **Earth (土)** - Barriers, stability, storage
- **Metal (金)** - Logic, precision, enhancement
- **Wood (木)** - Growth, iteration, organic expansion

### Elemental Relationships
- **Generative Cycle (生)** - Elements that strengthen each other
- **Destructive Cycle (克)** - Elements that conflict and weaken each other

## Key Features

### 🔮 **Talisman System**
Individual spell components with elemental properties:
```csharp
var fireTalisman = new Talisman(new Element(ElementType.Fire, 2.0), "Flame Core");
fireTalisman.AddSecondaryElement(new Element(ElementType.Wood, 0.8));
```

### ⭕ **Magic Circles**
Sacred geometry arrangements for complex spells:
```csharp
var circle = new MagicCircle("Elemental Harmony", 8.0);
circle.AddTalisman(fireTalisman);
var spellEffect = circle.CalculateSpellEffect();
```

### 🌐 **3D Architecture**
Layer-based spell construction with resonance connections:
```csharp
var upperCircle = new MagicCircle("Amplification Layer", 6.0);
upperCircle.Layer = 5.0;
var connection = circle.ConnectTo(upperCircle, ConnectionType.Resonance);
```

### 💾 **Serialization**
Complete spell persistence with JSON format:
```csharp
var spellConfig = SpellConfiguration.FromCircles("My Spell", circles);
await SpellSerializer.SaveSpellToFileAsync(spellConfig, "spell.json");
var loaded = await SpellSerializer.LoadSpellFromFileAsync("spell.json");
```

## Installation

```bash
dotnet add package WuLangSpellcraft
```

## Quick Start

```csharp
using WuLangSpellcraft.Core;

// Create elements
var water = new Element(ElementType.Water, 1.5);
var fire = new Element(ElementType.Fire, 2.0);

// Check elemental relationship
var relation = water.GetRelationTo(fire); // Returns Destroys

// Create talismans
var waterTalisman = new Talisman(water, "Azure Flow");
var fireTalisman = new Talisman(fire, "Crimson Burst");

// Build magic circle
var circle = new MagicCircle("Test Circle");
circle.AddTalisman(waterTalisman);
circle.AddTalisman(fireTalisman);

// Generate spell effect
var effect = circle.CalculateSpellEffect();
Console.WriteLine($"Generated: {effect.Type} with power {effect.Power}");
```

## Architecture

### Core Namespace: `WuLangSpellcraft.Core`
- `Element` - Base elemental forces with Wu Xing relationships
- `Talisman` - Individual spell components with elemental properties
- `MagicCircle` - Circular arrangements with sacred geometry
- `SpellEffect` - Generated effects for world engine integration

### Serialization: `WuLangSpellcraft.Serialization`
- `SpellSerializer` - JSON serialization for persistence
- `SpellConfiguration` - Complete spell definitions with metadata
- Custom converters for all core types

## Advanced Usage

### Complex Elemental Interactions
```csharp
// Create multi-element talisman
var complexTalisman = new Talisman(new Element(ElementType.Fire, 2.0));
complexTalisman.AddSecondaryElement(new Element(ElementType.Wood, 0.8));
complexTalisman.AddSecondaryElement(new Element(ElementType.Earth, 1.2));

// Check stability
Console.WriteLine($"Stability: {complexTalisman.Stability}");
```

### 3D Spell Architecture
```csharp
// Create layered spell
var baseCircle = new MagicCircle("Foundation", 10.0);
var midCircle = new MagicCircle("Amplification", 8.0) { Layer = 3.0 };
var topCircle = new MagicCircle("Focus", 6.0) { Layer = 6.0 };

// Connect layers
baseCircle.ConnectTo(midCircle, ConnectionType.Flow);
midCircle.ConnectTo(topCircle, ConnectionType.Resonance);
```

### Spell Libraries
```csharp
// Create spell collection
var spellConfig = new SpellConfiguration("Fire Storm Spell")
{
    Author = "Master Elementalist",
    Description = "Multi-layer fire projectile spell",
    Metadata = { ["difficulty"] = "Advanced", ["element"] = "Fire" }
};

spellConfig.AddCircle(baseCircle);
spellConfig.AddCircle(midCircle);
```

## Philosophy

WuLangSpellcraft is built on authentic Chinese Wu Xing philosophy:

- **Balance** - Harmonious element combinations create stable, powerful spells
- **Conflict** - Destructive relationships can be harnessed for unique effects
- **Flow** - Energy moves naturally through generative cycles
- **Sacred Geometry** - Circular arrangements follow traditional patterns
- **Natural Laws** - Element relationships mirror natural phenomena

## Contributing

We welcome contributions that respect the philosophical foundations while expanding technical capabilities. Please ensure new features maintain the balance between authenticity and innovation.

## License

MIT License - See LICENSE file for details.

## Acknowledgments

Inspired by traditional Chinese Wu Xing philosophy and modern visual programming paradigms.
