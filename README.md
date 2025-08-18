# WuLang Spellcraft

A comprehensive magical programming system inspired by Chinese Wu Xing (Five Elements) philosophy, designed for creating complex spell compositions through elemental interactions.

## 🌟 Overview

WuLang Spellcraft implements a complete magical system featuring:
- **12 Elemental Types** - 5 base Wu Xing elements + 7 derived combinations
- **Advanced Talisman System** - Individual spell components with stability mechanics
- **3D Magic Circle Compositions** - Nested, stacked, and networked arrangements
- **Artifact Creation System** - Permanent magical items with diverse functions
- **Comprehensive Spell Effects** - Physics-based world interactions

## 🏗️ Project Structure

### **WuLangSpellcraft** (Core Library)
The main C# library containing all core functionality:
- Elemental system with authentic Wu Xing relationships
- Talisman creation and stability mechanics
- Magic circle composition with 3D support
- Artifact creation and spell-imbued items
- Complete JSON serialization system

### **WuLangSpellcraft.Demo** (Console Demo)
Interactive demonstration showcasing library capabilities:
- Complete system walkthrough
- Composition system demos
- Stability and casting mechanics
- Interactive examples

## ✨ Key Features

### **Elemental System**
- **Base Elements**: Water, Fire, Earth, Metal, Wood (Wu Xing)
- **Derived Elements**: Lightning, Wind, Light, Dark, Forge, Chaos, Void
- **Authentic Relationships**: Traditional generative and destructive cycles
- **Complex Interactions**: Multi-element combinations and derived creations

### **Talisman System**
- **Stability Mechanics**: 6-tier stability system with casting consequences
- **Power Calculation**: Dynamic power based on elemental harmony
- **3D Positioning**: Full spatial awareness for circle arrangements
- **Casting Outcomes**: 7 different results from perfect success to destruction

### **Magic Circle Compositions**
- **Multiple Types**: Simple, Stacked, Nested, Network, Unified
- **Connection Systems**: Direct, Resonance, Flow, Trigger connections
- **Complexity Management**: Dynamic complexity scoring and casting time
- **3D Architecture**: Layer-based stacking with spatial constraints

### **Artifact System**
- **11 Elemental Artifacts**: Forge-based creations for each element
- **Spell-Imbued Artifacts**: Store complete spell patterns
- **Durability & Energy**: Usage tracking with repair mechanics
- **Rarity System**: Common to Legendary progression
## � Quick Start

### Installation
```bash
git clone <repository-url>
cd wulang
dotnet build
```

### Run Demo
```bash
cd WuLangSpellcraft.Demo
dotnet run
```

### Basic Usage
```csharp
using WuLangSpellcraft.Core;

// Create elements
var water = new Element(ElementType.Water, 1.5);
var fire = new Element(ElementType.Fire, 2.0);

// Create talisman
var waterTalisman = new Talisman(water, "Azure Flow");

// Create magic circle
var circle = new MagicCircle("Test Circle");
circle.AddTalisman(waterTalisman);

// Generate spell effect
var effect = circle.CalculateSpellEffect();
```

## 📚 Documentation

- **[Element System](docs/api/elements.md)** - Complete elemental reference
- **[Talisman Guide](docs/api/talismans.md)** - Talisman creation and mechanics
- **[Magic Circles](docs/api/magic-circles.md)** - Circle composition system
- **[Artifacts](docs/api/artifacts.md)** - Artifact creation and usage
- **[Composition Guide](docs/guides/composition-guide.md)** - Advanced composition techniques
- **[Stability System](docs/guides/stability-guide.md)** - Understanding casting mechanics

## 🔬 System Design

### Philosophical Foundation
Based on authentic Chinese Wu Xing philosophy with modern programming concepts:
- **Traditional Cycles**: Maintains authentic generative/destructive relationships
- **Modern Extensions**: Logical derived elements from base combinations
- **Balance Mechanics**: Harmony vs. conflict optimization

### Technical Architecture
- **Modular Design**: Extensible core systems
- **3D Spatial Support**: Full three-dimensional positioning
- **Serialization**: Complete persistence with JSON format
- **Performance**: Optimized for complex compositions

## 🤝 Contributing

We welcome contributions that respect the philosophical foundations while expanding technical capabilities.

### Development Guidelines
- Preserve Wu Xing authenticity
- Maintain comprehensive documentation
- Include thorough testing
- Follow established architectural patterns

## 📄 License

MIT License - See LICENSE file for details.

---

*"Through the harmony of elements, code becomes magic."* ✨
