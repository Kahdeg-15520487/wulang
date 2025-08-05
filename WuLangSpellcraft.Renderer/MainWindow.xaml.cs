using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;
using WuLangSpellcraft.Renderer.Controls;

namespace WuLangSpellcraft.Renderer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MagicCircle? _currentCircle;
    private readonly Random _random = new();

    public MainWindow()
    {
        InitializeComponent();
        InitializeTalismanLibrary();
        CreateNewCircle();
        
        // Test if controls are accessible
        Title = "Wu Lang Spellcraft Renderer - Loaded Successfully";
    }

    private void InitializeTalismanLibrary()
    {
        var talismanList = FindName("TalismanList") as StackPanel;
        if (talismanList == null) 
        {
            MessageBox.Show("TalismanList not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        var elements = new[]
        {
            (ElementType.Water, "CornflowerBlue"),
            (ElementType.Fire, "OrangeRed"),
            (ElementType.Earth, "SandyBrown"),
            (ElementType.Metal, "Silver"),
            (ElementType.Wood, "ForestGreen")
        };

        foreach (var (elementType, color) in elements)
        {
            var button = new Button
            {
                Content = $"Create {elementType} Talisman",
                Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString(color)!,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(5),
                Margin = new Thickness(0, 2, 0, 2),
                Tag = elementType
            };

            button.Click += CreateTalisman_Click;
            talismanList.Children.Add(button);
        }
    }

    private void CreateTalisman_Click(object sender, RoutedEventArgs e)
    {
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        var statusText = FindName("StatusText") as TextBlock;
        
        if (sender is Button button && button.Tag is ElementType elementType && _currentCircle != null)
        {
            var element = new Element(elementType) { IsActive = true };
            var talisman = new Talisman(element, $"{elementType} Talisman");
            
            if (_currentCircle.AddTalisman(talisman))
            {
                if (circleViewer != null)
                {
                    // Force the control to update by setting the property to null first, then back to the circle
                    circleViewer.MagicCircle = null;
                    circleViewer.MagicCircle = _currentCircle;
                    
                    // Also force a visual update
                    circleViewer.InvalidateVisual();
                    circleViewer.UpdateLayout();
                }
                UpdateCircleStats();
                if (statusText != null)
                    statusText.Text = $"Added {elementType} talisman to circle (Total: {_currentCircle.Talismans.Count})";
                    
                MessageBox.Show($"Successfully added {elementType} talisman!\nTotal talismans: {_currentCircle.Talismans.Count}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (statusText != null)
                    statusText.Text = "Circle is full - cannot add more talismans";
                MessageBox.Show("Circle is full!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void NewCircle_Click(object sender, RoutedEventArgs e)
    {
        CreateNewCircle();
    }

    private void CreateNewCircle()
    {
        _currentCircle = new MagicCircle("New Circle", 5.0);
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        var statusText = FindName("StatusText") as TextBlock;
        
        if (circleViewer != null)
        {
            // Force the control to update by setting the property to null first, then back to the circle
            circleViewer.MagicCircle = null;
            circleViewer.MagicCircle = _currentCircle;
            
            // Also force a visual update
            circleViewer.InvalidateVisual();
            circleViewer.UpdateLayout();
        }
        UpdateCircleStats();
        if (statusText != null)
            statusText.Text = "Created new magic circle";
    }

    private void LoadConfiguration_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Load feature not implemented yet", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveConfiguration_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Save feature not implemented yet", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Wu Lang Spellcraft Renderer\n\n" +
            "A visual programming language inspired by the Chinese Wu Xing (Five Elements) system.\n" +
            "Create and visualize magic circles with elemental talismans.\n\n" +
            "Elements:\n" +
            "• Water (水) - Flow and adaptation\n" +
            "• Fire (火) - Energy and transformation\n" +
            "• Earth (土) - Stability and grounding\n" +
            "• Metal (金) - Precision and structure\n" +
            "• Wood (木) - Growth and flexibility",
            "About Wu Lang Spellcraft",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void AddRandomTalisman_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null) return;

        var elementTypes = Enum.GetValues<ElementType>();
        var randomElementType = elementTypes[_random.Next(elementTypes.Length)];
        
        var element = new Element(randomElementType) { IsActive = true };
        var talisman = new Talisman(element, $"Random {randomElementType}");
        
        // Add some randomness to talisman properties
        talisman.PowerLevel = 1.0 + _random.NextDouble() * 4.0; // 1-5 power
        
        // Randomly add secondary elements
        if (_random.NextDouble() > 0.5)
        {
            var secondaryType = elementTypes[_random.Next(elementTypes.Length)];
            if (secondaryType != randomElementType)
            {
                talisman.SecondaryElements.Add(new Element(secondaryType) { IsActive = true });
            }
        }

        if (_currentCircle.AddTalisman(talisman))
        {
            var circleViewer = FindName("CircleViewer") as MagicCircleControl;
            var statusText = FindName("StatusText") as TextBlock;
            
            if (circleViewer != null)
            {
                // Force the control to update by setting the property to null first, then back to the circle
                circleViewer.MagicCircle = null;
                circleViewer.MagicCircle = _currentCircle;
                
                // Also force a visual update
                circleViewer.InvalidateVisual();
                circleViewer.UpdateLayout();
            }
            UpdateCircleStats();
            if (statusText != null)
                statusText.Text = $"Added random {randomElementType} talisman (Total: {_currentCircle.Talismans.Count})";
        }
        else
        {
            var statusText = FindName("StatusText") as TextBlock;
            if (statusText != null)
                statusText.Text = "Circle is full - cannot add more talismans";
        }
    }

    private void ClearCircle_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null) return;

        var result = MessageBox.Show("Are you sure you want to clear the current circle?", 
                                   "Clear Circle", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            _currentCircle.Talismans.Clear();
            var circleViewer = FindName("CircleViewer") as MagicCircleControl;
            var statusText = FindName("StatusText") as TextBlock;
            
            if (circleViewer != null)
            {
                // Force the control to update by setting the property to null first, then back to the circle
                circleViewer.MagicCircle = null;
                circleViewer.MagicCircle = _currentCircle;
                
                // Also force a visual update
                circleViewer.InvalidateVisual();
                circleViewer.UpdateLayout();
            }
            UpdateCircleStats();
            if (statusText != null)
                statusText.Text = "Cleared circle";
        }
    }

    private void GenerateDemoCircle_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null) return;

        // Clear existing talismans
        _currentCircle.Talismans.Clear();

        // Create a balanced demo circle with all elements
        var elements = Enum.GetValues<ElementType>();
        foreach (var elementType in elements)
        {
            var element = new Element(elementType) { IsActive = true };
            var talisman = new Talisman(element, $"Demo {elementType}")
            {
                PowerLevel = 2.0 + _random.NextDouble() * 2.0 // 2-4 power
            };
            
            _currentCircle.AddTalisman(talisman);
        }

        // Add a few more random talismans for visual interest
        for (int i = 0; i < 3; i++)
        {
            var randomType = elements[_random.Next(elements.Length)];
            var element = new Element(randomType) { IsActive = true };
            var talisman = new Talisman(element, $"Extra {randomType}")
            {
                PowerLevel = 1.0 + _random.NextDouble() * 3.0
            };
            
            if (!_currentCircle.AddTalisman(talisman))
                break; // Circle is full
        }

        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        var statusText = FindName("StatusText") as TextBlock;
        
        if (circleViewer != null)
        {
            // Force the control to update by setting the property to null first, then back to the circle
            circleViewer.MagicCircle = null;
            circleViewer.MagicCircle = _currentCircle;
            
            // Also force a visual update
            circleViewer.InvalidateVisual();
            circleViewer.UpdateLayout();
        }
        UpdateCircleStats();
        if (statusText != null)
            statusText.Text = "Generated demo circle with balanced elements";
    }

    private void UpdateCircleStats()
    {
        var circleStatsText = FindName("CircleStatsText") as TextBlock;
        if (circleStatsText == null) return;
        
        if (_currentCircle == null)
        {
            circleStatsText.Text = "No circle loaded";
            return;
        }

        var spellEffect = _currentCircle.CalculateSpellEffect();
        circleStatsText.Text = 
            $"Talismans: {_currentCircle.Talismans.Count} | " +
            $"Power: {_currentCircle.PowerOutput:F1} | " +
            $"Stability: {_currentCircle.Stability:F2} | " +
            $"Effect: {spellEffect.Type} ({spellEffect.Power:F1})";
    }
}