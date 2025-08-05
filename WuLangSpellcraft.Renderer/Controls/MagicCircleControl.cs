using System.Windows;
using System.Windows.Controls;
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

        private const double CircleRadius = 120;
        private const double TalismanSize = 60;
        private const double CanvasPadding = 40;

        public MagicCircleControl()
        {
            Width = (CircleRadius + TalismanSize + CanvasPadding) * 2;
            Height = (CircleRadius + TalismanSize + CanvasPadding) * 2;
            ClipToBounds = false; // Allow talismans to be fully visible
            RenderMagicCircle();
        }

        private static void OnMagicCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MagicCircleControl control)
            {
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

            // Draw background circle
            DrawBackgroundCircle(canvas);

            // Draw connections first (so they appear behind talismans)
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
            var ellipse = new Ellipse
            {
                Width = CircleRadius * 2,
                Height = CircleRadius * 2,
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

            // Outer circle
            var outerCircle = new Ellipse
            {
                Width = CircleRadius * 2,
                Height = CircleRadius * 2,
                Stroke = GetCircleBrush(),
                StrokeThickness = 3,
                Fill = new SolidColorBrush(Color.FromArgb(20, 100, 100, 200))
            };

            Canvas.SetLeft(outerCircle, centerX - CircleRadius);
            Canvas.SetTop(outerCircle, centerY - CircleRadius);
            canvas.Children.Add(outerCircle);

            // Inner circle
            var innerRadius = CircleRadius * 0.3;
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

            // Draw position markers
            DrawPositionMarkers(canvas, centerX, centerY);
        }

        private void DrawPositionMarkers(Canvas canvas, double centerX, double centerY)
        {
            if (MagicCircle == null) return;

            var maxTalismans = GetMaxTalismansForCircle();
            for (int i = 0; i < maxTalismans; i++)
            {
                var angle = (i * 360.0 / maxTalismans - 90) * Math.PI / 180;
                var x = centerX + Math.Cos(angle) * CircleRadius;
                var y = centerY + Math.Sin(angle) * CircleRadius;

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

        private void DrawTalismans(Canvas canvas)
        {
            if (MagicCircle == null) return;

            var centerX = Width / 2;
            var centerY = Height / 2;
            var maxTalismans = GetMaxTalismansForCircle();
            
            // Position talismans slightly outside the circle but within the canvas bounds
            var talismanRadius = CircleRadius + TalismanSize / 2 + 10;

            for (int i = 0; i < MagicCircle.Talismans.Count && i < maxTalismans; i++)
            {
                var talisman = MagicCircle.Talismans[i];
                var angle = (i * 360.0 / maxTalismans - 90) * Math.PI / 180;
                var x = centerX + Math.Cos(angle) * talismanRadius;
                var y = centerY + Math.Sin(angle) * talismanRadius;

                var talismanControl = new TalismanControl
                {
                    Talisman = talisman,
                    Width = TalismanSize,
                    Height = TalismanSize
                };

                Canvas.SetLeft(talismanControl, x - TalismanSize / 2);
                Canvas.SetTop(talismanControl, y - TalismanSize / 2);
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

            Canvas.SetLeft(infoPanel, 5);
            Canvas.SetTop(infoPanel, 5);
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
                ElementType.Water => Brushes.CornflowerBlue,
                ElementType.Fire => Brushes.OrangeRed,
                ElementType.Earth => Brushes.SandyBrown,
                ElementType.Metal => Brushes.Silver,
                ElementType.Wood => Brushes.ForestGreen,
                _ => Brushes.Gray
            };
        }
    }
}
