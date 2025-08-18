# Circle Notation Format - Implementation Specification

## Overview

This document outlines the technical implementation details for adding Circle Notation Format (CNF) support to the WuLang Spellcraft system as an alternative to JSON serialization.

## Architecture Design

### Core Components

```
WuLangSpellcraft.Core.Notation/
├── CircleNotationParser.cs      # CNF → Object conversion
├── CircleNotationSerializer.cs  # Object → CNF conversion
├── NotationValidator.cs         # Syntax and semantic validation
├── NotationExceptions.cs        # Custom exception types
└── NotationExtensions.cs        # Extension methods for existing classes
```

### Integration Points

1. **SpellSerialization.cs** - Add CNF methods alongside JSON methods
2. **MagicCircle.cs** - Add ToNotation() and FromNotation() extension methods
3. **SpellConfiguration.cs** - Support CNF format in serialization
4. **Demo applications** - Add CNF import/export features

## Class Design

### CircleNotationParser

```csharp
public static class CircleNotationParser
{
    // Single circle parsing
    public static MagicCircle ParseCircle(string notation);
    public static bool TryParseCircle(string notation, out MagicCircle circle);
    
    // Formation parsing
    public static List<MagicCircle> ParseFormation(string notation);
    public static bool TryParseFormation(string notation, out List<MagicCircle> circles);
    
    // SpellConfiguration parsing
    public static SpellConfiguration ParseConfiguration(string notation);
    public static bool TryParseConfiguration(string notation, out SpellConfiguration config);
    
    // Internal parsing methods
    private static CircleInfo ParseCircleInfo(string circleNotation);
    private static ElementInfo ParseElement(string elementNotation);
    private static ConnectionInfo ParseConnection(string connectionNotation);
    private static Position3D ParsePosition(string positionNotation);
    private static string GenerateTalismanId(int position); // Auto-generate IDs like "t1", "t2", etc.
}
```

### CircleNotationSerializer

```csharp
public static class CircleNotationSerializer
{
    // Single circle serialization
    public static string SerializeCircle(MagicCircle circle);
    public static string SerializeCircle(MagicCircle circle, NotationOptions options);
    
    // Formation serialization
    public static string SerializeFormation(IEnumerable<MagicCircle> circles);
    public static string SerializeFormation(IEnumerable<MagicCircle> circles, NotationOptions options);
    
    // SpellConfiguration serialization
    public static string SerializeConfiguration(SpellConfiguration config);
    public static string SerializeConfiguration(SpellConfiguration config, NotationOptions options);
    
    // Internal serialization methods
    private static string SerializeElements(IEnumerable<Element> elements);
    private static string SerializeConnections(MagicCircle circle);
    private static string SerializePosition(double x, double y, double z);
}
```

### NotationOptions

```csharp
public class NotationOptions
{
    // Formatting options
    public bool IncludePowerLevels { get; set; } = true;
    public bool IncludePositions { get; set; } = true;
    public bool IncludeElementStates { get; set; } = true;
    public bool IncludeTalismanIds { get; set; } = false;    // Only include when explicitly set
    public bool UseCompactFormat { get; set; } = false;
    
    // Layout options
    public bool MultiLine { get; set; } = false;
    public string IndentString { get; set; } = "  ";
    public int MaxLineLength { get; set; } = 80;
    
    // Content options
    public bool IncludeComments { get; set; } = false;
    public bool IncludeMetadata { get; set; } = false;
    public bool ValidateOnSerialize { get; set; } = true;
}
```

### NotationValidator

```csharp
public static class NotationValidator
{
    public static ValidationResult ValidateCircle(string notation);
    public static ValidationResult ValidateFormation(string notation);
    public static ValidationResult ValidateConfiguration(string notation);
    
    public static bool IsValidElementSymbol(char symbol);
    public static bool IsValidConnection(string connection);
    public static bool IsValidRadius(int radius);
    
    private static ValidationResult ValidateSyntax(string notation);
    private static ValidationResult ValidateSemantics(ParsedNotation parsed);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
}
```

## Parsing Strategy

```csharp
public static MagicCircle ParseCircle(string notation)
{
    var elements = ParseElements(notation); // Returns list of (ElementType, PowerLevel, State, StringId?)
    
    var circle = new MagicCircle(Guid.NewGuid(), "Parsed Circle", radius);
    
    foreach (var (elementType, power, state, stringId) in elements)
    {
        // Create TalismanData with string ID (or auto-generate if null)
        var talismanData = new TalismanData
        {
            Id = stringId ?? Guid.NewGuid().ToString("N")[..8],  // Use provided ID or generate short one
            ElementType = elementType,
            PowerLevel = power,
            State = state
        };
        
        var talisman = new Talisman(talismanData);
        circle.AddTalisman(talisman, position);
    }
    
    return circle;
}
```

### Serialization Strategy

```csharp
public static string SerializeCircle(MagicCircle circle, NotationOptions? options = null)
{
    options ??= NotationDefaults.StandardOptions;
    
    // Build CNF string with optional IDs
    var elements = circle.Talismans.Select(t => 
    {
        var elementStr = ElementSymbols.GetSymbol(t.PrimaryElement.Type);
        if (t.PrimaryElement.Energy != 1.0) elementStr += t.PrimaryElement.Energy;
        if (options.IncludeTalismanIds && !string.IsNullOrEmpty(t.Id)) 
        {
            elementStr += $":{t.Id}";
        }
        return elementStr;
    });
    
    return $"C{circle.Radius} {string.Join("", elements)}";
}
```

### Element Symbol Registry

```csharp
public static class ElementSymbols
{
    private static readonly Dictionary<ElementType, char> _typeToSymbol = new()
    {
        { ElementType.Fire, 'F' },
        { ElementType.Water, 'W' },
        { ElementType.Earth, 'E' },
        { ElementType.Metal, 'M' },
        { ElementType.Wood, 'O' },
        { ElementType.Lightning, 'L' },
        { ElementType.Wind, 'A' },
        { ElementType.Light, 'G' },
        { ElementType.Dark, 'D' },
        { ElementType.Forge, 'R' },
        { ElementType.Chaos, 'C' },
        { ElementType.Void, 'V' }
    };
    
    private static readonly Dictionary<char, ElementType> _symbolToType = 
        _typeToSymbol.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    
    public static char GetSymbol(ElementType type) => _typeToSymbol[type];
    public static ElementType GetElementType(char symbol) => _symbolToType[symbol];
    public static bool IsValidSymbol(char symbol) => _symbolToType.ContainsKey(symbol);
}
```

## Parser Implementation

### Lexical Analysis

```csharp
public class NotationLexer
{
    private readonly string _input;
    private int _position;
    
    public IEnumerable<Token> Tokenize()
    {
        while (!IsAtEnd())
        {
            yield return NextToken();
        }
    }
    
    private Token NextToken()
    {
        char c = Advance();
        return c switch
        {
            'C' => new Token(TokenType.CircleStart, "C"),
            '[' => new Token(TokenType.LeftBracket, "["),
            ']' => new Token(TokenType.RightBracket, "]"),
            '-' => new Token(TokenType.BasicConnection, "-"),
            '=' => new Token(TokenType.StrongConnection, "="),
            '~' => new Token(TokenType.HarmonicConnection, "~"),
            '@' => new Token(TokenType.CenterMarker, "@"),
            '(' => new Token(TokenType.LeftParen, "("),
            ')' => new Token(TokenType.RightParen, ")"),
            _ when char.IsDigit(c) => ScanNumber(),
            _ when char.IsLetter(c) => ScanElement(),
            _ => throw new NotationSyntaxException($"Unexpected character: {c}")
        };
    }
}

public enum TokenType
{
    CircleStart, LeftBracket, RightBracket,
    BasicConnection, StrongConnection, HarmonicConnection,
    CenterMarker, LeftParen, RightParen,
    Number, Element, EndOfInput
}
```

### Syntax Analysis

```csharp
public class NotationParser
{
    private readonly List<Token> _tokens;
    private int _current = 0;
    
    public ParsedFormation ParseFormation()
    {
        var formation = new ParsedFormation();
        
        // Parse first circle
        formation.Circles.Add(ParseCircleExpression());
        
        // Parse connections and additional circles
        while (!IsAtEnd() && IsConnection(Peek()))
        {
            var connection = ParseConnection();
            var circle = ParseCircleExpression();
            
            formation.Connections.Add(new ParsedConnection
            {
                FromIndex = formation.Circles.Count - 2,
                ToIndex = formation.Circles.Count - 1,
                Type = connection
            });
            
            formation.Circles.Add(circle);
        }
        
        return formation;
    }
    
    private ParsedCircle ParseCircleExpression()
    {
        if (Match(TokenType.LeftBracket))
        {
            var circle = ParseCircle();
            Consume(TokenType.RightBracket, "Expected ']' after circle");
            return circle;
        }
        
        return ParseCircle();
    }
}
```

## Serialization Integration

### Extended SpellSerialization

```csharp
public static partial class SpellSerializer
{
    // New CNF methods
    public static string SerializeCircleNotation(MagicCircle circle)
    {
        return CircleNotationSerializer.SerializeCircle(circle);
    }
    
    public static MagicCircle DeserializeCircleNotation(string notation)
    {
        return CircleNotationParser.ParseCircle(notation);
    }
    
    public static string SerializeFormationNotation(IEnumerable<MagicCircle> circles)
    {
        return CircleNotationSerializer.SerializeFormation(circles);
    }
    
    public static List<MagicCircle> DeserializeFormationNotation(string notation)
    {
        return CircleNotationParser.ParseFormation(notation);
    }
    
    public static string SerializeSpellConfigurationNotation(SpellConfiguration config)
    {
        return CircleNotationSerializer.SerializeConfiguration(config);
    }
    
    public static SpellConfiguration DeserializeSpellConfigurationNotation(string notation)
    {
        return CircleNotationParser.ParseConfiguration(notation);
    }
}
```

### Extension Methods

```csharp
public static class NotationExtensions
{
    public static string ToNotation(this MagicCircle circle)
    {
        return CircleNotationSerializer.SerializeCircle(circle);
    }
    
    public static string ToNotation(this MagicCircle circle, NotationOptions options)
    {
        return CircleNotationSerializer.SerializeCircle(circle, options);
    }
    
    public static string ToNotation(this SpellConfiguration config)
    {
        return CircleNotationSerializer.SerializeConfiguration(config);
    }
    
    public static string ToFormationNotation(this IEnumerable<MagicCircle> circles)
    {
        return CircleNotationSerializer.SerializeFormation(circles);
    }
}
```

## Error Handling

### Exception Hierarchy

```csharp
public class NotationException : Exception
{
    public NotationException(string message) : base(message) { }
    public NotationException(string message, Exception innerException) : base(message, innerException) { }
}

public class NotationSyntaxException : NotationException
{
    public int Position { get; }
    
    public NotationSyntaxException(string message, int position) : base(message)
    {
        Position = position;
    }
}

public class NotationSemanticException : NotationException
{
    public string InvalidElement { get; }
    
    public NotationSemanticException(string message, string element) : base(message)
    {
        InvalidElement = element;
    }
}
```

### Error Recovery

```csharp
public class ParseResult<T>
{
    public bool Success { get; set; }
    public T? Result { get; set; }
    public List<ParseError> Errors { get; set; } = new();
    public List<ParseWarning> Warnings { get; set; } = new();
    
    public static ParseResult<T> Successful(T result) => new() { Success = true, Result = result };
    public static ParseResult<T> Failed(params ParseError[] errors) => new() { Success = false, Errors = errors.ToList() };
}
```

## Performance Considerations

### Optimization Strategies

1. **String Interning** - Cache common element symbols and connection types
2. **Lazy Parsing** - Parse formations on-demand
3. **Builder Pattern** - Reuse StringBuilder instances for serialization
4. **Caching** - Cache parsed results for repeated access
5. **Memory Pooling** - Reuse token and node objects

### Benchmarking

```csharp
[MemoryDiagnoser]
public class NotationBenchmarks
{
    [Params("C3 FWE", "[C3 FWE] - [C4 MLGO]", "P5[C3 FWEMO]")]
    public string Notation { get; set; }
    
    [Benchmark]
    public MagicCircle ParseCircle() => CircleNotationParser.ParseCircle(Notation);
    
    [Benchmark]
    public string SerializeCircle()
    {
        var circle = CircleNotationParser.ParseCircle(Notation);
        return CircleNotationSerializer.SerializeCircle(circle);
    }
    
    [Benchmark]
    public string JsonSerialization()
    {
        var circle = CircleNotationParser.ParseCircle(Notation);
        return SpellSerializer.SerializeCircle(circle);
    }
}
```

## Testing Strategy

### Unit Test Categories

1. **Parser Tests**
   - Valid syntax parsing
   - Invalid syntax error handling
   - Edge cases and boundary conditions
   - Complex formation parsing

2. **Serializer Tests**
   - Round-trip consistency (parse → serialize → parse)
   - Format options handling
   - Output formatting validation
   - Performance with large formations

3. **Integration Tests**
   - JSON ↔ CNF conversion equivalence
   - Demo application integration
   - File I/O operations
   - Version compatibility

4. **Validation Tests**
   - Semantic rule enforcement
   - Element combination validation
   - Connection constraint checking
   - Formation stability analysis

### Test Data Sets

```csharp
public static class NotationTestData
{
    public static readonly string[] ValidCircles = 
    {
        "C1 F",
        "C3 FWE", 
        "C5 FLGEMWLD",
        "C4 FWEO@M",
        "C3 F2W1E3",
        "C2 F*W?@E!",
        "C3 FWE(0,0,0)"
    };
    
    public static readonly string[] ValidFormations =
    {
        "[C3 FWE] - [C4 MLGO]",
        "[C1 F] = [C5 WEMO@C] = [C1 W]",
        "[C3 FWE] ~ [C3 MLD] ~ [C3 OGC]"
    };
    
    public static readonly string[] InvalidNotations =
    {
        "C F",           // Missing radius
        "C3",            // Missing elements  
        "C3 XYZ",        // Invalid elements
        "[C3 FWE] & [C4 MLGO]", // Invalid connection
        "C-1 F"          // Invalid radius
    };
}
```

## Migration Strategy

### Phase 1: Core Implementation
- Implement basic parser and serializer
- Add unit tests for core functionality
- Create documentation and examples

### Phase 2: Integration
- Extend SpellSerialization with CNF methods
- Add extension methods to existing classes
- Update demo applications with CNF support

### Phase 3: Enhancement
- Add advanced features (shortcuts, macros)
- Implement performance optimizations
- Create conversion utilities (JSON ↔ CNF)

### Phase 4: Production
- Comprehensive testing and validation
- Performance benchmarking
- Documentation completion
- API stabilization

## Configuration

### Default Settings

```csharp
public static class NotationDefaults
{
    public static readonly NotationOptions StandardOptions = new()
    {
        IncludePowerLevels = true,
        IncludePositions = false,
        IncludeElementStates = true,
        UseCompactFormat = false,
        MultiLine = false,
        ValidateOnSerialize = true
    };
    
    public static readonly NotationOptions CompactOptions = new()
    {
        IncludePowerLevels = false,
        IncludePositions = false,
        IncludeElementStates = false,
        UseCompactFormat = true,
        MultiLine = false,
        ValidateOnSerialize = false
    };
}
```

## Future Enhancements

### Advanced Features
- Template/macro system for common patterns
- Conditional and probabilistic notation
- Temporal sequences and animations
- Interactive editing with real-time validation

### Tool Integration
- VS Code extension for syntax highlighting
- Command-line utilities for conversion
- Web-based notation editor
- Integration with version control systems
