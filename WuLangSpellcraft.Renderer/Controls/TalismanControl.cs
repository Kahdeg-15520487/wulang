using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Renderer.Controls
{
    /// <summary>
    /// Event arguments for when a talisman is removed
    /// </summary>
    public class TalismanRemovedEventArgs : EventArgs
    {
        public Talisman Talisman { get; }
        
        public TalismanRemovedEventArgs(Talisman talisman)
        {
            Talisman = talisman;
        }
    }
    /// <summary>
    /// Custom control for rendering a talisman with elemental styling
    /// </summary>
    public class TalismanControl : UserControl
    {
        public static readonly DependencyProperty TalismanProperty =
            DependencyProperty.Register(nameof(Talisman), typeof(Talisman), typeof(TalismanControl),
                new PropertyMetadata(null, OnTalismanChanged));

        // Event fired when the talisman should be removed
        public event EventHandler<TalismanRemovedEventArgs>? TalismanRemoved;

        public Talisman? Talisman
        {
            get => (Talisman?)GetValue(TalismanProperty);
            set => SetValue(TalismanProperty, value);
        }

        public TalismanControl()
        {
            // Increase the control size to accommodate secondary elements
            Width = 90;  // Increased from 60 to 90
            Height = 90; // Increased from 60 to 90
            ClipToBounds = false; // Allow the full talisman to be visible
            
            // Add click handling for removal
            MouseLeftButtonDown += OnTalismanClicked;
            MouseEnter += OnTalismanMouseEnter;
            MouseLeave += OnTalismanMouseLeave;
            Cursor = System.Windows.Input.Cursors.Hand;
            
            RenderTalisman();
        }

        private static void OnTalismanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TalismanControl control)
            {
                control.RenderTalisman();
            }
        }

        private void OnTalismanClicked(object sender, MouseButtonEventArgs e)
        {
            if (Talisman != null)
            {
                // Fire the removal event
                TalismanRemoved?.Invoke(this, new TalismanRemovedEventArgs(Talisman));
                e.Handled = true; // Prevent event from bubbling up further
            }
        }

        private void OnTalismanMouseEnter(object sender, MouseEventArgs e)
        {
            // Add visual feedback on hover
            Opacity = 0.8;
        }

        private void OnTalismanMouseLeave(object sender, MouseEventArgs e)
        {
            // Remove visual feedback when not hovering
            Opacity = 1.0;
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
            ToolTip = $"{Talisman.Name}\nPower: {Talisman.PowerLevel:F1}\nStability: {Talisman.Stability:F1}\n\nClick to remove";

            Content = grid;
        }

        private UIElement CreateEmptyTalisman()
        {
            var ellipse = new Ellipse
            {
                Width = 50,  // Match the background shape size
                Height = 50,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 5 },
                Fill = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            return ellipse;
        }

        private Shape CreateBackgroundShape()
        {
            if (Talisman == null) return new Ellipse();

            var shape = GetShapeForElement(Talisman.PrimaryElement.Type);
            // Keep the background shape centered but smaller than the full control
            shape.Width = 50;  // Fixed size for the main talisman shape
            shape.Height = 50;
            shape.Fill = GetBrushForElement(Talisman.PrimaryElement.Type, 0.3);
            shape.Stroke = GetBrushForElement(Talisman.PrimaryElement.Type, 1.0);
            shape.StrokeThickness = 3;
            shape.HorizontalAlignment = HorizontalAlignment.Center;
            shape.VerticalAlignment = VerticalAlignment.Center;
            
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
                
                // Position secondary elements around the edge with enough space
                var angle = (i * 120) * Math.PI / 180; // 120 degrees apart
                var radius = 30; // Increased radius to utilize the larger control size
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
                // Base Elements
                ElementType.Water => new Ellipse(), // Circle for flow
                ElementType.Fire => CreateTriangle(), // Triangle for energy
                ElementType.Earth => new Rectangle(), // Square for stability
                ElementType.Metal => CreateDiamond(), // Diamond for precision
                ElementType.Wood => CreateHexagon(), // Hexagon for growth
                
                // Derived Elements
                ElementType.Lightning => CreateZigzag(), // Zigzag for electricity
                ElementType.Wind => CreateSpiral(), // Spiral for air currents
                ElementType.Light => CreateStar(), // Star for radiance
                ElementType.Dark => CreateCrescent(), // Crescent for shadow
                ElementType.Forge => CreateAnvil(), // Anvil shape for crafting
                ElementType.Chaos => CreateIrregular(), // Irregular shape for chaos
                ElementType.Void => CreateVoid(), // Empty ring for void
                
                _ => new Ellipse()
            };
        }

        private Shape CreateTriangle()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50; // Fixed size to match background shape
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
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateDiamond()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50; // Fixed size to match background shape
            var center = size / 2;
            var radius = center * 0.8;
            
            // Diamond shape
            points.Add(new Point(center, center - radius)); // Top
            points.Add(new Point(center + radius, center)); // Right
            points.Add(new Point(center, center + radius)); // Bottom
            points.Add(new Point(center - radius, center)); // Left
            
            polygon.Points = points;
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateHexagon()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50; // Fixed size to match background shape
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
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateZigzag()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50;
            var center = size / 2;
            var width = center * 1.2;
            var height = center * 1.6;
            
            // Lightning bolt zigzag pattern
            points.Add(new Point(center - width/4, center - height/2));
            points.Add(new Point(center + width/8, center - height/4));
            points.Add(new Point(center - width/8, center - height/8));
            points.Add(new Point(center + width/4, center + height/8));
            points.Add(new Point(center, center + height/4));
            points.Add(new Point(center + width/6, center));
            points.Add(new Point(center - width/6, center + height/6));
            points.Add(new Point(center - width/3, center - height/6));
            
            polygon.Points = points;
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateSpiral()
        {
            var path = new Path();
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            var size = 50;
            var center = size / 2;
            var radius = center * 0.7;
            
            // Start from center and spiral outward
            figure.StartPoint = new Point(center, center);
            
            // Create spiral using arcs
            for (int i = 0; i < 3; i++)
            {
                var currentRadius = radius * (i + 1) / 3;
                var arc = new ArcSegment
                {
                    Point = new Point(center + currentRadius * Math.Cos(i * Math.PI), 
                                    center + currentRadius * Math.Sin(i * Math.PI)),
                    Size = new Size(currentRadius, currentRadius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = i > 0
                };
                figure.Segments.Add(arc);
            }
            
            geometry.Figures.Add(figure);
            path.Data = geometry;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            path.VerticalAlignment = VerticalAlignment.Center;
            return path;
        }

        private Shape CreateStar()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50;
            var center = size / 2;
            var outerRadius = center * 0.8;
            var innerRadius = outerRadius * 0.4;
            
            // 5-pointed star
            for (int i = 0; i < 10; i++)
            {
                var angle = (i * 36 - 90) * Math.PI / 180;
                var radius = (i % 2 == 0) ? outerRadius : innerRadius;
                var x = center + Math.Cos(angle) * radius;
                var y = center + Math.Sin(angle) * radius;
                points.Add(new Point(x, y));
            }
            
            polygon.Points = points;
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateCrescent()
        {
            var path = new Path();
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            var size = 50;
            var center = size / 2;
            var radius = center * 0.8;
            
            // Crescent moon shape using two arcs
            figure.StartPoint = new Point(center - radius * 0.3, center - radius * 0.8);
            
            // Outer arc (right side of crescent)
            var outerArc = new ArcSegment
            {
                Point = new Point(center - radius * 0.3, center + radius * 0.8),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
            figure.Segments.Add(outerArc);
            
            // Inner arc (left side of crescent)
            var innerArc = new ArcSegment
            {
                Point = new Point(center - radius * 0.3, center - radius * 0.8),
                Size = new Size(radius * 0.6, radius * 0.6),
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = true
            };
            figure.Segments.Add(innerArc);
            
            geometry.Figures.Add(figure);
            path.Data = geometry;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            path.VerticalAlignment = VerticalAlignment.Center;
            return path;
        }

        private Shape CreateAnvil()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50;
            var center = size / 2;
            var width = center * 1.4;
            var height = center * 1.2;
            
            // Anvil shape - flat top with wider base
            points.Add(new Point(center - width/3, center - height/2)); // Top left
            points.Add(new Point(center + width/3, center - height/2)); // Top right
            points.Add(new Point(center + width/2, center - height/4)); // Right shoulder
            points.Add(new Point(center + width/2, center + height/4));  // Right side
            points.Add(new Point(center + width/3, center + height/2));  // Bottom right
            points.Add(new Point(center - width/3, center + height/2));  // Bottom left
            points.Add(new Point(center - width/2, center + height/4));  // Left side
            points.Add(new Point(center - width/2, center - height/4));  // Left shoulder
            
            polygon.Points = points;
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateIrregular()
        {
            var polygon = new Polygon();
            var points = new PointCollection();
            var size = 50;
            var center = size / 2;
            var baseRadius = center * 0.8;
            
            // Chaotic irregular shape with random-looking but deterministic points
            var angles = new[] { 0, 45, 90, 135, 180, 225, 270, 315 };
            var radiusVariations = new[] { 1.0, 0.6, 1.2, 0.8, 0.9, 1.1, 0.7, 1.3 };
            
            for (int i = 0; i < angles.Length; i++)
            {
                var angle = angles[i] * Math.PI / 180;
                var radius = baseRadius * radiusVariations[i];
                var x = center + Math.Cos(angle) * radius;
                var y = center + Math.Sin(angle) * radius;
                points.Add(new Point(x, y));
            }
            
            polygon.Points = points;
            polygon.HorizontalAlignment = HorizontalAlignment.Center;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            return polygon;
        }

        private Shape CreateVoid()
        {
            var path = new Path();
            var geometry = new PathGeometry();
            var outerFigure = new PathFigure();
            var innerFigure = new PathFigure();
            var size = 50;
            var center = size / 2;
            var outerRadius = center * 0.8;
            var innerRadius = center * 0.4;
            
            // Outer circle
            outerFigure.StartPoint = new Point(center - outerRadius, center);
            var outerArc1 = new ArcSegment
            {
                Point = new Point(center + outerRadius, center),
                Size = new Size(outerRadius, outerRadius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
            var outerArc2 = new ArcSegment
            {
                Point = new Point(center - outerRadius, center),
                Size = new Size(outerRadius, outerRadius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
            outerFigure.Segments.Add(outerArc1);
            outerFigure.Segments.Add(outerArc2);
            outerFigure.IsClosed = true;
            
            // Inner circle (void)
            innerFigure.StartPoint = new Point(center - innerRadius, center);
            var innerArc1 = new ArcSegment
            {
                Point = new Point(center + innerRadius, center),
                Size = new Size(innerRadius, innerRadius),
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = true
            };
            var innerArc2 = new ArcSegment
            {
                Point = new Point(center - innerRadius, center),
                Size = new Size(innerRadius, innerRadius),
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = true
            };
            innerFigure.Segments.Add(innerArc1);
            innerFigure.Segments.Add(innerArc2);
            innerFigure.IsClosed = true;
            
            geometry.Figures.Add(outerFigure);
            geometry.Figures.Add(innerFigure);
            geometry.FillRule = FillRule.EvenOdd; // Creates the void effect
            
            path.Data = geometry;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            path.VerticalAlignment = VerticalAlignment.Center;
            return path;
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
                // Base Elements
                ElementType.Water => Colors.CornflowerBlue,
                ElementType.Fire => Colors.OrangeRed,
                ElementType.Earth => Colors.SandyBrown,
                ElementType.Metal => Colors.Silver,
                ElementType.Wood => Colors.ForestGreen,
                
                // Derived Elements
                ElementType.Lightning => Colors.Magenta,
                ElementType.Wind => Colors.LightGray,
                ElementType.Light => Colors.Gold,
                ElementType.Dark => Colors.DarkSlateBlue,
                ElementType.Forge => Colors.DarkGray,
                ElementType.Chaos => Colors.DarkRed,
                ElementType.Void => Colors.Black,
                
                _ => Colors.Gray
            };
        }

        private string GetSymbolForElement(ElementType elementType)
        {
            return elementType switch
            {
                // Base Elements
                ElementType.Water => "水",
                ElementType.Fire => "火",
                ElementType.Earth => "土",
                ElementType.Metal => "金",
                ElementType.Wood => "木",
                
                // Derived Elements
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
    }
}
