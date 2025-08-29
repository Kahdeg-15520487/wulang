# Circle Notation Format (CNF)

## Overview

Circle Notation Format (CNF) is a compact, human-readable text format for representing WuLang Spellcraft magic circles and formations. It serves as an alternative to JSON serialization, offering:

- **Compact representat### Grammar Specification (EBNF)

```ebnf
Formation = Circle | ComplexFormation
ComplexFormation = CircleDefinition+ ConnectionDefinition*
CircleDefinition = "[" CircleId ":" Circle "]" | Circle
Circle = "C" Radius Elements ["@" CenterElement] [Position]
CircleId = (Letter | Digit | "_" | "-")+
ConnectionDefinition = CircleId ConnectionType CircleId
Radius = Number
Elements = Element+
Element = ElementLetter [PowerLevel] [State] [":" TalismanId]
ElementLetter = "F" | "W" | "E" | "M" | "O" | "L" | "N" | "I" | "D" | "G" | "C" | "V"
PowerLevel = Number
State = "?" | "!" | "~"
TalismanId = (Letter | Digit | "_" | "-")+
CenterElement = Element
ConnectionType = "-" | "=" | "~" | "~=" | "->" | "<->"
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
- `N` - Wind (風) - Wood + Water - Adaptive movement (wiNd)
- `I` - Light (光) - Fire + Wood - Illumination, revelation (lIght)
- `D` - Dark (闇) - Earth + Water - Concealment, mystery
- `G` - Forge (鍛) - Metal + Wood - Creation, crafting (forGe)
- `C` - Chaos (混沌) - All 5 base elements - Unpredictable potential
- `V` - Void (虛無) - Absence of elements - Balance, neutralization

### Circle Syntax

```
C<radius> <elements>[@<center>]
```

**Components:**
- `C` - Circle identifier (required, must be uppercase)
- `<radius>` - Integer radius value (required)
- `<elements>` - Sequence of element letters (required)
- `@<center>` - Optional center element/talisman

**Examples:**
```
C3 FWE          # Three talismans: Fire, Water, Earth
C5 FLEMWID     # Seven talismans around circumference
C1 F            # Single Fire talisman
C4 FWEO@M       # Four outer talismans with Metal at center
C3 F:core W:flow E:anchor    # Named talisman references
C4 F:1 W:2 E:3 O:4@M:center  # Mixed numbered and named references
```

### Element Power Levels (Optional)

Power levels can be specified after each element using numbers:

```
C3 F2W1E3       # Fire power 2, Water power 1, Earth power 3
C5 FLIG2E3MNI  # Mixed power levels (default is 1)
C3 F2:core W1:flow E3:anchor  # Power levels with named IDs
```

### Element States (Optional)

Special states can be indicated with symbols after the element:

```
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

- `--` - Basic connection (simple energy flow)
- `==` - Strong connection (amplified energy flow)
- `~~` - Harmonic connection (resonant frequency)
- `~=` - Unstable connection (fluctuating energy)
- `->` - Directional flow (one-way)
- `<->` - Bidirectional flow (two-way)

## Multi-Circle Formations

### Formation Syntax

For formations with multiple circles, use the bracket syntax with circle IDs:

```
[circle_id:circle_definition] [circle_id:circle_definition]... connection_definitions...
```

**Components:**
- `[<circle_id>:<circle>]` - Circle definition with unique identifier
- `<circle_id> <connection_type> <circle_id>` - Connection between circles by ID

**Examples:**
```
[main:C3 FWE] [support:C2 MW] main--support
[core:C5 FLIGEMWND] [boost:C1 F] [balance:C1 W] core==boost core~~balance
[alpha:C4 FWEO@M] [beta:C3 LND] [gamma:C2 GC] alpha->beta beta<->gamma
```

### Single Circle Notation

For single circles, brackets are optional:

```
C3 FWE          # Single circle (brackets optional)
[C3 FWE]        # Same circle with explicit brackets
[main:C3 FWE]   # Single circle with ID (for potential connections)
```

## Advanced Formations

### Multi-Layer Circles

Use `|` to separate layers from inner to outer:

```
C1 F|C3 WEO|C5 MLIGDN    # Three concentric layers
C2 FW@E|C4 MLOG         # Two layers with center element
```

### Complex Formations

#### Triangle Formation
```
[alpha:C3 FWE] [beta:C3 MLD] [gamma:C3 OGC] alpha--beta beta--gamma gamma--alpha
```

#### Star Formation with Hub
```
[hub:C1 C] [point1:C2 F] [point2:C2 W] [point3:C2 E] [point4:C2 M] [point5:C2 O]
hub==point1 hub==point2 hub==point3 hub==point4 hub==point5
```

#### Grid Formation (3x3)
```
[nw:C1 F] [n:C1 W] [ne:C1 E] [w:C1 M] [center:C1 O] [e:C1 L] [sw:C1 G] [s:C1 D] [se:C1 C]
nw--n n--ne w--center center--e sw--s s--se
nw--w n--center ne--e w--sw center--s e--se
```

### Positional Information (Optional)

3D coordinates can be specified in parentheses:

```
C3 FWE(0,0,0)           # Circle at origin
[main:C3 FWE(5,0,2)]    # Circle with ID at position (5,0,2)
[support:C3 FWE(10,-3,1)] # Circle with ID at position (10,-3,1)
main--support             # Connection between positioned circles
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
### Stability Enhancement Formation

```
[stabilizer:C3 EEE] [core:C3 WWW] [amplifier:C3 EEE]
[foundation1:C3 WWW] [nexus:C3 FFF] [foundation2:C3 WWW]
[anchor1:C3 EEE] [anchor2:C3 WWW] [anchor3:C3 EEE]

stabilizer=core core=amplifier
foundation1=nexus nexus=foundation2
anchor1=anchor2 anchor2=anchor3

stabilizer=foundation1 amplifier=foundation2
foundation1=anchor1 nexus=anchor2 foundation2=anchor3
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
[main:C3 FWE] [support:C4 MLGO] [boost:C2 FW] main--support support==boost
```

**JSON Equivalent:**
```json
{
  "formation": {
    "circles": [
      {
        "id": "main",
        "radius": 3,
        "elements": ["Fire", "Water", "Earth"]
      },
      {
        "id": "support", 
        "radius": 4,
        "elements": ["Metal", "Light", "Light", "Wood"]
      },
      {
        "id": "boost",
        "radius": 2,
        "elements": ["Fire", "Water"]
      }
    ],
    "connections": [
      {"source": "main", "target": "support", "type": "basic"},
      {"source": "support", "target": "boost", "type": "strong"}
    ]
  }
}
```

**Size Reduction:** ~80% smaller

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
State = "?" | "!" | "~"
CenterElement = Element
Connection = "-" | "=" | "~" | "~=" | "->" | "<->"
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
amplifier_simple=[C1 F3] == [C5 WEMO@C] == [C1 W3]
chaos_seal=[C2 V] ~~ [C6 CEMOGD] ~~ [C2 V]
```

### 4. Interactive Design
```
> create [C3 FWE] -- [C4 MLGO]
Created formation with 2 circles, 1 connection
> connect last == [C2 FW]  
Added strong connection to new circle
> show
[C3 FWE] -- [C4 MLGO] == [C2 FW]
```

### 5. Version Control
```diff
- stability_ward=[C3 E2E2E2] == [C3 W3W3W3] == [C3 E2E2E2]
+ stability_ward=[C3 E2E2E2] == [C3 W3W3W3] == [C3 E2E2E2]
```

### Working Examples (Tested Implementation)

The following examples are verified to work with the current implementation:

#### Single Circle Examples
```
C3 F W E                    # Basic three-element circle
C5 F2.5 W1.2 L0.8          # Circle with power levels
C4 F:core W:shield E:ground # Circle with named IDs
C2.5 F2:flame W1.5:water   # Combined power and IDs
C6 FWEMO                   # Compact format (5 elements)
C1 V                       # Single void element
C10 FWEML                  # Base + derived elements
C0.5 C:chaos D:dark        # Special elements with IDs
C7.5 F3.14:pi W2.71:euler  # Mathematical constants
C12 FWEMOLINDGCV           # All elements (12 total)
C3 FWE@M                   # Circle with center talisman
```

#### Multi-Circle Examples (Planned Implementation)
```
[main:C3 FWE] [support:C2 MW] main-support
[core:C5 FLIGEMWND] [boost:C1 F] [balance:C1 W] core=boost core~balance
[alpha:C4 FWEO@M] [beta:C3 LND] [gamma:C2 GC] alpha->beta beta<->gamma
```

**Note:** Multi-circle formations with ID-based connections are currently in development.

**Element Symbol Reference:**
- F=Fire, W=Water, E=Earth, M=Metal, O=Wood
- L=Lightning, N=Wind, I=Light, D=Dark, G=Forge  
- C=Chaos, V=Void

**Power Level Precision:**
- Supports decimal values: `F2.5`, `W1.2`, `L0.8`
- Mathematical constants: `F3.14:pi`, `W2.71:euler`
- High precision maintained in round-trip conversion

## Implementation Notes

### Current Implementation Status

**✅ Implemented Features:**
- Basic circle parsing: `C3 FWE`
- Power levels: `C5 F2.5 W1.2 L0.8`
- Talisman IDs: `C4 F:core W:shield E:ground`
- Combined notation: `C2.5 F2:flame W1.5:water`
- Compact format: `C6 FWEMO` (each letter as separate element)
- Decimal radius support: `C2.5`
- All 12 element types: Fire, Water, Earth, Metal, Wood, Lightning, Wind, Light, Dark, Forge, Chaos, Void
- Round-trip serialization/deserialization
- Mathematical precision power levels: `F3.14:pi`

**⚠️ Planned Features (Not Yet Implemented):**
- Element states (`*`, `?`, `!`, `~`)
- Center elements (`@`)
- Multi-circle formations with connections
- Position notation
- Multi-layer circles
- Geometric patterns and shorthand
- Temporal and conditional logic
- Macro definitions

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
C3 FWE[t0] -> C3 MLD[t1] -> C3 OGC[t2]    # Time-based transitions
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
