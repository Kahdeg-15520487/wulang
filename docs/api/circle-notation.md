# Circle Notation Format (CNF)

## Overview

Circle Notation Format (CNF) is a compact, human-readable text format for representing WuLang Spellcraft magic circles and formations. It serves as an alternative to JSON serialization, offering:

- **Compact representat### Grammar Specification (EBNF)

```ebnf
Formation = Circle | ComplexFormation
ComplexFormation = "[" Circle "]" (Connection "[" Circle "]")*
Circle = "C" Radius Elements ["@" CenterElement] [Position]
Radius = Digit+
Elements = Element+
Element = ElementLetter [PowerLevel] [State] [":" TalismanId]
ElementLetter = "F" | "W" | "E" | "M" | "O" | "L" | "A" | "G" | "D" | "R" | "C" | "V"
PowerLevel = Digit+
State = "*" | "?" | "!" | "~"
TalismanId = (Letter | Digit | "_" | "-")+
CenterElement = Element
Connection = "-" | "=" | "~" | "≈" | "→" | "↔"
Position = "(" Number "," Number "," Number ")"
Number = ["-"] Digit+ ["." Digit+]
Digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
Letter = "A" | "B" | ... | "Z" | "a" | "b" | ... | "z"
```tly smaller than JSON
- **Human readability** - Easy to type, read, and understand
- **Pattern recognition** - Visual patterns emerge in complex formations
- **Version control friendly** - Text diffs show meaningful changes
- **Quick prototyping** - Rapid spell design and testing

## Basic Format Specification

### Element Symbols

All elements are represented by single uppercase letters:

#### Base Wu Xing Elements (五行)
- `F` - Fire (火) - Energy, transformation, passion
- `W` - Water (水) - Flow, adaptation, intuition  
- `E` - Earth (土) - Stability, grounding, endurance
- `M` - Metal (金) - Structure, precision, clarity
- `O` - Wood (木) - Growth, flexibility, creativity

#### Derived Elements
- `L` - Lightning (雷) - Fire + Metal - Sudden directed energy
- `A` - Wind (風) - Wood + Water - Adaptive movement
- `G` - Light (光) - Fire + Wood - Illumination, revelation
- `D` - Dark (闇) - Earth + Water - Concealment, mystery
- `R` - Forge (鍛) - Metal + Wood - Creation, crafting
- `C` - Chaos (混沌) - All 5 base elements - Unpredictable potential
- `V` - Void (虛無) - Absence of elements - Balance, neutralization

### Circle Syntax

```
C<radius> <elements>[@<center>]
```

**Components:**
- `C` - Circle identifier (required)
- `<radius>` - Integer radius value (required)
- `<elements>` - Sequence of element letters (required)
- `@<center>` - Optional center element/talisman

**Examples:**
```
C3 FWE          # Three talismans: Fire, Water, Earth (auto-generated IDs)
C5 FLGEMWLD     # Eight talismans around circumference (auto-generated IDs)
C1 F            # Single Fire talisman (auto-generated ID)
C4 FWEO@M       # Four outer talismans with Metal at center (auto-generated IDs)
C3 F:core W:flow E:anchor    # Named talisman references
C4 F:1 W:2 E:3 O:4@M:center  # Mixed numbered and named references
```

### Element Power Levels (Optional)

Power levels can be specified after each element using numbers:

```
C3 F2W1E3       # Fire power 2, Water power 1, Earth power 3
C5 FLG2E3MWL1D  # Mixed power levels (default is 1)
C3 F2:core W1:flow E3:anchor  # Power levels with named IDs
```

### Element States (Optional)

Special states can be indicated with symbols after the element:

```
C3 F*WE         # * = active/charged element
C3 F?WE         # ? = unstable element  
C3 F!WE         # ! = damaged element
C3 F~WE         # ~ = resonating element
```

### Talisman Identification (Optional)

Talismans can be given string IDs for reference using the `:` notation:

```
C3 F:fire_core W:water_flow E:earth_anchor    # Named talisman references
C5 F:1 L:2 G:3 E:4 M:5                        # Simple numbered references
C3 FWE                                         # Auto-generated IDs (anonymous)
```

**ID Generation Rules:**
- If no ID specified: auto-generate (e.g., "t1", "t2", "t3", ...)
- If ID specified: use the provided string exactly
- IDs must be unique within the same formation
- IDs can contain letters, numbers, underscores, and hyphens

## Connection Notation

### Connection Types

Connections between circles use specific symbols:

- `-` - Basic connection (simple energy flow)
- `=` - Strong connection (amplified energy flow)
- `~` - Harmonic connection (resonant frequency)
- `≈` - Unstable connection (fluctuating energy)
- `→` - Directional flow (one-way)
- `↔` - Bidirectional flow (two-way)

### Formation Syntax

```
[<circle>] <connection> [<circle>] [<connection> [<circle>]]...
```

**Examples:**
```
[C3 FWE] - [C4 MLGO]                    # Two circles, basic connection
[C5 FLGEMWLD] = [C2 FW]                 # Strong connection
[C3 FWE] ~ [C3 MLD] ~ [C3 OGC]          # Chain of harmonic connections
[C1 F] → [C3 WEO] ← [C1 M]              # Directional flows
```

## Advanced Formations

### Multi-Layer Circles

Use `|` to separate layers from inner to outer:

```
C1 F|C3 WEO|C5 MLGDA    # Three concentric layers
C2 FW@E|C4 MLGO         # Two layers with center element
```

### Complex Formations

#### Triangle Formation
```
[C3 FWE] - [C3 MLD] - [C3 OGC] - [C3 FWE]
```

#### Star Formation with Hub
```
[C2 F] = [C1 H] = [C2 W] = [C1 H] = [C2 E] = [C1 H] = [C2 F]
```
Where `H` represents a hub circle.

#### Grid Formation
```
[C1 F] - [C1 W] - [C1 E]
   |       |       |
[C1 M] - [C1 O] - [C1 L]  
   |       |       |
[C1 G] - [C1 D] - [C1 C]
```

### Positional Information (Optional)

3D coordinates can be specified in parentheses:

```
C3 FWE(0,0,0)           # Circle at origin
C3 FWE(5,0,2)           # Circle at position (5,0,2)
C3 FWE(10,-3,1)         # Circle at position (10,-3,1)
```

## Pattern Libraries

### Common Formation Patterns

#### Elemental Ward (Pentagon)
```
[C3 F] - [C3 W] - [C3 E] - [C3 M] - [C3 O] - [C3 F]
```

#### Power Amplification Array
```
[C1 F3] = [C5 FWEMOFWEMO@C] = [C1 W3]
              |
              =
              |
         [C7 FWEMOLFWEMOLG]
```

#### Chaos Containment Grid
```
[C2 V] ~ [C4 FLWE] ~ [C2 V]
  |         |         |
  ~    [C6 CEMOGD]    ~
  |         |         |
[C2 V] ~ [C4 MOGL] ~ [C2 V]
```

#### Stability Matrix
```
[C3 EEE] = [C3 WWW] = [C3 EEE]
    |         |         |
    =         =         =
    |         |         |
[C3 WWW] = [C3 FFF] = [C3 WWW]
    |         |         |  
    =         =         =
    |         |         |
[C3 EEE] = [C3 WWW] = [C3 EEE]
```

## Shorthand Notations

### Geometric Patterns

```
P5[C3 FWEMO]                    # Pentagon: 5 circles in pentagon shape
T3[C2 FW, C2 ME, C2 OL]         # Triangle: 3 specified circles
G3x3[C1 F, C1 W, ..., C1 C]     # Grid: 3x3 grid of circles
R[C3 FWE]                       # Ring: circular arrangement
S5[C1 F, C2 W, C3 E, C2 M, C1 O] # Star: 5-pointed star formation
```

### Repetition Patterns

```
C5 F*8              # Eight Fire elements around radius 5
C3 (FW)*3           # Three pairs of Fire-Water around radius 3
C7 F3W2E*4          # Four repetitions of Fire3-Water2-Earth sequence
```

## Comparison with JSON Format

### Size Comparison

**CNF:**
```
[C3 FWE] - [C4 MLGO] = [C2 FW]
```

**JSON Equivalent:**
```json
{
  "circles": [
    {
      "radius": 3,
      "elements": ["Fire", "Water", "Earth"],
      "connections": [{"target": 1, "type": "basic"}]
    },
    {
      "radius": 4, 
      "elements": ["Metal", "Light", "Light", "Wood"],
      "connections": [{"target": 0, "type": "basic"}, {"target": 2, "type": "strong"}]
    },
    {
      "radius": 2,
      "elements": ["Fire", "Water"],
      "connections": [{"target": 1, "type": "strong"}]
    }
  ]
}
```

**Size Reduction:** ~85% smaller

### Readability Comparison

**CNF Benefits:**
- Pattern recognition at a glance
- Easy to type and modify
- Version control shows meaningful diffs
- Natural grouping of related elements

**JSON Benefits:**
- Structured metadata support
- Tool ecosystem (validators, editors)
- Precise type information
- Extensible schema

## Grammar Specification (EBNF)

```ebnf
Formation = Circle | ComplexFormation
ComplexFormation = "[" Circle "]" (Connection "[" Circle "]")*
Circle = "C" Radius Elements ["@" CenterElement] [Position]
Radius = Digit+
Elements = Element+
Element = ElementLetter [PowerLevel] [State]
ElementLetter = "F" | "W" | "E" | "M" | "O" | "L" | "A" | "G" | "D" | "R" | "C" | "V"
PowerLevel = Digit+
State = "*" | "?" | "!" | "~"
CenterElement = Element
Connection = "-" | "=" | "~" | "≈" | "→" | "↔"
Position = "(" Number "," Number "," Number ")"
Number = ["-"] Digit+ ["." Digit+]
Digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
```

## Use Cases

### 1. Documentation
```markdown
The Elemental Ward formation uses a pentagon pattern:
`[C3 F] - [C3 W] - [C3 E] - [C3 M] - [C3 O] - [C3 F]`
```

### 2. Unit Testing
```csharp
[Test]
public void TestFireWaterInteraction()
{
    var formation = CircleNotation.Parse("[C1 F] - [C1 W]");
    Assert.That(formation.GetInteractionEffect(), Is.EqualTo(ElementalEffect.Steam));
}
```

### 3. Configuration Files
```
# spell_library.cnf
ward_basic=P5[C3 FWEMO]
amplifier_simple=[C1 F3] = [C5 WEMO@C] = [C1 W3]
chaos_seal=[C2 V] ~ [C6 CEMOGD] ~ [C2 V]
```

### 4. Interactive Design
```
> create [C3 FWE] - [C4 MLGO]
Created formation with 2 circles, 1 connection
> connect last = [C2 FW]  
Added strong connection to new circle
> show
[C3 FWE] - [C4 MLGO] = [C2 FW]
```

### 5. Version Control
```diff
- stability_ward=[C3 EEE] = [C3 WWW] = [C3 EEE]
+ stability_ward=[C3 E2E2E2] = [C3 W3W3W3] = [C3 E2E2E2]
```

## Implementation Notes

### Parser Considerations
- Left-to-right parsing for connections
- Precedence rules for complex expressions
- Error handling for malformed notation
- Validation against element rules

### Serialization Strategy
- Primary format: JSON for metadata, CNF for structure
- Hybrid approach: JSON wrapper with CNF payload
- Pure CNF with metadata comments
- Context-dependent format selection

### Performance Characteristics
- Parse time: O(n) where n is notation length
- Memory usage: Significantly lower than JSON
- Human processing: Much faster pattern recognition
- Tool support: Requires custom parsers

## Future Extensions

### Temporal Notation
```
C3 FWE[t0] → C3 MLD[t1] → C3 OGC[t2]    # Time-based transitions
```

### Conditional Logic
```
C3 FWE ? [C2 ML] : [C2 OG]              # Conditional formations
```

### Macro Definitions
```
@WARD = P5[C3 FWEMO]
@SPELL = [@WARD] = [C1 C] = [@WARD]
```

### Probability Expressions
```
C3 F80%W15%E5%                          # Probabilistic element selection
```
