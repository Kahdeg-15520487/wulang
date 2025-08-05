# Wu Xing Spellcraft

A magical programming language inspired by Chinese Wu Xing (Five Elements) philosophy, designed for creating visual programming experiences where code becomes spellcrafting.

## Project Structure

### 🏛️ **WuLangSpellcraft** (Core Library)
The main C# library containing all core functionality:
- Five Elements (Wu Xing) system with authentic Chinese relationships
- Talisman creation and elemental interactions
- Magic circle composition with sacred geometry
- 3D spell architecture and connections
- Complete JSON serialization system
- Extensible framework for visual programming applications

**Usage:**
```bash
dotnet add package WuLangSpellcraft
```

### 🎮 **WuLangSpellcraft.Demo** (Console Demo)
Interactive demonstration showcasing library capabilities:
- Complete walkthrough of Wu Xing concepts
- Real-time spell generation and effects
- Serialization system demonstration
- Educational console output with Chinese characters

**Run Demo:**
```bash
cd WuLangSpellcraft.Demo
dotnet run
```

## Philosophy & Design

### Wu Xing (Five Elements) 五行
Based on authentic Chinese elemental philosophy:

- **Water (水)** - Flow, adaptation, yielding strength
- **Wood (木)** - Growth, expansion, flexibility  
- **Fire (火)** - Transformation, energy, passion
- **Earth (土)** - Stability, grounding, nourishment
- **Metal (金)** - Structure, precision, clarity

### Generative Cycle (生)
Elements that strengthen each other:
Water → Wood → Fire → Earth → Metal → Water

### Destructive Cycle (克)
Elements that constrain each other:
Water ⚔ Fire ⚔ Metal ⚔ Wood ⚔ Earth ⚔ Water

## Key Features

### ✨ **Visual Programming Foundation**
- **Talisman System** - Individual spell components with elemental properties
- **Magic Circles** - Sacred geometry arrangements for complex operations
- **3D Architecture** - Layered spell construction with resonance connections
- **Balance Mechanics** - Harmony vs. conflict optimization puzzles

### 💾 **Complete Persistence**
- **JSON Serialization** - Human-readable spell configurations
- **Metadata Support** - Author, version, difficulty tracking
- **Spell Libraries** - Save, load, and share complex architectures
- **Version Control** - Git-friendly format for collaboration

### 🎯 **Educational Value**
- **Cultural Authenticity** - Genuine Chinese philosophical concepts
- **Programming Concepts** - Object composition, state management, serialization
- **Game Design** - Resource management, balance mechanics, emergent complexity

## Technical Architecture

### Core Classes
- `Element` - Base elemental forces with Wu Xing relationships
- `Talisman` - Individual spell components with positioning
- `MagicCircle` - Circular arrangements with geometric constraints
- `SpellEffect` - Generated effects for world engine integration
- `SpellConfiguration` - Complete spell definitions with metadata

### Serialization System
- `SpellSerializer` - JSON persistence with custom converters
- Full round-trip accuracy for complex spell architectures
- Async file operations for non-blocking I/O
- Extensible format for future enhancements

## Getting Started

### 1. Clone Repository
```bash
git clone <repository-url>
cd wulang
```

### 2. Build Solution
```bash
dotnet build
```

### 3. Run Demo
```bash
cd WuLangSpellcraft.Demo
dotnet run
```

### 4. Use Library
```csharp
using WuLangSpellcraft.Core;

var water = new Element(ElementType.Water, 1.5);
var fire = new Element(ElementType.Fire, 2.0);
var waterTalisman = new Talisman(water, "Azure Flow");

var circle = new MagicCircle("Test Circle");
circle.AddTalisman(waterTalisman);
var effect = circle.CalculateSpellEffect();
```

## Development Roadmap

### ✅ **Phase 1: Core Foundation** (Complete)
- Wu Xing elemental system
- Talisman and magic circle mechanics
- 3D architecture support
- JSON serialization system

### 🔄 **Phase 2: Visual Editor** (Next)
- Drag-and-drop talisman designer
- Real-time interaction feedback
- Sacred geometry visualization
- Spell effect preview

### 🔮 **Phase 3: World Engine Integration**
- Physics-based spell effects
- Weapon and object enhancement
- Environmental interactions
- Real-time rendering

### 🌟 **Phase 4: Advanced Features**
- Community spell sharing
- Procedural generation
- Performance optimization puzzles
- Educational curriculum

## Contributing

We welcome contributions that respect the philosophical foundations while expanding technical capabilities. Please ensure new features maintain the balance between authenticity and innovation.

### Development Guidelines
- Preserve Wu Xing authenticity
- Maintain clean, documented code
- Include comprehensive tests
- Follow established patterns

## License

MIT License - See LICENSE file for details.

## Acknowledgments

- Traditional Chinese Wu Xing philosophy
- Modern visual programming paradigms
- The open-source C# community

---

*"In the harmony of the five elements lies the power to transform code into magic."* ✨
