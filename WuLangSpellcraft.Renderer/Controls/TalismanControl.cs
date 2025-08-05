using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Renderer.Controls
{
    /// <summary>
    /// Custom control for rendering a talisman with elemental styling
    /// </summary>
    public class TalismanControl : UserControl
    {
        public static readonly DependencyProperty TalismanProperty =
            DependencyProperty.Register(nameof(Talisman), typeof(Talisman), typeof(TalismanControl),
                new PropertyMetadata(null, OnTalismanChanged));

        public Talisman? Talisman
        {
            get => (Talisman?)GetValue(TalismanProperty);
            set => SetValue(TalismanProperty, value);
        }

        public TalismanControl()
        {
            Width = 60;
            Height = 60;
            ClipToBounds = false; // Allow the full talisman to be visible
            RenderTalisman();
        }

        private static void OnTalismanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TalismanControl control)
            {
                control.RenderTalisman();
            }
        }

        private void RenderTalisman()
        {
            if (Talisman == null)
            {
                Content = CreateEmptyTalisman();
                return;
            }

            var grid = new Grid();
            
            // Background circle
            var background = CreateBackgroundShape();
            grid.Children.Add(background);

            // Primary element symbol
            var primarySymbol = CreateElementSymbol(Talisman.PrimaryElement, 0.8);
            grid.Children.Add(primarySymbol);

            // Secondary element symbols
            if (Talisman.SecondaryElements.Any())
            {
                var secondaryContainer = CreateSecondaryElementsContainer();
                grid.Children.Add(secondaryContainer);
            }

            // Power level indicator
            var powerIndicator = CreatePowerIndicator();
            grid.Children.Add(powerIndicator);

            // Stability indicator
            var stabilityIndicator = CreateStabilityIndicator();
            grid.Children.Add(stabilityIndicator);

            // Name tooltip
            ToolTip = $"{Talisman.Name}\nPower: {Talisman.PowerLevel:F1}\nStability: {Talisman.Stability:F1}";

            Content = grid;
        }

        private UIElement CreateEmptyTalisman()
        {
            var ellipse = new Ellipse
            {
                Width = Width - 4,
                Height = Height - 4,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 5 },
                Fill = Brushes.Transparent
            };

            return ellipse;
        }

        private Shape CreateBackgroundShape()
        {
            if (Talisman == null) return new Ellipse();

            var shape = GetShapeForElement(Talisman.PrimaryElement.Type);
            shape.Width = Width - 4;
            shape.Height = Height - 4;
            shape.Fill = GetBrushForElement(Talisman.PrimaryElement.Type, 0.3);
            shape.Stroke = GetBrushForElement(Talisman.PrimaryElement.Type, 1.0);
            shape.StrokeThickness = 3;
            
            // Add glow effect for active talismans
            if (Talisman.PrimaryElement.IsActive)
            {
                shape.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = GetColorForElement(Talisman.PrimaryElement.Type),
                    BlurRadius = 8,
                    ShadowDepth = 0,
                    Opacity = 0.6
                };
            }

            return shape;
        }

        private TextBlock CreateElementSymbol(Element element, double opacity)
        {
            return new TextBlock
            {
                Text = GetSymbolForElement(element.Type),
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = GetBrushForElement(element.Type, opacity),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 2,
                    ShadowDepth = 1,
                    Opacity = 0.5
                }
            };
        }

        private Grid CreateSecondaryElementsContainer()
        {
            if (Talisman == null) return new Grid();

            var container = new Grid();
            var count = Talisman.SecondaryElements.Count;
            
            for (int i = 0; i < count && i < 3; i++)
            {
                var element = Talisman.SecondaryElements[i];
                var symbol = CreateElementSymbol(element, 0.5);
                symbol.FontSize = 12;
                
                // Position secondary elements around the edge
                var angle = (i * 120) * Math.PI / 180; // 120 degrees apart
                var radius = 25;
                var x = Math.Cos(angle) * radius;
                var y = Math.Sin(angle) * radius;
                
                symbol.Margin = new Thickness(x, y, 0, 0);
                symbol.HorizontalAlignment = HorizontalAlignment.Center;
                symbol.VerticalAlignment = VerticalAlignment.Center;
                
                container.Children.Add(symbol);
            }

            return container;
        }

        private ProgressBar CreatePowerIndicator()
        {
            if (Talisman == null) return new ProgressBar();

            return new ProgressBar
            {
                Value = Math.Min(Talisman.PowerLevel * 20, 100), // Scale to 0-100
                Maximum = 100,
                Height = 4,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(5, 0, 5, 5),
                Foreground = GetBrushForElement(Talisman.PrimaryElement.Type, 0.8),
                Background = Brushes.DarkGray
            };
        }

        private Rectangle CreateStabilityIndicator()
        {
            if (Talisman == null) return new Rectangle();

            var color = Talisman.Stability > 0.7 ? Colors.Green :
                       Talisman.Stability > 0.4 ? Colors.Yellow : Colors.Red;

            return new Rectangle
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(color),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 5, 5, 0)
            };
        }

        private Shape GetShapeForElement(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Water => new Ellipse(), // Circle for flow
                ElementType.Fire => CreateTriangle(), // Triangle for energy
                ElementType.Earth => new Rectangle(), // Square for stability
                ElementType.Metal => CreateDiamond(), // Diamond for precision
                ElementType.Wood => CreateHexagon(), // Hexagon for growth
                _ => new Ellipse()
            };
        }

        private Shape CreateTriangle()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = Width - 4;
            var center = size / 2;
            var radius = center * 0.8;
            
            // Equilateral triangle pointing up
            for (int i = 0; i < 3; i++)
            {
                var angle = (i * 120 - 90) * Math.PI / 180;
                var x = center + Math.Cos(angle) * radius;
                var y = center + Math.Sin(angle) * radius;
                points.Add(new Point(x, y));
            }
            
            polygon.Points = points;
            return polygon;
        }

        private Shape CreateDiamond()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = Width - 4;
            var center = size / 2;
            var radius = center * 0.8;
            
            // Diamond shape
            points.Add(new Point(center, center - radius)); // Top
            points.Add(new Point(center + radius, center)); // Right
            points.Add(new Point(center, center + radius)); // Bottom
            points.Add(new Point(center - radius, center)); // Left
            
            polygon.Points = points;
            return polygon;
        }

        private Shape CreateHexagon()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = Width - 4;
            var center = size / 2;
            var radius = center * 0.8;
            
            // Regular hexagon
            for (int i = 0; i < 6; i++)
            {
                var angle = i * 60 * Math.PI / 180;
                var x = center + Math.Cos(angle) * radius;
                var y = center + Math.Sin(angle) * radius;
                points.Add(new Point(x, y));
            }
            
            polygon.Points = points;
            return polygon;
        }

        private Brush GetBrushForElement(ElementType elementType, double opacity)
        {
            var color = GetColorForElement(elementType);
            color.A = (byte)(255 * opacity);
            return new SolidColorBrush(color);
        }

        private Color GetColorForElement(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Water => Colors.CornflowerBlue,
                ElementType.Fire => Colors.OrangeRed,
                ElementType.Earth => Colors.SandyBrown,
                ElementType.Metal => Colors.Silver,
                ElementType.Wood => Colors.ForestGreen,
                _ => Colors.Gray
            };
        }

        private string GetSymbolForElement(ElementType elementType)
        {
            return elementType switch
            {
                ElementType.Water => "水",
                ElementType.Fire => "火",
                ElementType.Earth => "土",
                ElementType.Metal => "金",
                ElementType.Wood => "木",
                _ => "?"
            };
        }
    }
}
