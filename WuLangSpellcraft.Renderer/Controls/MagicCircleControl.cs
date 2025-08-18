using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WuLangSpellcraft.Core;

namespace WuLangSpellcraft.Renderer.Controls
{
    /// <summary>
    /// Custom control for rendering a magic circle with talismans arranged in positions
    /// </summary>
    public class MagicCircleControl : UserControl
    {
        public static readonly DependencyProperty MagicCircleProperty =
            DependencyProperty.Register(nameof(MagicCircle), typeof(MagicCircle), typeof(MagicCircleControl),
                new PropertyMetadata(null, OnMagicCircleChanged));

        public static readonly DependencyProperty ShowConnectionsProperty =
            DependencyProperty.Register(nameof(ShowConnections), typeof(bool), typeof(MagicCircleControl),
                new PropertyMetadata(true, OnRenderPropertyChanged));

        public static readonly DependencyProperty ShowEffectsProperty =
            DependencyProperty.Register(nameof(ShowEffects), typeof(bool), typeof(MagicCircleControl),
                new PropertyMetadata(true, OnRenderPropertyChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MagicCircleControl),
                new PropertyMetadata(false, OnRenderPropertyChanged));

        public static readonly DependencyProperty RefreshTriggerProperty =
            DependencyProperty.Register(nameof(RefreshTrigger), typeof(DateTime), typeof(MagicCircleControl),
                new PropertyMetadata(DateTime.Now, OnRenderPropertyChanged));

        // Event for when the circle position changes due to dragging
        public event EventHandler? PositionChanged;
        public event EventHandler? PositionChanging; // For real-time updates during drag
        
        // Event for when the circle is selected (clicked)
        public event EventHandler? Selected;
        
        // Event for when a talisman should be removed from this circle
        public event EventHandler<TalismanRemovedEventArgs>? TalismanRemoved;

        public MagicCircle? MagicCircle
        {
            get => (MagicCircle?)GetValue(MagicCircleProperty);
            set => SetValue(MagicCircleProperty, value);
        }

        public bool ShowConnections
        {
            get => (bool)GetValue(ShowConnectionsProperty);
            set => SetValue(ShowConnectionsProperty, value);
        }

        public bool ShowEffects
        {
            get => (bool)GetValue(ShowEffectsProperty);
            set => SetValue(ShowEffectsProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public DateTime RefreshTrigger
        {
            get => (DateTime)GetValue(RefreshTriggerProperty);
            set => SetValue(RefreshTriggerProperty, value);
        }

        // Dynamic sizing constants
        private const double BasePixelsPerUnit = 20; // 20 pixels per logical radius unit
        private const double MinCircleRadius = 60;   // Minimum visual radius
        private const double MaxCircleRadius = 300;  // Maximum visual radius
        private const double BaseTalismanSize = 40;  // Base talisman size
        private const double CanvasPadding = 40;

        // Drag functionality
        private bool _isDragging = false;
        private bool _mousePressed = false;
        private Point _lastMousePosition;
        private Point _pressPosition;
        private Canvas? _parentCanvas;
        private const double DragThreshold = 5.0; // Minimum distance to start dragging

        public MagicCircleControl()
        {
            UpdateControlSize();
            ClipToBounds = false; // Allow talismans to be fully visible
            RenderMagicCircle();
            
            // Enable mouse interaction
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            this.MouseLeftButtonUp += OnMouseLeftButtonUp;
            this.MouseMove += OnMouseMove;
            this.MouseLeave += OnMouseLeave;
            this.Cursor = Cursors.Hand;
        }

        private void UpdateControlSize()
        {
            var visualRadius = GetVisualCircleRadius();
            var talismanSize = GetAdaptiveTalismanSize();
            
            Width = (visualRadius + talismanSize + CanvasPadding) * 2;
            Height = (visualRadius + talismanSize + CanvasPadding) * 2;
        }

        private double GetVisualCircleRadius()
        {
            if (MagicCircle == null) return MinCircleRadius;
            
            var calculatedRadius = MagicCircle.Radius * BasePixelsPerUnit;
            return Math.Max(MinCircleRadius, Math.Min(MaxCircleRadius, calculatedRadius));
        }

        private double GetAdaptiveTalismanSize()
        {
            if (MagicCircle == null) return BaseTalismanSize;
            
            var maxTalismans = GetMaxTalismansForCircle();
            var visualRadius = GetVisualCircleRadius();
            
            // Calculate optimal talisman size based on circle circumference and talisman count
            var availableCircumference = 2 * Math.PI * visualRadius;
            var spacingPerTalisman = availableCircumference / Math.Max(1, maxTalismans);
            var optimalSize = Math.Min(spacingPerTalisman * 0.7, BaseTalismanSize); // 70% of available space
            
            return Math.Max(20, Math.Min(BaseTalismanSize, optimalSize)); // Between 20-40 pixels
        }

        private static void OnMagicCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MagicCircleControl control)
            {
                control.UpdateControlSize();
                control.RenderMagicCircle();
            }
        }

        private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MagicCircleControl control)
            {
                control.RenderMagicCircle();
            }
        }

        private void RenderMagicCircle()
        {
            var canvas = new Canvas
            {
                Width = Width,
                Height = Height,
                Background = Brushes.Transparent
            };

            if (MagicCircle == null)
            {
                Content = CreateEmptyCircle();
                return;
            }

            // Add a visible indicator showing the current talisman count and render timestamp
            var debugInfo = new TextBlock
            {
                Text = $"Talismans: {MagicCircle.Talismans.Count} | Rendered: {DateTime.Now:HH:mm:ss.fff}",
                FontSize = 10,
                Foreground = Brushes.Yellow,
                Background = Brushes.Black,
                Padding = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetLeft(debugInfo, 5);
            Canvas.SetTop(debugInfo, 5);
            canvas.Children.Add(debugInfo);

            // Draw main circle background
            DrawBackgroundCircle(canvas);

            // Draw nested circles first (so they appear inside the main circle)
            DrawNestedCircles(canvas);

            // Draw connections (so they appear behind talismans)
            if (ShowConnections)
            {
                DrawConnections(canvas);
            }

            // Draw talismans
            DrawTalismans(canvas);

            // Draw spell effects
            if (ShowEffects)
            {
                DrawSpellEffects(canvas);
            }

            // Add circle info
            AddCircleInfo(canvas);

            Content = canvas;
        }

        private UIElement CreateEmptyCircle()
        {
            var visualRadius = GetVisualCircleRadius();
            var ellipse = new Ellipse
            {
                Width = visualRadius * 2,
                Height = visualRadius * 2,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 10, 10 },
                Fill = Brushes.Transparent
            };

            var canvas = new Canvas();
            Canvas.SetLeft(ellipse, (Width - ellipse.Width) / 2);
            Canvas.SetTop(ellipse, (Height - ellipse.Height) / 2);
            canvas.Children.Add(ellipse);

            return canvas;
        }

        private void DrawBackgroundCircle(Canvas canvas)
        {
            if (MagicCircle == null) return;

            var centerX = Width / 2;
            var centerY = Height / 2;
            var visualRadius = GetVisualCircleRadius();

            // Outer circle
            var outerCircle = new Ellipse
            {
                Width = visualRadius * 2,
                Height = visualRadius * 2,
                Stroke = GetCircleBrush(),
                StrokeThickness = 3,
                Fill = new SolidColorBrush(Color.FromArgb(20, 100, 100, 200))
            };

            Canvas.SetLeft(outerCircle, centerX - visualRadius);
            Canvas.SetTop(outerCircle, centerY - visualRadius);
            canvas.Children.Add(outerCircle);

            // Inner circle
            var innerRadius = visualRadius * 0.3;
            var innerCircle = new Ellipse
            {
                Width = innerRadius * 2,
                Height = innerRadius * 2,
                Stroke = GetCircleBrush(),
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft(innerCircle, centerX - innerRadius);
            Canvas.SetTop(innerCircle, centerY - innerRadius);
            canvas.Children.Add(innerCircle);

            // Draw selection indicator if this circle is selected
            if (IsSelected)
            {
                DrawSelectionIndicator(canvas, centerX, centerY, visualRadius);
            }

            // Draw position markers
            DrawPositionMarkers(canvas, centerX, centerY);
        }

        private void DrawPositionMarkers(Canvas canvas, double centerX, double centerY)
        {
            if (MagicCircle == null) return;

            var maxTalismans = GetMaxTalismansForCircle();
            var visualRadius = GetVisualCircleRadius();
            
            for (int i = 0; i < maxTalismans; i++)
            {
                var angle = (i * 360.0 / maxTalismans - 90) * Math.PI / 180;
                var x = centerX + Math.Cos(angle) * visualRadius;
                var y = centerY + Math.Sin(angle) * visualRadius;

                var marker = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.DarkGray,
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(marker, x - 4);
                Canvas.SetTop(marker, y - 4);
                canvas.Children.Add(marker);
            }
        }

        private void DrawSelectionIndicator(Canvas canvas, double centerX, double centerY, double visualRadius)
        {
            // Outer selection ring
            var selectionRadius = visualRadius + 15; // Slightly larger than the main circle
            var selectionRing = new Ellipse
            {
                Width = selectionRadius * 2,
                Height = selectionRadius * 2,
                Stroke = Brushes.Gold,
                StrokeThickness = 4,
                Fill = Brushes.Transparent,
                Opacity = 0.8,
                StrokeDashArray = new DoubleCollection { 10, 5 } // Dashed line for animated effect
            };

            Canvas.SetLeft(selectionRing, centerX - selectionRadius);
            Canvas.SetTop(selectionRing, centerY - selectionRadius);
            canvas.Children.Add(selectionRing);

            // Selection corner indicators (like selection handles)
            var cornerSize = 12;
            var cornerDistance = selectionRadius + 10;
            var corners = new[]
            {
                new Point(centerX, centerY - cornerDistance), // Top
                new Point(centerX + cornerDistance, centerY), // Right
                new Point(centerX, centerY + cornerDistance), // Bottom
                new Point(centerX - cornerDistance, centerY)  // Left
            };

            foreach (var corner in corners)
            {
                var cornerIndicator = new Rectangle
                {
                    Width = cornerSize,
                    Height = cornerSize,
                    Fill = Brushes.Gold,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    Opacity = 0.9
                };

                Canvas.SetLeft(cornerIndicator, corner.X - cornerSize / 2);
                Canvas.SetTop(cornerIndicator, corner.Y - cornerSize / 2);
                canvas.Children.Add(cornerIndicator);
            }

            // Add a subtle glow effect to the main circle when selected
            var glowCircle = new Ellipse
            {
                Width = (visualRadius + 8) * 2,
                Height = (visualRadius + 8) * 2,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Gold,
                StrokeThickness = 2,
                Opacity = 0.3,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gold,
                    BlurRadius = 8,
                    ShadowDepth = 0,
                    Opacity = 0.6
                }
            };

            Canvas.SetLeft(glowCircle, centerX - (visualRadius + 8));
            Canvas.SetTop(glowCircle, centerY - (visualRadius + 8));
            canvas.Children.Add(glowCircle);
        }

        private void DrawConnections(Canvas canvas)
        {
            if (MagicCircle?.Connections == null) return;

            var centerX = Width / 2;
            var centerY = Height / 2;

            foreach (var connection in MagicCircle.Connections)
            {
                // For connections between circles, we'll draw a simple line
                // since the current model doesn't have position indices for connections
                var line = new Line
                {
                    X1 = centerX - 50,
                    Y1 = centerY,
                    X2 = centerX + 50,
                    Y2 = centerY,
                    Stroke = GetConnectionBrush(connection.Strength),
                    StrokeThickness = Math.Max(1, connection.Strength * 4),
                    Opacity = 0.7
                };

                // Add glow effect for strong connections
                if (connection.Strength > 0.5)
                {
                    line.Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = GetConnectionColor(connection.Strength),
                        BlurRadius = 6,
                        ShadowDepth = 0,
                        Opacity = 0.5
                    };
                }

                canvas.Children.Add(line);
            }
        }

        private void DrawNestedCircles(Canvas canvas)
        {
            if (MagicCircle?.NestedCircles == null || !MagicCircle.NestedCircles.Any()) return;

            var centerX = Width / 2;
            var centerY = Height / 2;
            var mainVisualRadius = GetVisualCircleRadius();

            foreach (var nestedCircle in MagicCircle.NestedCircles)
            {
                // Calculate nested circle visual radius based on its actual radius and nesting scale
                var nestedScale = nestedCircle.NestedScale > 0 ? nestedCircle.NestedScale : 0.6;
                var nestedVisualRadius = Math.Max(20, nestedCircle.Radius * BasePixelsPerUnit * nestedScale);
                
                // Ensure nested circle fits within the main circle
                if (nestedVisualRadius > mainVisualRadius * 0.8)
                {
                    nestedVisualRadius = mainVisualRadius * 0.8;
                }

                // Draw nested circle background
                var nestedBackground = new Ellipse
                {
                    Width = nestedVisualRadius * 2,
                    Height = nestedVisualRadius * 2,
                    Stroke = GetNestedCircleBrush(nestedCircle),
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent,
                    Opacity = 0.8
                };

                Canvas.SetLeft(nestedBackground, centerX - nestedVisualRadius);
                Canvas.SetTop(nestedBackground, centerY - nestedVisualRadius);
                canvas.Children.Add(nestedBackground);

                // Draw nested circle inner border
                var nestedInner = new Ellipse
                {
                    Width = (nestedVisualRadius - 10) * 2,
                    Height = (nestedVisualRadius - 10) * 2,
                    Stroke = GetNestedCircleBrush(nestedCircle),
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Opacity = 0.5
                };

                Canvas.SetLeft(nestedInner, centerX - (nestedVisualRadius - 10));
                Canvas.SetTop(nestedInner, centerY - (nestedVisualRadius - 10));
                canvas.Children.Add(nestedInner);

                // Draw nested circle's talismans
                DrawNestedTalismans(canvas, nestedCircle, centerX, centerY, nestedVisualRadius);

                // Add nested circle label
                var nestedLabel = new TextBlock
                {
                    Text = nestedCircle.Name,
                    FontSize = 8,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Black,
                        BlurRadius = 2,
                        ShadowDepth = 1
                    }
                };

                Canvas.SetLeft(nestedLabel, centerX - 30);
                Canvas.SetTop(nestedLabel, centerY + nestedVisualRadius + 5);
                canvas.Children.Add(nestedLabel);
            }
        }

        private void DrawNestedTalismans(Canvas canvas, MagicCircle nestedCircle, double centerX, double centerY, double nestedRadius)
        {
            if (!nestedCircle.Talismans.Any()) return;

            var maxNestedTalismans = Math.Max(3, (int)(2 * Math.PI * nestedCircle.Radius / 2.0));
            var talismanSize = Math.Max(20, nestedRadius / 4); // Smaller talismans for nested circles
            var talismanPlacementRadius = nestedRadius * 0.7; // Place talismans slightly inside the nested circle

            for (int i = 0; i < nestedCircle.Talismans.Count && i < maxNestedTalismans; i++)
            {
                var talisman = nestedCircle.Talismans[i];
                var angle = (i * 360.0 / maxNestedTalismans - 90) * Math.PI / 180;
                var x = centerX + Math.Cos(angle) * talismanPlacementRadius;
                var y = centerY + Math.Sin(angle) * talismanPlacementRadius;

                // Create a simple talisman representation for nested circles
                var talismanShape = new Ellipse
                {
                    Width = talismanSize,
                    Height = talismanSize,
                    Fill = GetBrushForElement(talisman.PrimaryElement.Type),
                    Stroke = Brushes.White,
                    StrokeThickness = 1,
                    Opacity = 0.9
                };

                Canvas.SetLeft(talismanShape, x - talismanSize / 2);
                Canvas.SetTop(talismanShape, y - talismanSize / 2);
                canvas.Children.Add(talismanShape);
            }
        }

        private Brush GetNestedCircleBrush(MagicCircle nestedCircle)
        {
            if (!nestedCircle.Talismans.Any()) return Brushes.DarkGray;

            var dominantElement = nestedCircle.Talismans
                .GroupBy(t => t.PrimaryElement.Type)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? ElementType.Void;

            return GetBrushForElement(dominantElement);
        }

        private void DrawTalismans(Canvas canvas)
        {
            if (MagicCircle == null) return;

            var centerX = Width / 2;
            var centerY = Height / 2;
            var maxTalismans = GetMaxTalismansForCircle();
            var visualRadius = GetVisualCircleRadius();
            var talismanSize = GetAdaptiveTalismanSize();
            
            // Position talismans slightly outside the circle but within the canvas bounds
            // Add extra spacing to account for the larger talisman control size
            var talismanRadius = visualRadius + talismanSize / 2 + 20; 

            for (int i = 0; i < MagicCircle.Talismans.Count && i < maxTalismans; i++)
            {
                var talisman = MagicCircle.Talismans[i];
                var angle = (i * 360.0 / maxTalismans - 90) * Math.PI / 180;
                var x = centerX + Math.Cos(angle) * talismanRadius;
                var y = centerY + Math.Sin(angle) * talismanRadius;

                var talismanControl = new TalismanControl
                {
                    Talisman = talisman,
                    Width = 90,  // Match the TalismanControl's new size
                    Height = 90
                };

                // Wire up the removal event
                talismanControl.TalismanRemoved += OnTalismanRemoved;

                // Center the larger talisman control at the calculated position
                Canvas.SetLeft(talismanControl, x - 45); // Half of the 90px width
                Canvas.SetTop(talismanControl, y - 45);  // Half of the 90px height
                canvas.Children.Add(talismanControl);
            }
        }

        private void DrawSpellEffects(Canvas canvas)
        {
            if (MagicCircle == null) return;

            var spellEffect = MagicCircle.CalculateSpellEffect();
            if (spellEffect.Power < 0.1) return;

            var centerX = Width / 2;
            var centerY = Height / 2;

            // Central effect indicator
            var effectRadius = Math.Min(spellEffect.Power * 20, 40);
            var effectCircle = new Ellipse
            {
                Width = effectRadius * 2,
                Height = effectRadius * 2,
                Fill = GetEffectBrush(spellEffect),
                Opacity = 0.6
            };

            Canvas.SetLeft(effectCircle, centerX - effectRadius);
            Canvas.SetTop(effectCircle, centerY - effectRadius);
            canvas.Children.Add(effectCircle);

            // Effect description
            var effectText = new TextBlock
            {
                Text = $"{spellEffect.Type}\n{spellEffect.Power:F1}",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.White,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 2,
                    ShadowDepth = 1
                }
            };

            Canvas.SetLeft(effectText, centerX - 30);
            Canvas.SetTop(effectText, centerY - 15);
            canvas.Children.Add(effectText);
        }

        private void AddCircleInfo(Canvas canvas)
        {
            if (MagicCircle == null) return;

            var infoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
                Margin = new Thickness(5)
            };

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Circle: {MagicCircle.Name}",
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 2, 5, 2)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Layer: {MagicCircle.Layer}",
                Foreground = Brushes.LightGray,
                FontSize = 10,
                Margin = new Thickness(5, 0, 5, 2)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Talismans: {MagicCircle.Talismans.Count}/{GetMaxTalismansForCircle()}",
                Foreground = Brushes.LightGray,
                FontSize = 10,
                Margin = new Thickness(5, 0, 5, 2)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Power: {MagicCircle.PowerOutput:F1}",
                Foreground = Brushes.LightGray,
                FontSize = 10,
                Margin = new Thickness(5, 0, 5, 2)
            });

            // Position the info panel to the side of the circle, not overlapping it
            var visualRadius = GetVisualCircleRadius();
            var centerX = Width / 2;
            var centerY = Height / 2;
            var panelX = centerX + visualRadius + 50; // Place to the right of the circle
            var panelY = centerY - 50; // Slightly above center

            // Draw a connecting line from circle to info panel
            var connectionLine = new Line
            {
                X1 = centerX + visualRadius,
                Y1 = centerY,
                X2 = panelX - 5,
                Y2 = panelY + 25, // Connect to middle of panel
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 3, 3 },
                Opacity = 0.7
            };
            canvas.Children.Add(connectionLine);

            Canvas.SetLeft(infoPanel, panelX);
            Canvas.SetTop(infoPanel, panelY);
            canvas.Children.Add(infoPanel);
        }

        private int GetMaxTalismansForCircle()
        {
            if (MagicCircle == null) return 8;
            
            // Calculate based on circle radius and minimum spacing
            var circumference = 2 * Math.PI * MagicCircle.Radius;
            var minSpacing = 2.0;
            return Math.Max(3, (int)(circumference / minSpacing));
        }

        private Brush GetCircleBrush()
        {
            if (MagicCircle == null || MagicCircle.Talismans.Count == 0) return Brushes.Gray;

            var dominantElement = MagicCircle.Talismans
                .GroupBy(t => t.PrimaryElement.Type)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? ElementType.Earth;

            return GetBrushForElement(dominantElement);
        }

        private Brush GetConnectionBrush(double strength)
        {
            var intensity = (byte)(255 * Math.Min(strength, 1.0));
            return new SolidColorBrush(Color.FromRgb(intensity, (byte)(255 - intensity / 2), 100));
        }

        private Color GetConnectionColor(double strength)
        {
            var intensity = (byte)(255 * Math.Min(strength, 1.0));
            return Color.FromRgb(intensity, (byte)(255 - intensity / 2), 100);
        }

        private Brush GetEffectBrush(SpellEffect effect)
        {
            return effect.Type switch
            {
                SpellEffectType.Flow => Brushes.CornflowerBlue,
                SpellEffectType.Projectile => Brushes.OrangeRed,
                SpellEffectType.Barrier => Brushes.SandyBrown,
                SpellEffectType.Enhancement => Brushes.Gold,
                SpellEffectType.Growth => Brushes.ForestGreen,
                SpellEffectType.Hybrid => Brushes.Purple,
                _ => Brushes.Gray
            };
        }

        private Brush GetBrushForElement(ElementType elementType)
        {
            return elementType switch
            {
                // Base Elements
                ElementType.Water => Brushes.CornflowerBlue,
                ElementType.Fire => Brushes.OrangeRed,
                ElementType.Earth => Brushes.SandyBrown,
                ElementType.Metal => Brushes.Silver,
                ElementType.Wood => Brushes.ForestGreen,
                
                // Derived Elements
                ElementType.Lightning => Brushes.Magenta,
                ElementType.Wind => Brushes.LightGray,
                ElementType.Light => Brushes.Gold,
                ElementType.Dark => Brushes.DarkSlateBlue,
                ElementType.Forge => Brushes.DarkGray,
                ElementType.Chaos => Brushes.DarkRed,
                ElementType.Void => Brushes.Black,
                
                _ => Brushes.Gray
            };
        }

        private void OnTalismanRemoved(object? sender, TalismanRemovedEventArgs e)
        {
            // Bubble up the event to any listeners (like MainWindow)
            TalismanRemoved?.Invoke(this, e);
        }

        #region Mouse Event Handling

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MagicCircle == null) return;

            // Always handle selection immediately on mouse down
            Selected?.Invoke(this, EventArgs.Empty);

            // Prepare for potential dragging
            _mousePressed = true;
            _parentCanvas = FindParentCanvas();
            _pressPosition = _parentCanvas != null ? e.GetPosition(_parentCanvas) : e.GetPosition(this);
            _lastMousePosition = _pressPosition;
            
            this.CaptureMouse();
            
            // Don't mark as handled - let the selection logic in MainWindow also process this event
            // e.Handled = true; // Removed to allow selection to work
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mousePressed)
            {
                _mousePressed = false;
                
                if (_isDragging)
                {
                    _isDragging = false;
                    this.Cursor = Cursors.Hand;
                    
                    // Update the MagicCircle's position if it has position properties
                    if (MagicCircle != null && _parentCanvas != null)
                    {
                        var canvasLeft = Canvas.GetLeft(this);
                        var canvasTop = Canvas.GetTop(this);
                        
                        // Update the circle's logical position
                        MagicCircle.CenterX = canvasLeft + this.Width / 2;
                        MagicCircle.CenterY = canvasTop + this.Height / 2;
                        
                        // Notify that position has changed (for connection line updates)
                        PositionChanged?.Invoke(this, EventArgs.Empty);
                    }
                    
                    e.Handled = true; // Handle the event if we were dragging
                }
                
                this.ReleaseMouseCapture();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePressed && _parentCanvas != null)
            {
                var currentPosition = e.GetPosition(_parentCanvas);
                var distance = Math.Sqrt(Math.Pow(currentPosition.X - _pressPosition.X, 2) + 
                                       Math.Pow(currentPosition.Y - _pressPosition.Y, 2));
                
                // Only start dragging if we've moved beyond the threshold
                if (!_isDragging && distance > DragThreshold)
                {
                    _isDragging = true;
                    this.Cursor = Cursors.SizeAll;
                }
                
                // Only move the circle if we're actually dragging
                if (_isDragging)
                {
                    var deltaX = currentPosition.X - _lastMousePosition.X;
                    var deltaY = currentPosition.Y - _lastMousePosition.Y;
                    
                    var currentLeft = Canvas.GetLeft(this);
                    var currentTop = Canvas.GetTop(this);
                    
                    // Handle NaN values (when position hasn't been set yet)
                    if (double.IsNaN(currentLeft)) currentLeft = 0;
                    if (double.IsNaN(currentTop)) currentTop = 0;
                    
                    var newLeft = currentLeft + deltaX;
                    var newTop = currentTop + deltaY;
                    
                    // Constrain to canvas bounds
                    var canvasWidth = _parentCanvas.ActualWidth;
                    var canvasHeight = _parentCanvas.ActualHeight;
                    
                    newLeft = Math.Max(0, Math.Min(newLeft, canvasWidth - this.Width));
                    newTop = Math.Max(0, Math.Min(newTop, canvasHeight - this.Height));
                    
                    Canvas.SetLeft(this, newLeft);
                    Canvas.SetTop(this, newTop);
                    
                    // Update the circle's logical position during drag
                    if (MagicCircle != null)
                    {
                        MagicCircle.CenterX = newLeft + this.Width / 2;
                        MagicCircle.CenterY = newTop + this.Height / 2;
                        
                        // Notify for real-time connection updates
                        PositionChanging?.Invoke(this, EventArgs.Empty);
                    }
                    
                    _lastMousePosition = currentPosition;
                    e.Handled = true;
                }
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                this.Cursor = Cursors.Hand;
            }
        }

        private Canvas? FindParentCanvas()
        {
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent is Canvas canvas)
                    return canvas;
                
                parent = parent is FrameworkElement fe ? fe.Parent : null;
            }
            return null;
        }

        #endregion

        public void ForceRefresh()
        {
            // Force a complete re-render of the circle
            UpdateControlSize();
            RenderMagicCircle();
        }
    }
}
