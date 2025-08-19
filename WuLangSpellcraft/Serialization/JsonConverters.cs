using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Serialization
{
    /// <summary>
    /// JSON converter for Element objects
    /// </summary>
    public class ElementConverter : JsonConverter<Element>
    {
        public override Element Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<ElementData>(ref reader, options);
            return data?.ToElement() ?? throw new JsonException("Failed to deserialize Element");
        }

        public override void Write(Utf8JsonWriter writer, Element value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new ElementData(value), options);
        }
    }

    /// <summary>
    /// JSON converter for Talisman objects
    /// </summary>
    public class TalismanConverter : JsonConverter<Talisman>
    {
        public override Talisman Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<TalismanData>(ref reader, options);
            return data?.ToTalisman() ?? throw new JsonException("Failed to deserialize Talisman");
        }

        public override void Write(Utf8JsonWriter writer, Talisman value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new TalismanData(value), options);
        }
    }

    /// <summary>
    /// JSON converter for MagicCircle objects
    /// </summary>
    public class MagicCircleConverter : JsonConverter<MagicCircle>
    {
        public override MagicCircle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = JsonSerializer.Deserialize<MagicCircleData>(ref reader, options);
            return data?.ToMagicCircle() ?? throw new JsonException("Failed to deserialize MagicCircle");
        }

        public override void Write(Utf8JsonWriter writer, MagicCircle value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new MagicCircleData(value), options);
        }
    }
}
