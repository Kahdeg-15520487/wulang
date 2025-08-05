using System.Text;
using System.Text.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;

// Alias the conflicting types to avoid ambiguity
using ImageSharpColor = SixLabors.ImageSharp.Color;
using ImageSharpPointF = SixLabors.ImageSharp.PointF;
using ImageSharpFont = SixLabors.Fonts.Font;
using ImageSharpSystemFonts = SixLabors.Fonts.SystemFonts;
using ImageSharpFontStyle = SixLabors.Fonts.FontStyle;

namespace WuLangSpellcraft.Renderer.Services
{
    /// <summary>
    /// Service for saving and loading magic circles as images with spell data encoded in metadata
    /// </summary>
    public static class ImageSpellStorage
    {
        private const string SPELL_DATA_KEY = "SpellData";
        private const string CREATOR_KEY = "Creator";
        private const string VERSION_KEY = "Version";
        
        /// <summary>
        /// Saves a magic circle as a PNG image with spell data encoded in metadata
        /// </summary>
        public static void SaveSpellAsImage(MagicCircle circle, string filePath)
        {
            try
            {
                // Create a programmatic rendering of the spell
                var image = RenderMagicCircleToImage(circle);
                
                // Serialize the spell data
                var spellJson = SpellSerializer.SerializeCircle(circle);
                
                // Save as PNG with metadata
                SaveImageWithMetadata(image, spellJson, filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save spell image: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Loads a magic circle from a PNG image with spell data in metadata
        /// </summary>
        public static MagicCircle? LoadSpellFromImage(string filePath)
        {
            try
            {
                // Load the image and extract metadata
                var spellJson = ExtractMetadataFromImage(filePath);
                
                if (string.IsNullOrEmpty(spellJson))
                {
                    throw new InvalidOperationException("No spell data found in image metadata");
                }
                
                // Deserialize the spell data
                return SpellSerializer.DeserializeCircle(spellJson);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load spell from image: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Renders a magic circle to an ImageSharp image
        /// </summary>
        private static Image<Rgba32> RenderMagicCircleToImage(MagicCircle circle)
        {
            const int baseSize = 1600;
            const int padding = 200;
            var imageSize = baseSize + padding * 2;
            
            var image = new Image<Rgba32>(imageSize, imageSize);
            
            // Get font for rendering text - use fallback fonts to avoid missing font errors
            ImageSharpFont fontFamily;
            ImageSharpFont symbolFont;
            
            try
            {
                // Try to create the preferred fonts
                fontFamily = ImageSharpSystemFonts.CreateFont("Arial", 32, ImageSharpFontStyle.Bold);
                symbolFont = ImageSharpSystemFonts.CreateFont("Microsoft YaHei", 42, ImageSharpFontStyle.Bold);
            }
            catch
            {
                try
                {
                    // Fallback to any available font
                    var availableFontFamily = ImageSharpSystemFonts.Families.First();
                    fontFamily = availableFontFamily.CreateFont(32, ImageSharpFontStyle.Bold);
                    symbolFont = availableFontFamily.CreateFont(42, ImageSharpFontStyle.Bold);
                }
                catch
                {
                    // Last resort - use default system font
                    fontFamily = ImageSharpSystemFonts.CreateFont("Segoe UI", 32, ImageSharpFontStyle.Bold);
                    symbolFont = ImageSharpSystemFonts.CreateFont("Segoe UI", 42, ImageSharpFontStyle.Bold);
                }
            }
            
            image.Mutate(ctx =>
            {
                // Dark background
                ctx.Fill(ImageSharpColor.FromRgb(45, 45, 48));
                
                // Calculate circle properties
                var center = new ImageSharpPointF(imageSize / 2f, imageSize / 2f);
                var radius = Math.Max(400f, CalculateVisualRadius(circle.Radius)); // Force minimum 400px radius
                
                // Draw the main circle
                DrawCircleOutline(ctx, center, radius);
                
                // Draw inner circles
                DrawInnerCircles(ctx, center, radius);
                
                // Draw talismans
                DrawTalismans(ctx, center, radius, circle.Talismans.ToList(), fontFamily, symbolFont);
                
                // Draw connections
                if (circle.Talismans.Count > 1)
                {
                    DrawConnections(ctx, center, radius, circle.Talismans.ToList());
                }
                
                // Draw circle info
                DrawCircleInfo(ctx, circle, fontFamily, imageSize);
            });
            
            return image;
        }
        
        private static float CalculateVisualRadius(double logicalRadius)
        {
            const double basePixelsPerUnit = 60;
            const double minRadius = 400;
            const double maxRadius = 800;
            
            var visualRadius = logicalRadius * basePixelsPerUnit;
            return (float)Math.Max(minRadius, Math.Min(maxRadius, visualRadius));
        }
        
        private static void DrawCircleOutline(IImageProcessingContext ctx, ImageSharpPointF center, float radius)
        {
            var pen = SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(ImageSharpColor.LightGray, 4);
            ctx.Draw(pen, new EllipsePolygon(center, radius));
        }
        
        private static void DrawInnerCircles(IImageProcessingContext ctx, ImageSharpPointF center, float radius)
        {
            var innerRadius = radius * 0.7f;
            var pen = SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(ImageSharpColor.Gray, 3);
            ctx.Draw(pen, new EllipsePolygon(center, innerRadius));
            
            // Core circle
            var coreRadius = radius * 0.15f;
            var corePen = SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(ImageSharpColor.DarkGray, 3);
            ctx.Draw(corePen, new EllipsePolygon(center, coreRadius));
        }
        
        private static void DrawTalismans(IImageProcessingContext ctx, ImageSharpPointF center, float radius, 
            List<Talisman> talismans, ImageSharpFont font, ImageSharpFont symbolFont)
        {
            for (int i = 0; i < talismans.Count; i++)
            {
                var talisman = talismans[i];
                var angle = (i * 2 * Math.PI) / talismans.Count - Math.PI / 2; // Start from top
                var x = center.X + (float)(Math.Cos(angle) * radius);
                var y = center.Y + (float)(Math.Sin(angle) * radius);
                var position = new ImageSharpPointF(x, y);
                
                DrawTalisman(ctx, position, talisman, font, symbolFont);
            }
        }
        
        private static void DrawTalisman(IImageProcessingContext ctx, ImageSharpPointF position, Talisman talisman, 
            ImageSharpFont font, ImageSharpFont symbolFont)
        {
            const float talismanSize = 90;
            
            // Get element color
            var elementColor = GetColorForElement(talisman.PrimaryElement.Type);
            
            // Draw background shape
            DrawTalismanShape(ctx, position, talisman.PrimaryElement.Type, elementColor, talismanSize);
            
            // Draw element symbol
            var symbol = GetSymbolForElement(talisman.PrimaryElement.Type);
            var textColor = ImageSharpColor.White;
            
            var textOptions = new RichTextOptions(symbolFont)
            {
                Origin = position,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            ctx.DrawText(textOptions, symbol, textColor);
            
            // Draw secondary elements as smaller indicators
            for (int i = 0; i < Math.Min(talisman.SecondaryElements.Count, 3); i++)
            {
                var secondaryAngle = (i * 120) * Math.PI / 180;
                var secondaryRadius = 45;
                var secX = position.X + (float)(Math.Cos(secondaryAngle) * secondaryRadius);
                var secY = position.Y + (float)(Math.Sin(secondaryAngle) * secondaryRadius);
                
                var secColor = GetColorForElement(talisman.SecondaryElements[i].Type);
                ctx.Fill(secColor, new EllipsePolygon(new ImageSharpPointF(secX, secY), 10));
            }
        }
        
        private static void DrawTalismanShape(IImageProcessingContext ctx, ImageSharpPointF position, 
            ElementType elementType, ImageSharpColor color, float size)
        {
            var brush = color;
            var pen = SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(ImageSharpColor.White, 2);
            
            switch (elementType)
            {
                case ElementType.Water:
                    ctx.Fill(brush, new EllipsePolygon(position, size / 2));
                    ctx.Draw(pen, new EllipsePolygon(position, size / 2));
                    break;
                    
                case ElementType.Fire:
                    DrawTriangle(ctx, position, size, brush, pen);
                    break;
                    
                case ElementType.Earth:
                    var rect = new RectangularPolygon(position.X - size/2, position.Y - size/2, size, size);
                    ctx.Fill(brush, rect);
                    ctx.Draw(pen, rect);
                    break;
                    
                case ElementType.Metal:
                    DrawDiamond(ctx, position, size, brush, pen);
                    break;
                    
                case ElementType.Wood:
                    DrawHexagon(ctx, position, size, brush, pen);
                    break;
                    
                default:
                    // Default to circle for derived elements
                    ctx.Fill(brush, new EllipsePolygon(position, size / 2));
                    ctx.Draw(pen, new EllipsePolygon(position, size / 2));
                    break;
            }
        }
        
        private static void DrawTriangle(IImageProcessingContext ctx, ImageSharpPointF center, float size, 
            ImageSharpColor brush, SolidPen pen)
        {
            var radius = size / 2;
            var points = new ImageSharpPointF[3];
            for (int i = 0; i < 3; i++)
            {
                var angle = (i * 120 - 90) * Math.PI / 180;
                points[i] = new ImageSharpPointF(
                    center.X + (float)(Math.Cos(angle) * radius),
                    center.Y + (float)(Math.Sin(angle) * radius)
                );
            }
            var polygon = new Polygon(new LinearLineSegment(points));
            ctx.Fill(brush, polygon);
            ctx.Draw(pen, polygon);
        }
        
        private static void DrawDiamond(IImageProcessingContext ctx, ImageSharpPointF center, float size, 
            ImageSharpColor brush, SolidPen pen)
        {
            var halfSize = size / 2;
            var points = new ImageSharpPointF[]
            {
                new(center.X, center.Y - halfSize),
                new(center.X + halfSize, center.Y),
                new(center.X, center.Y + halfSize),
                new(center.X - halfSize, center.Y)
            };
            var polygon = new Polygon(new LinearLineSegment(points));
            ctx.Fill(brush, polygon);
            ctx.Draw(pen, polygon);
        }
        
        private static void DrawHexagon(IImageProcessingContext ctx, ImageSharpPointF center, float size, 
            ImageSharpColor brush, SolidPen pen)
        {
            var radius = size / 2;
            var points = new ImageSharpPointF[6];
            for (int i = 0; i < 6; i++)
            {
                var angle = i * 60 * Math.PI / 180;
                points[i] = new ImageSharpPointF(
                    center.X + (float)(Math.Cos(angle) * radius),
                    center.Y + (float)(Math.Sin(angle) * radius)
                );
            }
            var polygon = new Polygon(new LinearLineSegment(points));
            ctx.Fill(brush, polygon);
            ctx.Draw(pen, polygon);
        }
        
        private static void DrawConnections(IImageProcessingContext ctx, ImageSharpPointF center, float radius, 
            List<Talisman> talismans)
        {
            var pen = SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(ImageSharpColor.Magenta, 2);
            
            for (int i = 0; i < talismans.Count; i++)
            {
                var nextIndex = (i + 1) % talismans.Count;
                
                var angle1 = (i * 2 * Math.PI) / talismans.Count - Math.PI / 2;
                var angle2 = (nextIndex * 2 * Math.PI) / talismans.Count - Math.PI / 2;
                
                var point1 = new ImageSharpPointF(
                    center.X + (float)(Math.Cos(angle1) * radius),
                    center.Y + (float)(Math.Sin(angle1) * radius)
                );
                var point2 = new ImageSharpPointF(
                    center.X + (float)(Math.Cos(angle2) * radius),
                    center.Y + (float)(Math.Sin(angle2) * radius)
                );
                
                ctx.DrawLine(pen, point1, point2);
            }
        }
        
        private static void DrawCircleInfo(IImageProcessingContext ctx, MagicCircle circle, ImageSharpFont font, int imageSize)
        {
            // Calculate simple metrics from the circle
            var totalTalismans = circle.Talismans.Count;
            var totalElements = circle.Talismans.Sum(t => 1 + t.SecondaryElements.Count);
            
            var text = $"{circle.Name}\nTalismans: {totalTalismans}\nElements: {totalElements}";
            var textOptions = new RichTextOptions(font)
            {
                Origin = new ImageSharpPointF(30, imageSize - 120),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            
            ctx.DrawText(textOptions, text, ImageSharpColor.White);
        }
        
        private static ImageSharpColor GetColorForElement(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Water => ImageSharpColor.CornflowerBlue,
                ElementType.Fire => ImageSharpColor.OrangeRed,
                ElementType.Earth => ImageSharpColor.SandyBrown,
                ElementType.Metal => ImageSharpColor.Silver,
                ElementType.Wood => ImageSharpColor.ForestGreen,
                ElementType.Lightning => ImageSharpColor.Magenta,
                ElementType.Wind => ImageSharpColor.LightGray,
                ElementType.Light => ImageSharpColor.Gold,
                ElementType.Dark => ImageSharpColor.DarkSlateBlue,
                ElementType.Forge => ImageSharpColor.DarkGray,
                ElementType.Chaos => ImageSharpColor.DarkRed,
                ElementType.Void => ImageSharpColor.Black,
                _ => ImageSharpColor.Gray
            };
        }
        
        private static string GetSymbolForElement(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Water => "水",
                ElementType.Fire => "火",
                ElementType.Earth => "土",
                ElementType.Metal => "金",
                ElementType.Wood => "木",
                ElementType.Lightning => "雷",
                ElementType.Wind => "風",
                ElementType.Light => "光",
                ElementType.Dark => "闇",
                ElementType.Forge => "鍛",
                ElementType.Chaos => "混沌",
                ElementType.Void => "虛無",
                _ => "?"
            };
        }
        
        private static void SaveImageWithMetadata(Image<Rgba32> image, string spellData, string filePath)
        {
            // Convert ImageSharp image to System.Drawing.Bitmap for metadata handling
            using var memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);
            memoryStream.Position = 0;
            
            using var bitmap = new Bitmap(memoryStream);
            
            // Add spell data as a text property
            var spellDataBytes = Encoding.UTF8.GetBytes(spellData);
            var spellProperty = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true)!;
            spellProperty.Id = 0x010E; // ImageDescription property
            spellProperty.Type = 2; // ASCII string
            spellProperty.Len = spellDataBytes.Length + 1;
            spellProperty.Value = spellDataBytes.Concat(new byte[] { 0 }).ToArray();
            
            // Add creator info
            var creatorBytes = Encoding.UTF8.GetBytes("WuLangSpellcraft");
            var creatorProperty = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true)!;
            creatorProperty.Id = 0x013B; // Artist property
            creatorProperty.Type = 2; // ASCII string
            creatorProperty.Len = creatorBytes.Length + 1;
            creatorProperty.Value = creatorBytes.Concat(new byte[] { 0 }).ToArray();
            
            // Add version info
            var versionBytes = Encoding.UTF8.GetBytes("1.0");
            var versionProperty = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true)!;
            versionProperty.Id = 0x0131; // Software property
            versionProperty.Type = 2; // ASCII string
            versionProperty.Len = versionBytes.Length + 1;
            versionProperty.Value = versionBytes.Concat(new byte[] { 0 }).ToArray();
            
            // Set the properties
            bitmap.SetPropertyItem(spellProperty);
            bitmap.SetPropertyItem(creatorProperty);
            bitmap.SetPropertyItem(versionProperty);
            
            // Save as PNG with metadata
            bitmap.Save(filePath, ImageFormat.Png);
        }
        
        private static string ExtractMetadataFromImage(string filePath)
        {
            using var bitmap = new Bitmap(filePath);
            
            try
            {
                // Try to get the ImageDescription property (where we stored spell data)
                var spellProperty = bitmap.GetPropertyItem(0x010E);
                if (spellProperty?.Value != null)
                {
                    var spellData = Encoding.UTF8.GetString(spellProperty.Value).TrimEnd('\0');
                    return spellData;
                }
            }
            catch (ArgumentException)
            {
                // Property not found
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// Checks if an image file contains spell data
        /// </summary>
        public static bool IsSpellImage(string filePath)
        {
            try
            {
                var spellData = ExtractMetadataFromImage(filePath);
                return !string.IsNullOrEmpty(spellData);
            }
            catch
            {
                return false;
            }
        }
    }
}
