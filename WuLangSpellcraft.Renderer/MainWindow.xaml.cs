using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Core.Serialization;
using WuLangSpellcraft.Renderer.Controls;
using WuLangSpellcraft.Renderer.Services;

namespace WuLangSpellcraft.Renderer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MagicCircle? _currentCircle;
    private MagicCircle? _selectedCircle; // For composition operations
    private readonly Random _random = new();
    private readonly List<MagicCircle> _allCircles = new(); // Track all circles in the composition

    // Canvas panning variables
    private bool _isPanning = false;
    private Point _lastPanPosition;

    public MainWindow()
    {
        InitializeComponent();
        InitializeTalismanLibrary();
        InitializeCompositionControls();
        InitializeCanvasPanning();
        
        // Wire up talisman removal event
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        if (circleViewer != null)
        {
            circleViewer.TalismanRemoved += OnTalismanRemoved;
        }
        
        CreateNewCircle();
        UpdateCircleCapacityLabel(); // Initialize capacity label
        
        // Test if controls are accessible
        Title = "Wu Lang Spellcraft Renderer - Loaded Successfully";
    }

    private void OnTalismanRemoved(object? sender, TalismanRemovedEventArgs e)
    {
        // Find which circle contains this talisman and remove it
        MagicCircle? targetCircle = null;
        
        // Check current circle first
        if (_currentCircle != null && _currentCircle.Talismans.Contains(e.Talisman))
        {
            targetCircle = _currentCircle;
        }
        else
        {
            // Check all circles in composition
            foreach (var circle in _allCircles)
            {
                if (circle.Talismans.Contains(e.Talisman))
                {
                    targetCircle = circle;
                    break;
                }
            }
        }
        
        if (targetCircle != null)
        {
            // Remove the talisman from the target circle
            targetCircle.Talismans.Remove(e.Talisman);
            
            // Update all visualizations
            UpdateCircleVisualization();
            UpdateCircleStats();
            UpdatePreview();
            
            // Update status
            var statusText = FindName("StatusText") as TextBlock;
            if (statusText != null)
                statusText.Text = $"Removed {e.Talisman.PrimaryElement.Type} talisman from {targetCircle.Name}";
        }
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
            // Base Elements
            (ElementType.Water, "CornflowerBlue"),
            (ElementType.Fire, "OrangeRed"),
            (ElementType.Earth, "SandyBrown"),
            (ElementType.Metal, "Silver"),
            (ElementType.Wood, "ForestGreen"),
            
            // Derived Elements
            (ElementType.Lightning, "Magenta"),
            (ElementType.Wind, "LightGray"),
            (ElementType.Light, "Gold"),
            (ElementType.Dark, "DarkSlateBlue"),
            (ElementType.Forge, "DarkGray"),
            (ElementType.Chaos, "DarkRed"),
            (ElementType.Void, "Black")
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

    private void InitializeCompositionControls()
    {
        // Initialize connection type combo box
        var connectionTypeCombo = FindName("ConnectionTypeCombo") as ComboBox;
        if (connectionTypeCombo != null)
        {
            connectionTypeCombo.SelectedIndex = 0; // Default to Direct
        }

        // Initialize nested size slider event handler
        var nestedSizeSlider = FindName("NestedSizeSlider") as Slider;
        if (nestedSizeSlider != null)
        {
            nestedSizeSlider.ValueChanged += NestedSizeSlider_ValueChanged;
        }

        // Initialize connection strength slider event handler
        var connectionStrengthSlider = FindName("ConnectionStrengthSlider") as Slider;
        if (connectionStrengthSlider != null)
        {
            connectionStrengthSlider.ValueChanged += ConnectionStrengthSlider_ValueChanged;
        }

        UpdateCompositionList();
    }

    private void InitializeCanvasPanning()
    {
        var mainCanvas = FindName("MainCanvas") as Canvas;
        if (mainCanvas != null)
        {
            // Add mouse event handlers for panning
            mainCanvas.PreviewMouseDown += MainCanvas_PreviewMouseDown;
            mainCanvas.PreviewMouseUp += MainCanvas_PreviewMouseUp;
            mainCanvas.PreviewMouseMove += MainCanvas_PreviewMouseMove;
            mainCanvas.MouseLeave += MainCanvas_MouseLeave;
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
                // Update all visual representations of circles
                UpdateCircleVisualization(); // This will update both single circle view and composition view
                UpdateCircleStats();
                UpdatePreview();
                if (statusText != null)
                    statusText.Text = $"Added {elementType} talisman to circle (Total: {_currentCircle.Talismans.Count})";
            }
            else
            {
                if (statusText != null)
                    statusText.Text = "Circle is full - cannot add more talismans";
            }
        }
    }

    private void NewCircle_Click(object sender, RoutedEventArgs e)
    {
        CreateNewCircle();
    }

    private void CreateNewCircle()
    {
        var slider = FindName("CircleSizeSlider") as Slider;
        var radius = slider?.Value ?? 5.0;
        
        _currentCircle = new MagicCircle("New Circle", radius);
        _selectedCircle = _currentCircle; // Select the new circle by default
        
        // Clear existing circles and add the new one
        _allCircles.Clear();
        _allCircles.Add(_currentCircle);
        
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
        UpdatePreview(); // Add preview update
        UpdateCircleCapacityLabel();
        UpdateCompositionList();
        if (statusText != null)
            statusText.Text = "Created new magic circle";
    }

    private void LoadConfiguration_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Load Spell Configuration",
            Filter = "Spell Images (*.png)|*.png|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                // Check if the file contains spell data
                if (!ImageSpellStorage.IsSpellImage(openFileDialog.FileName))
                {
                    MessageBox.Show("The selected image does not contain spell data.", "Invalid Spell Image", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Load the spell from image metadata
                var loadedCircle = ImageSpellStorage.LoadSpellFromImage(openFileDialog.FileName);
                if (loadedCircle != null)
                {
                    _currentCircle = loadedCircle;
                    var circleViewer = FindName("CircleViewer") as MagicCircleControl;
                    if (circleViewer != null)
                    {
                        circleViewer.MagicCircle = null;
                        circleViewer.MagicCircle = _currentCircle;
                        circleViewer.InvalidateVisual();
                        circleViewer.UpdateLayout();
                    }
                    UpdateCircleStats();
                    UpdatePreview(); // Add preview update
                    UpdateCircleCapacityLabel();

                    var statusText = FindName("StatusText") as TextBlock;
                    if (statusText != null)
                        statusText.Text = $"Successfully loaded spell from {System.IO.Path.GetFileName(openFileDialog.FileName)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load spell: {ex.Message}", "Load Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                var statusText = FindName("StatusText") as TextBlock;
                if (statusText != null)
                    statusText.Text = "Failed to load spell configuration";
            }
        }
    }

    private void SaveConfiguration_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null)
        {
            MessageBox.Show("No spell to save. Please create a spell first.", "No Spell", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var saveFileDialog = new SaveFileDialog
        {
            Title = "Save Spell Configuration",
            Filter = "Spell Images (*.png)|*.png",
            FilterIndex = 1,
            RestoreDirectory = true,
            FileName = $"{_currentCircle.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                // Save the current circle as an image with programmatic rendering
                ImageSpellStorage.SaveSpellAsImage(_currentCircle, saveFileDialog.FileName);

                var statusText = FindName("StatusText") as TextBlock;
                if (statusText != null)
                    statusText.Text = $"Successfully saved spell to {System.IO.Path.GetFileName(saveFileDialog.FileName)}";

                MessageBox.Show($"Spell saved successfully!\n\nThe spell data has been saved with the image. " +
                    $"You can share this image and others can load the exact spell configuration from it.", 
                    "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save spell: {ex.Message}", "Save Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                var statusText = FindName("StatusText") as TextBlock;
                if (statusText != null)
                    statusText.Text = "Failed to save spell configuration";
            }
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
        {
            statusText.Text = "Wu Lang Spellcraft Renderer - A visual programming language inspired by the Chinese Wu Xing (Five Elements) system";
        }
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
            UpdatePreview(); // Add preview update
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

        // Clear the circle without confirmation
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
        UpdatePreview(); // Add preview update
        if (statusText != null)
            statusText.Text = "Cleared circle";
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
        UpdatePreview(); // Add preview update
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

    private void CircleSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is not Slider slider) return;
        
        var sizeLabel = FindName("CircleSizeLabel") as TextBlock;
        if (sizeLabel != null)
            sizeLabel.Text = slider.Value.ToString("F1");
        
        UpdateCircleCapacityLabel();
        
        // Update existing circle if one exists
        if (_currentCircle != null)
        {
            _currentCircle.Radius = slider.Value;
            var circleViewer = FindName("CircleViewer") as MagicCircleControl;
            if (circleViewer != null)
            {
                circleViewer.MagicCircle = null;
                circleViewer.MagicCircle = _currentCircle;
                circleViewer.InvalidateVisual();
                circleViewer.UpdateLayout();
            }
            UpdateCircleStats();
            UpdatePreview(); // Add preview update
        }
    }

    private void UpdateCircleCapacityLabel()
    {
        var capacityLabel = FindName("CircleCapacityLabel") as TextBlock;
        var slider = FindName("CircleSizeSlider") as Slider;
        if (capacityLabel == null || slider == null) return;
        
        var radius = slider.Value;
        var circumference = 2 * Math.PI * radius;
        var minSpacing = 2.0;
        var maxTalismans = Math.Max(3, (int)(circumference / minSpacing));
        
        capacityLabel.Text = $"Max Talismans: {maxTalismans}";
    }

    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is not Slider slider) return;
        
        var zoomLabel = FindName("ZoomLabel") as TextBlock;
        var viewbox = FindName("CanvasViewbox") as Viewbox;
        
        if (zoomLabel != null)
            zoomLabel.Text = $"{slider.Value * 100:F0}%";
        
        if (viewbox != null)
        {
            var scaleTransform = new ScaleTransform(slider.Value, slider.Value);
            viewbox.RenderTransform = scaleTransform;
        }
    }

    private void ResetView_Click(object sender, RoutedEventArgs e)
    {
        var zoomSlider = FindName("ZoomSlider") as Slider;
        var scrollViewer = FindName("CanvasScrollViewer") as ScrollViewer;
        
        if (zoomSlider != null)
            zoomSlider.Value = 1.0;
        
        if (scrollViewer != null)
        {
            scrollViewer.ScrollToVerticalOffset(0);
            scrollViewer.ScrollToHorizontalOffset(0);
        }
    }

    #region Composition Methods

    private void NestedSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is not Slider slider) return;
        
        var sizeLabel = FindName("NestedSizeLabel") as TextBlock;
        if (sizeLabel != null)
            sizeLabel.Text = slider.Value.ToString("F1");
    }

    private void ConnectionStrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is not Slider slider) return;
        
        var strengthLabel = FindName("ConnectionStrengthLabel") as TextBlock;
        if (strengthLabel != null)
            strengthLabel.Text = slider.Value.ToString("F1");
    }

    private void AddNestedCircle_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null) return;

        var nestedSizeSlider = FindName("NestedSizeSlider") as Slider;
        var nestedRadius = nestedSizeSlider?.Value ?? 3.0;
        
        // Create a nested circle
        var nestedCircle = new MagicCircle($"Nested_{_allCircles.Count + 1}", nestedRadius);
        nestedCircle.NestedScale = 0.7; // Scale it down when nested
        
        // Nest the circle using the composition system
        _currentCircle.NestCircle(nestedCircle);
        _allCircles.Add(nestedCircle);
        
        // Update the visualization
        UpdateCircleVisualization();
        UpdateCompositionList();
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = $"Added nested circle (Radius: {nestedRadius:F1}) to {_currentCircle.Name}";
    }

    private void CreateConnectedCircle_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null) return;

        var connectionTypeCombo = FindName("ConnectionTypeCombo") as ComboBox;
        var connectionStrengthSlider = FindName("ConnectionStrengthSlider") as Slider;
        
        var selectedItem = connectionTypeCombo?.SelectedItem as ComboBoxItem;
        var connectionTypeString = selectedItem?.Tag?.ToString() ?? "Direct";
        var connectionStrength = connectionStrengthSlider?.Value ?? 1.0;
        
        if (!Enum.TryParse<ConnectionType>(connectionTypeString, out var connectionType))
        {
            connectionType = ConnectionType.Direct;
        }

        // Create a connected circle
        var connectedCircle = new MagicCircle($"Connected_{_allCircles.Count + 1}", 4.0);
        
        // Position it relative to the current circle (for visualization)
        connectedCircle.CenterX = _currentCircle.CenterX + (_currentCircle.Radius + connectedCircle.Radius + 2) * Math.Cos(_allCircles.Count * Math.PI / 3);
        connectedCircle.CenterY = _currentCircle.CenterY + (_currentCircle.Radius + connectedCircle.Radius + 2) * Math.Sin(_allCircles.Count * Math.PI / 3);
        
        // Connect the circles using the composition system
        var connection = _currentCircle.ConnectTo(connectedCircle, connectionType);
        connection.Strength = connectionStrength; // Set custom strength
        _allCircles.Add(connectedCircle);
        
        // Update the visualization
        UpdateCircleVisualization();
        UpdateCompositionList();
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = $"Created connected circle with {connectionType} connection (Strength: {connectionStrength:F1})";
    }

    private void SelectCircle_Click(object sender, RoutedEventArgs e)
    {
        var compositionList = FindName("CompositionList") as ListBox;
        if (compositionList?.SelectedItem is MagicCircle selectedCircle)
        {
            _selectedCircle = selectedCircle;
            _currentCircle = selectedCircle; // Switch to working on this circle
            
            UpdateCircleVisualization();
            
            var statusText = FindName("StatusText") as TextBlock;
            if (statusText != null)
                statusText.Text = $"Selected circle: {selectedCircle.Name}";
        }
    }

    private void RemoveCircle_Click(object sender, RoutedEventArgs e)
    {
        var compositionList = FindName("CompositionList") as ListBox;
        if (compositionList?.SelectedItem is MagicCircle circleToRemove && circleToRemove != _allCircles[0])
        {
            // Remove from parent circle if nested
            foreach (var circle in _allCircles)
            {
                if (circle.NestedCircles.Contains(circleToRemove))
                {
                    circle.NestedCircles.Remove(circleToRemove);
                    break;
                }
            }
            
            // Remove connections involving this circle
            foreach (var circle in _allCircles)
            {
                circle.Connections.RemoveAll(conn => conn.Source == circleToRemove || conn.Target == circleToRemove);
            }
            
            _allCircles.Remove(circleToRemove);
            
            // If we removed the current circle, switch to the main circle
            if (_currentCircle == circleToRemove)
            {
                _currentCircle = _allCircles.FirstOrDefault();
                _selectedCircle = _currentCircle;
            }
            
            UpdateCircleVisualization();
            UpdateCompositionList();
            
            var statusText = FindName("StatusText") as TextBlock;
            if (statusText != null)
                statusText.Text = $"Removed circle: {circleToRemove.Name}";
        }
    }

    private void UpdateCompositionList()
    {
        var compositionList = FindName("CompositionList") as ListBox;
        if (compositionList == null) return;

        compositionList.Items.Clear();
        
        foreach (var circle in _allCircles)
        {
            var displayName = circle.Name;
            if (circle == _currentCircle)
                displayName += " (Current)";
            if (circle == _selectedCircle)
                displayName += " (Selected)";
                
            var item = new ListBoxItem
            {
                Content = displayName,
                Tag = circle,
                Foreground = Brushes.White
            };
            
            compositionList.Items.Add(circle);
        }
    }

    private void UpdateCircleVisualization()
    {
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        var mainCanvas = FindName("MainCanvas") as Canvas;
        var viewModeText = FindName("ViewModeText") as TextBlock;
        
        if (mainCanvas == null) return;

        // Clear existing circles
        var existingCircles = mainCanvas.Children.OfType<MagicCircleControl>().ToList();
        foreach (var existing in existingCircles)
        {
            mainCanvas.Children.Remove(existing);
        }

        // If we have multiple circles (composition), render them all
        if (_allCircles.Count > 1)
        {
            if (viewModeText != null)
                viewModeText.Text = "View: Combination Mode";
            RenderCompositionView(mainCanvas);
        }
        // Otherwise, render single circle in the main viewer
        else if (circleViewer != null && _currentCircle != null)
        {
            if (viewModeText != null)
                viewModeText.Text = "View: Single Circle";
            // Force the control to update
            circleViewer.MagicCircle = null;
            circleViewer.MagicCircle = _currentCircle;
            circleViewer.IsSelected = _selectedCircle == _currentCircle; // Set selection state
            circleViewer.InvalidateVisual();
            circleViewer.UpdateLayout();
            
            // Ensure the main viewer is visible
            circleViewer.Visibility = Visibility.Visible;
        }
        
        UpdateCircleStats();
        UpdatePreview();
    }

    private void RenderCompositionView(Canvas mainCanvas)
    {
        if (!_allCircles.Any()) return;

        // Clear any existing connection lines
        ClearConnectionLines(mainCanvas);

        // Hide the main circle viewer when showing composition
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        if (circleViewer != null)
        {
            circleViewer.Visibility = Visibility.Hidden;
        }

        var canvasWidth = mainCanvas.Width;
        var canvasHeight = mainCanvas.Height;
        var centerX = canvasWidth / 2;
        var centerY = canvasHeight / 2;

        // Main circle at center
        var mainCircle = _allCircles[0];
        var mainCircleControl = CreateDraggableCircleControl(mainCircle, 400, 400);

        Canvas.SetLeft(mainCircleControl, centerX - 200);
        Canvas.SetTop(mainCircleControl, centerY - 200);
        mainCanvas.Children.Add(mainCircleControl);

        // Connected circles positioned around the main circle
        var connectedCircles = _allCircles.Skip(1).Where(c => !IsNestedCircle(c)).ToList();
        
        for (int i = 0; i < connectedCircles.Count; i++)
        {
            var connectedCircle = connectedCircles[i];
            var angle = i * 2 * Math.PI / connectedCircles.Count;
            var distance = 350; // Distance from center
            var x = centerX + Math.Cos(angle) * distance;
            var y = centerY + Math.Sin(angle) * distance;

            var connectedControl = CreateDraggableCircleControl(connectedCircle, 250, 250);

            Canvas.SetLeft(connectedControl, x - 125);
            Canvas.SetTop(connectedControl, y - 125);
            mainCanvas.Children.Add(connectedControl);
        }

        // Draw all connection lines
        RedrawConnectionLines(mainCanvas);
    }

    private MagicCircleControl CreateDraggableCircleControl(MagicCircle circle, double width, double height)
    {
        var control = new MagicCircleControl
        {
            MagicCircle = circle,
            Width = width,
            Height = height,
            ShowConnections = false, // Don't show individual connections, we'll draw them globally
            ShowEffects = true,
            IsSelected = _selectedCircle == circle, // Set initial selection state
            Tag = circle // Store reference for easy identification
        };

        // Use the Selected event from MagicCircleControl for cleaner separation of selection and dragging
        control.Selected += (s, e) => {
            if (s is MagicCircleControl clickedControl && clickedControl.MagicCircle != null)
            {
                SelectCircle(clickedControl.MagicCircle);
            }
        };

        // Wire up talisman removal event
        control.TalismanRemoved += OnTalismanRemoved;

        // Add real-time position change handler for smooth connection updates
        control.PositionChanging += (s, e) => {
            if (s is MagicCircleControl draggedControl)
            {
                var canvas = FindParentCanvas(draggedControl);
                if (canvas != null)
                {
                    RedrawConnectionLines(canvas);
                }
            }
        };

        // Add final position change handler
        control.PositionChanged += (s, e) => {
            if (s is MagicCircleControl draggedControl)
            {
                var canvas = FindParentCanvas(draggedControl);
                if (canvas != null)
                {
                    RedrawConnectionLines(canvas);
                }
            }
        };

        return control;
    }

    private void SelectCircle(MagicCircle circle)
    {
        // Update the selected circle
        _selectedCircle = circle;
        _currentCircle = circle; // Also update the current working circle
        
        // Update all circle controls to reflect selection state
        var mainCanvas = FindName("MainCanvas") as Canvas;
        if (mainCanvas != null)
        {
            var circleControls = mainCanvas.Children.OfType<MagicCircleControl>().ToList();
            foreach (var control in circleControls)
            {
                control.IsSelected = control.MagicCircle == _selectedCircle;
            }
        }
        
        // Update the main circle viewer to show the selected circle
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        if (circleViewer != null)
        {
            circleViewer.MagicCircle = _selectedCircle;
            circleViewer.IsSelected = true;
            circleViewer.Visibility = Visibility.Visible;
        }
        
        // Update UI elements
        UpdateCircleStats();
        UpdatePreview();
        UpdateCompositionList();
        
        // Update status
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = $"Selected circle: {circle.Name} (Layer: {circle.Layer})";
    }

    private void ClearConnectionLines(Canvas canvas)
    {
        // Remove existing connection lines (identified by their tag)
        var linesToRemove = canvas.Children.OfType<Line>()
            .Where(l => l.Tag?.ToString() == "ConnectionLine")
            .ToList();
        
        foreach (var line in linesToRemove)
        {
            canvas.Children.Remove(line);
        }

        // Remove connection labels
        var labelsToRemove = canvas.Children.OfType<TextBlock>()
            .Where(t => t.Tag?.ToString() == "ConnectionLabel")
            .ToList();
            
        foreach (var label in labelsToRemove)
        {
            canvas.Children.Remove(label);
        }
    }

    private void RedrawConnectionLines(Canvas canvas)
    {
        ClearConnectionLines(canvas);

        var circleControls = canvas.Children.OfType<MagicCircleControl>().ToList();
        
        foreach (var sourceControl in circleControls)
        {
            if (sourceControl.MagicCircle?.Connections == null) continue;

            var sourceCenter = GetCircleControlCenter(sourceControl);

            foreach (var connection in sourceControl.MagicCircle.Connections)
            {
                var targetControl = circleControls.FirstOrDefault(c => c.MagicCircle == connection.Target);
                if (targetControl != null)
                {
                    var targetCenter = GetCircleControlCenter(targetControl);
                    DrawConnectionLine(canvas, sourceCenter.X, sourceCenter.Y, targetCenter.X, targetCenter.Y, connection);
                }
            }
        }
    }

    private Point GetCircleControlCenter(MagicCircleControl control)
    {
        var left = Canvas.GetLeft(control);
        var top = Canvas.GetTop(control);
        
        if (double.IsNaN(left)) left = 0;
        if (double.IsNaN(top)) top = 0;
        
        return new Point(left + control.Width / 2, top + control.Height / 2);
    }

    private Canvas? FindParentCanvas(FrameworkElement element)
    {
        var parent = element.Parent;
        while (parent != null)
        {
            if (parent is Canvas canvas)
                return canvas;
            
            parent = parent is FrameworkElement fe ? fe.Parent : null;
        }
        return null;
    }

    private bool IsNestedCircle(MagicCircle circle)
    {
        return _allCircles.Any(c => c.NestedCircles.Contains(circle));
    }

    private CircleConnection? GetConnectionBetween(MagicCircle from, MagicCircle to)
    {
        return from.Connections?.FirstOrDefault(conn => conn.Target == to);
    }

    private void DrawConnectionLine(Canvas canvas, double x1, double y1, double x2, double y2, CircleConnection? connection)
    {
        var line = new Line
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = GetConnectionBrush(connection),
            StrokeThickness = GetConnectionThickness(connection),
            Opacity = 0.8,
            Tag = "ConnectionLine" // Tag for easy identification and removal
        };

        canvas.Children.Add(line);

        // Add connection type label
        if (connection != null)
        {
            var midX = (x1 + x2) / 2;
            var midY = (y1 + y2) / 2;
            
            var label = new TextBlock
            {
                Text = $"{connection.Type}\n{connection.Strength:F1}",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)),
                Padding = new Thickness(3),
                Tag = "ConnectionLabel" // Tag for easy identification and removal
            };

            Canvas.SetLeft(label, midX - 20);
            Canvas.SetTop(label, midY - 10);
            canvas.Children.Add(label);
        }
    }

    private Brush GetConnectionBrush(CircleConnection? connection)
    {
        if (connection == null) return Brushes.Gray;

        return connection.Type switch
        {
            ConnectionType.Direct => Brushes.Yellow,
            ConnectionType.Resonance => Brushes.Purple,
            ConnectionType.Flow => Brushes.Cyan,
            ConnectionType.Trigger => Brushes.Orange,
            _ => Brushes.Gray
        };
    }

    private double GetConnectionThickness(CircleConnection? connection)
    {
        if (connection == null) return 2;
        return Math.Max(2, connection.Strength * 4);
    }

    #endregion

    #region Preview Panel Methods

    private void UpdatePreview()
    {
        if (_currentCircle == null)
        {
            ClearPreview();
            return;
        }

        // Update the visual preview circle
        UpdatePreviewVisual();
        
        UpdateBasicInfo();
        UpdatePowerAnalysis();
        UpdateElementBalance();
        UpdateEffectsList();
        UpdateCompositionDetails();
        UpdatePerformanceMetrics();
    }

    private void UpdatePreviewVisual()
    {
        var circleViewer = FindName("CircleViewer") as MagicCircleControl;
        if (circleViewer != null && _currentCircle != null)
        {
            // Method 1: Use the RefreshTrigger property to force a change
            circleViewer.RefreshTrigger = DateTime.Now;
            
            // Method 2: Force refresh using the ForceRefresh method
            circleViewer.ForceRefresh();
            
            // Method 3: Also try the null assignment approach as backup
            circleViewer.MagicCircle = null;
            circleViewer.UpdateLayout();
            
            // Re-assign the circle
            circleViewer.MagicCircle = _currentCircle;
            circleViewer.InvalidateVisual();
            circleViewer.UpdateLayout();
        }
    }

    private void ClearPreview()
    {
        // Reset all preview fields to default values
        var previewRadius = FindName("PreviewRadius") as TextBlock;
        var previewTalismanCount = FindName("PreviewTalismanCount") as TextBlock;
        var previewCompositionType = FindName("PreviewCompositionType") as TextBlock;
        var previewBasePower = FindName("PreviewBasePower") as TextBlock;
        var previewComplexity = FindName("PreviewComplexity") as TextBlock;
        var previewFinalPower = FindName("PreviewFinalPower") as TextBlock;
        var previewCastingTime = FindName("PreviewCastingTime") as TextBlock;
        var previewEfficiency = FindName("PreviewEfficiency") as TextBlock;
        var previewStability = FindName("PreviewStability") as TextBlock;
        var previewRiskLevel = FindName("PreviewRiskLevel") as TextBlock;

        if (previewRadius != null) previewRadius.Text = "0.0";
        if (previewTalismanCount != null) previewTalismanCount.Text = "0/0";
        if (previewCompositionType != null) previewCompositionType.Text = "None";
        if (previewBasePower != null) previewBasePower.Text = "0";
        if (previewComplexity != null) previewComplexity.Text = "1.0";
        if (previewFinalPower != null) previewFinalPower.Text = "0";
        if (previewCastingTime != null) previewCastingTime.Text = "0.0s";
        if (previewEfficiency != null) previewEfficiency.Text = "100%";
        if (previewStability != null) previewStability.Text = "Unknown";
        if (previewRiskLevel != null) previewRiskLevel.Text = "Unknown";
    }

    private void UpdateBasicInfo()
    {
        var previewRadius = FindName("PreviewRadius") as TextBlock;
        var previewTalismanCount = FindName("PreviewTalismanCount") as TextBlock;
        var previewCompositionType = FindName("PreviewCompositionType") as TextBlock;

        if (previewRadius != null)
            previewRadius.Text = _currentCircle!.Radius.ToString("F1");

        if (previewTalismanCount != null)
        {
            var maxTalismans = CalculateMaxTalismans(_currentCircle!.Radius);
            previewTalismanCount.Text = $"{_currentCircle.Talismans.Count}/{maxTalismans}";
        }

        if (previewCompositionType != null)
        {
            var compositionType = _currentCircle!.CompositionType;
            previewCompositionType.Text = compositionType.ToString();
        }
    }

    private void UpdatePowerAnalysis()
    {
        var previewBasePower = FindName("PreviewBasePower") as TextBlock;
        var previewComplexity = FindName("PreviewComplexity") as TextBlock;
        var previewFinalPower = FindName("PreviewFinalPower") as TextBlock;
        var previewCastingTime = FindName("PreviewCastingTime") as TextBlock;

        if (previewBasePower != null)
            previewBasePower.Text = _currentCircle!.PowerOutput.ToString("F1");

        if (previewComplexity != null)
        {
            var complexity = _currentCircle!.CalculateComplexityScore();
            previewComplexity.Text = complexity.ToString("F2");
        }

        if (previewFinalPower != null)
        {
            var finalPower = _currentCircle!.GetCompositionPowerOutput();
            previewFinalPower.Text = finalPower.ToString("F1");
        }

        if (previewCastingTime != null)
        {
            var castingTime = _currentCircle!.CalculateCastingTime();
            previewCastingTime.Text = $"{castingTime:F1}s";
        }
    }

    private void UpdateElementBalance()
    {
        var elementBalance = FindName("PreviewElementBalance") as StackPanel;
        if (elementBalance == null) return;

        elementBalance.Children.Clear();

        var elementCounts = new Dictionary<ElementType, int>();
        foreach (ElementType element in Enum.GetValues<ElementType>())
        {
            elementCounts[element] = 0;
        }

        // Count elements from talismans
        foreach (var talisman in _currentCircle!.Talismans)
        {
            elementCounts[talisman.PrimaryElement.Type]++;
            foreach (var secondary in talisman.SecondaryElements)
            {
                elementCounts[secondary.Type]++;
            }
        }

        // Create visual representation
        foreach (var kvp in elementCounts)
        {
            if (kvp.Value > 0)
            {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var elementText = new TextBlock
                {
                    Text = $"{kvp.Key}:",
                    Foreground = Brushes.LightGray,
                    Margin = new Thickness(0, 2, 5, 2)
                };
                Grid.SetColumn(elementText, 0);

                var countText = new TextBlock
                {
                    Text = kvp.Value.ToString(),
                    Foreground = GetElementColor(kvp.Key),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                Grid.SetColumn(countText, 1);

                grid.Children.Add(elementText);
                grid.Children.Add(countText);
                elementBalance.Children.Add(grid);
            }
        }

        if (elementBalance.Children.Count == 0)
        {
            var noElements = new TextBlock
            {
                Text = "No elements present",
                Foreground = Brushes.LightGray,
                FontStyle = FontStyles.Italic
            };
            elementBalance.Children.Add(noElements);
        }
    }

    private void UpdateEffectsList()
    {
        var effectsList = FindName("PreviewEffectsList") as StackPanel;
        if (effectsList == null) return;

        effectsList.Children.Clear();

        var spellEffect = _currentCircle!.CalculateSpellEffect();
        
        var effectText = new TextBlock
        {
            Text = $"{spellEffect.Type} (Power: {spellEffect.Power:F1})",
            Foreground = Brushes.White,
            Margin = new Thickness(0, 2, 0, 2)
        };
        effectsList.Children.Add(effectText);

        // Add composition effects if any
        if (_currentCircle.CompositionType != CompositionType.Simple)
        {
            var compositionEffect = new TextBlock
            {
                Text = $"Composition Bonus: +{(_currentCircle.GetCompositionPowerOutput() - _currentCircle.PowerOutput):F1}",
                Foreground = Brushes.Cyan,
                Margin = new Thickness(0, 2, 0, 2)
            };
            effectsList.Children.Add(compositionEffect);
        }
    }

    private void UpdateCompositionDetails()
    {
        var compositionDetails = FindName("PreviewCompositionDetails") as StackPanel;
        if (compositionDetails == null) return;

        compositionDetails.Children.Clear();

        var compositionType = _currentCircle!.CompositionType;
        
        switch (compositionType)
        {
            case CompositionType.Simple:
                var simpleText = new TextBlock
                {
                    Text = "Simple circle - no composition",
                    Foreground = Brushes.LightGray,
                    FontStyle = FontStyles.Italic
                };
                compositionDetails.Children.Add(simpleText);
                break;

            case CompositionType.Stacked:
                var stackedText = new TextBlock
                {
                    Text = $"Stacked composition with {_currentCircle.NestedCircles.Count} nested circles",
                    Foreground = Brushes.Yellow
                };
                compositionDetails.Children.Add(stackedText);
                break;

            case CompositionType.Network:
                var connectedText = new TextBlock
                {
                    Text = $"Network composition with {_currentCircle.Connections.Count} connections",
                    Foreground = Brushes.Orange
                };
                compositionDetails.Children.Add(connectedText);
                break;

            case CompositionType.Unified:
                var unifiedText = new TextBlock
                {
                    Text = $"Unified composition: {_currentCircle.NestedCircles.Count} nested + {_currentCircle.Connections.Count} connections",
                    Foreground = Brushes.Magenta
                };
                compositionDetails.Children.Add(unifiedText);
                break;

            default:
                var defaultText = new TextBlock
                {
                    Text = $"{compositionType} composition",
                    Foreground = Brushes.White
                };
                compositionDetails.Children.Add(defaultText);
                break;
        }
    }

    private void UpdatePerformanceMetrics()
    {
        var previewEfficiency = FindName("PreviewEfficiency") as TextBlock;
        var previewStability = FindName("PreviewStability") as TextBlock;
        var previewRiskLevel = FindName("PreviewRiskLevel") as TextBlock;

        if (previewEfficiency != null)
        {
            var efficiency = CalculateEfficiency();
            previewEfficiency.Text = $"{efficiency:F0}%";
        }

        if (previewStability != null)
        {
            var stability = _currentCircle!.Stability;
            string stabilityText = stability switch
            {
                >= 0.8 => "High",
                >= 0.6 => "Medium",
                >= 0.4 => "Low",
                _ => "Critical"
            };
            previewStability.Text = stabilityText;
            previewStability.Foreground = stability >= 0.6 ? Brushes.Green : 
                                          stability >= 0.4 ? Brushes.Yellow : Brushes.Red;
        }

        if (previewRiskLevel != null)
        {
            var risk = CalculateRiskLevel();
            previewRiskLevel.Text = risk;
            previewRiskLevel.Foreground = risk == "Low" ? Brushes.Green :
                                          risk == "Medium" ? Brushes.Yellow : Brushes.Red;
        }
    }

    private double CalculateEfficiency()
    {
        if (_currentCircle!.Talismans.Count == 0) return 100.0;
        
        var maxCapacity = CalculateMaxTalismans(_currentCircle.Radius);
        var utilization = (double)_currentCircle.Talismans.Count / maxCapacity;
        var complexityPenalty = Math.Max(0, _currentCircle.CalculateComplexityScore() - 1.0) * 10;
        
        return Math.Max(0, Math.Min(100, (utilization * 100) - complexityPenalty));
    }

    private string CalculateRiskLevel()
    {
        var complexity = _currentCircle!.CalculateComplexityScore();
        var stability = _currentCircle.Stability;
        
        if (complexity > 3.0 || stability < 0.4) return "High";
        if (complexity > 2.0 || stability < 0.6) return "Medium";
        return "Low";
    }

    private int CalculateMaxTalismans(double radius)
    {
        var circumference = 2 * Math.PI * radius;
        var minSpacing = 2.0;
        return Math.Max(3, (int)(circumference / minSpacing));
    }

    private Brush GetElementColor(ElementType elementType)
    {
        return elementType switch
        {
            ElementType.Water => Brushes.CornflowerBlue,
            ElementType.Fire => Brushes.OrangeRed,
            ElementType.Earth => Brushes.SandyBrown,
            ElementType.Metal => Brushes.Silver,
            ElementType.Wood => Brushes.ForestGreen,
            ElementType.Lightning => Brushes.Yellow,
            ElementType.Wind => Brushes.LightCyan,
            ElementType.Light => Brushes.LightYellow,
            ElementType.Dark => Brushes.DarkGray,
            ElementType.Forge => Brushes.DarkRed,
            ElementType.Chaos => Brushes.Magenta,
            ElementType.Void => Brushes.DarkViolet,
            _ => Brushes.White
        };
    }

    // Preview Panel Event Handlers
    private void SimulateCasting_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null)
        {
            MessageBox.Show("No circle to simulate. Please create a circle first.", "No Circle", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var effect = _currentCircle.CalculateSpellEffect();
        var castingTime = _currentCircle.CalculateCastingTime();
        var complexity = _currentCircle.CalculateComplexityScore();
        var stability = _currentCircle.Stability;

        var result = $"Casting Simulation Results:\n\n" +
                    $"Spell Effect: {effect.Type}\n" +
                    $"Power Output: {effect.Power:F1}\n" +
                    $"Casting Time: {castingTime:F1} seconds\n" +
                    $"Complexity Score: {complexity:F2}\n" +
                    $"Stability: {stability:F2}\n\n" +
                    $"Success Probability: {(stability * 100):F0}%\n" +
                    $"Risk Assessment: {CalculateRiskLevel()}";

        MessageBox.Show(result, "Casting Simulation", MessageBoxButton.OK, MessageBoxImage.Information);

        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = $"Simulated casting - {effect.Type} with {effect.Power:F1} power";
    }

    private void ExportAnalysis_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null)
        {
            MessageBox.Show("No circle to analyze. Please create a circle first.", "No Circle", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var saveFileDialog = new SaveFileDialog
        {
            Title = "Export Circle Analysis",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true,
            FileName = $"{_currentCircle.Name}_Analysis_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                var analysis = GenerateDetailedAnalysis();
                File.WriteAllText(saveFileDialog.FileName, analysis);

                var statusText = FindName("StatusText") as TextBlock;
                if (statusText != null)
                    statusText.Text = $"Exported analysis to {System.IO.Path.GetFileName(saveFileDialog.FileName)}";

                MessageBox.Show("Analysis exported successfully!", "Export Complete", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export analysis: {ex.Message}", "Export Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void GenerateRecommendations_Click(object sender, RoutedEventArgs e)
    {
        if (_currentCircle == null)
        {
            MessageBox.Show("No circle to analyze. Please create a circle first.", "No Circle", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var recommendations = GenerateCircleRecommendations();
        MessageBox.Show(recommendations, "Circle Recommendations", MessageBoxButton.OK, MessageBoxImage.Information);

        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = "Generated optimization recommendations";
    }

    private string GenerateDetailedAnalysis()
    {
        var effect = _currentCircle!.CalculateSpellEffect();
        var complexity = _currentCircle.CalculateComplexityScore();
        var castingTime = _currentCircle.CalculateCastingTime();
        var compositionType = _currentCircle.CompositionType;

        var analysis = $"Wu Lang Spellcraft Circle Analysis\n";
        analysis += $"Generated: {DateTime.Now}\n";
        analysis += $"========================================\n\n";
        
        analysis += $"CIRCLE INFORMATION:\n";
        analysis += $"Name: {_currentCircle.Name}\n";
        analysis += $"Radius: {_currentCircle.Radius:F1}\n";
        analysis += $"Talisman Count: {_currentCircle.Talismans.Count}\n";
        analysis += $"Composition Type: {compositionType}\n\n";
        
        analysis += $"POWER ANALYSIS:\n";
        analysis += $"Base Power: {_currentCircle.PowerOutput:F1}\n";
        analysis += $"Complexity Score: {complexity:F2}\n";
        analysis += $"Final Power Output: {_currentCircle.GetCompositionPowerOutput():F1}\n";
        analysis += $"Casting Time: {castingTime:F1} seconds\n\n";
        
        analysis += $"SPELL EFFECT:\n";
        analysis += $"Type: {effect.Type}\n";
        analysis += $"Power: {effect.Power:F1}\n\n";
        
        analysis += $"PERFORMANCE METRICS:\n";
        analysis += $"Efficiency: {CalculateEfficiency():F1}%\n";
        analysis += $"Stability: {_currentCircle.Stability:F2}\n";
        analysis += $"Risk Level: {CalculateRiskLevel()}\n\n";

        // Element breakdown
        analysis += $"ELEMENT DISTRIBUTION:\n";
        var elementCounts = new Dictionary<ElementType, int>();
        foreach (ElementType element in Enum.GetValues<ElementType>())
        {
            elementCounts[element] = 0;
        }

        foreach (var talisman in _currentCircle.Talismans)
        {
            elementCounts[talisman.PrimaryElement.Type]++;
            foreach (var secondary in talisman.SecondaryElements)
            {
                elementCounts[secondary.Type]++;
            }
        }

        foreach (var kvp in elementCounts.Where(x => x.Value > 0))
        {
            analysis += $"{kvp.Key}: {kvp.Value}\n";
        }

        analysis += $"\n";
        analysis += GenerateCircleRecommendations();

        return analysis;
    }

    private string GenerateCircleRecommendations()
    {
        var recommendations = "OPTIMIZATION RECOMMENDATIONS:\n\n";
        
        var efficiency = CalculateEfficiency();
        var stability = _currentCircle!.Stability;
        var complexity = _currentCircle.CalculateComplexityScore();
        var maxTalismans = CalculateMaxTalismans(_currentCircle.Radius);

        if (efficiency < 70)
        {
            recommendations += "• Consider adding more talismans to improve circle utilization\n";
        }

        if (stability < 0.6)
        {
            recommendations += "• Circle stability is low - consider balancing element distribution\n";
        }

        if (complexity > 2.5)
        {
            recommendations += "• High complexity detected - simplify composition for better casting reliability\n";
        }

        if (_currentCircle.Talismans.Count < maxTalismans / 2)
        {
            recommendations += "• Circle is underutilized - you can add more talismans for increased power\n";
        }

        // Element balance recommendations
        var elementCounts = new Dictionary<ElementType, int>();
        foreach (ElementType element in Enum.GetValues<ElementType>())
        {
            elementCounts[element] = 0;
        }

        foreach (var talisman in _currentCircle.Talismans)
        {
            elementCounts[talisman.PrimaryElement.Type]++;
        }

        var dominantElement = elementCounts.Where(x => x.Value > 0).OrderByDescending(x => x.Value).FirstOrDefault();
        if (dominantElement.Value > _currentCircle.Talismans.Count * 0.6)
        {
            recommendations += $"• Element imbalance detected - {dominantElement.Key} is dominant. Consider adding other elements\n";
        }

        if (recommendations == "OPTIMIZATION RECOMMENDATIONS:\n\n")
        {
            recommendations += "• Circle is well-optimized! No major issues detected.\n";
        }

        return recommendations;
    }

    #endregion

    #region Example Loaders

    private void LoadDefensiveExample_Click(object sender, RoutedEventArgs e)
    {
        LoadDefensiveLayeredExample();
    }

    private void LoadElementalNetworkExample_Click(object sender, RoutedEventArgs e)
    {
        LoadElementalNetworkExample();
    }

    private void LoadUnifiedExample_Click(object sender, RoutedEventArgs e)
    {
        LoadUnifiedCompositionExample();
    }

    private void LoadDefensiveLayeredExample()
    {
        var mainCircle = new MagicCircle("Defensive Fortress", 8.0);
        
        // Main circle - Earth-based defense
        var earthTalismans = new[]
        {
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Stone Barrier") { PowerLevel = 3.0 },
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Earthen Wall") { PowerLevel = 2.8 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Iron Shield") { PowerLevel = 3.2 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Steel Reinforcement") { PowerLevel = 2.9 }
        };
        
        foreach (var talisman in earthTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Nested circle - Metal reinforcement
        var nestedCircle = new MagicCircle("Metal Core", 4.0);
        var metalTalismans = new[]
        {
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Adamantine Core") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Metal) { IsActive = true }, "Reinforced Lattice") { PowerLevel = 3.5 },
            new Talisman(new Element(ElementType.Earth) { IsActive = true }, "Crystalline Support") { PowerLevel = 3.0 }
        };
        
        foreach (var talisman in metalTalismans)
        {
            nestedCircle.AddTalisman(talisman);
        }
        
        mainCircle.NestCircle(nestedCircle, 0.6);
        
        LoadCompositionExample(mainCircle, nestedCircle, "Loaded Defensive Layered composition example");
    }
    
    private void LoadElementalNetworkExample()
    {
        var mainCircle = new MagicCircle("Fire Storm", 6.0);
        
        // Main circle - Fire element
        var fireTalismans = new[]
        {
            new Talisman(new Element(ElementType.Fire) { IsActive = true }, "Inferno Core") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Fire) { IsActive = true }, "Flame Burst") { PowerLevel = 3.5 },
            new Talisman(new Element(ElementType.Light) { IsActive = true }, "Solar Flare") { PowerLevel = 3.8 }
        };
        
        foreach (var talisman in fireTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Wind amplifier circle
        var windCircle = new MagicCircle("Wind Amplifier", 4.0);
        windCircle.CenterX = 12.0; // Position it to the right
        var windTalismans = new[]
        {
            new Talisman(new Element(ElementType.Wind) { IsActive = true }, "Gale Force") { PowerLevel = 3.0 },
            new Talisman(new Element(ElementType.Wind) { IsActive = true }, "Cyclone Spin") { PowerLevel = 2.8 }
        };
        
        foreach (var talisman in windTalismans)
        {
            windCircle.AddTalisman(talisman);
        }
        
        // Lightning trigger circle
        var lightningCircle = new MagicCircle("Lightning Trigger", 3.5);
        lightningCircle.CenterX = -10.0; // Position it to the left
        lightningCircle.CenterY = 8.0;
        var lightningTalismans = new[]
        {
            new Talisman(new Element(ElementType.Lightning) { IsActive = true }, "Thunder Strike") { PowerLevel = 4.5 },
            new Talisman(new Element(ElementType.Lightning) { IsActive = true }, "Electric Arc") { PowerLevel = 3.2 }
        };
        
        foreach (var talisman in lightningTalismans)
        {
            lightningCircle.AddTalisman(talisman);
        }
        
        // Create resonance connection (Fire + Wind = enhanced combustion)
        var windConnection = mainCircle.ConnectTo(windCircle, ConnectionType.Resonance);
        windConnection.Strength = 1.8;
        
        // Create trigger connection (Lightning triggers the fire storm)
        var lightningConnection = mainCircle.ConnectTo(lightningCircle, ConnectionType.Trigger);
        lightningConnection.Strength = 1.5;
        
        LoadCompositionExample(mainCircle, windCircle, lightningCircle, "Loaded Elemental Network composition example");
    }
    
    private void LoadUnifiedCompositionExample()
    {
        var mainCircle = new MagicCircle("Unified Nexus", 10.0);
        
        // Main circle - Chaos element for maximum flexibility
        var chaosTalismans = new[]
        {
            new Talisman(new Element(ElementType.Chaos) { IsActive = true }, "Primordial Chaos") { PowerLevel = 5.0 },
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Void Anchor") { PowerLevel = 4.8 },
            new Talisman(new Element(ElementType.Chaos) { IsActive = true }, "Reality Flux") { PowerLevel = 4.5 }
        };
        
        foreach (var talisman in chaosTalismans)
        {
            mainCircle.AddTalisman(talisman);
        }
        
        // Nested stabilizing circle
        var stabilizingCircle = new MagicCircle("Void Core", 5.0);
        var voidTalismans = new[]
        {
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Balance Point") { PowerLevel = 4.0 },
            new Talisman(new Element(ElementType.Void) { IsActive = true }, "Null Field") { PowerLevel = 3.8 }
        };
        
        foreach (var talisman in voidTalismans)
        {
            stabilizingCircle.AddTalisman(talisman);
        }
        
        mainCircle.NestCircle(stabilizingCircle, 0.5);
        
        // Connected elemental support circles
        var elements = new[] { ElementType.Fire, ElementType.Water, ElementType.Earth };
        var connectionTypes = new[] { ConnectionType.Direct, ConnectionType.Resonance, ConnectionType.Flow };
        var supportCircles = new List<MagicCircle>();
        
        for (int i = 0; i < elements.Length; i++)
        {
            var supportCircle = new MagicCircle($"{elements[i]} Support", 3.0);
            
            // Position in a circle around the main circle
            var angle = i * 2 * Math.PI / elements.Length;
            supportCircle.CenterX = mainCircle.CenterX + 16 * Math.Cos(angle);
            supportCircle.CenterY = mainCircle.CenterY + 16 * Math.Sin(angle);
            
            var elementTalisman = new Talisman(new Element(elements[i]) { IsActive = true }, $"{elements[i]} Essence")
            {
                PowerLevel = 3.0 + i * 0.2
            };
            supportCircle.AddTalisman(elementTalisman);
            
            var connection = mainCircle.ConnectTo(supportCircle, connectionTypes[i]);
            connection.Strength = 1.2 + (i * 0.1);
            
            supportCircles.Add(supportCircle);
        }
        
        LoadCompositionExample(mainCircle, supportCircles.ToArray());
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = "Loaded Unified composition example";
    }
    
    private void LoadCompositionExample(MagicCircle mainCircle, params MagicCircle[] additionalCircles)
    {
        _currentCircle = mainCircle;
        _selectedCircle = mainCircle;
        
        // Clear existing circles and add all circles from the example
        _allCircles.Clear();
        _allCircles.Add(mainCircle);
        _allCircles.AddRange(additionalCircles);
        
        // Update the visualization
        UpdateCircleVisualization();
        UpdateCompositionList();
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = "Loaded composition example - explore the composition panel to see structure";
    }
    
    private void LoadCompositionExample(MagicCircle mainCircle, MagicCircle additionalCircle, string message)
    {
        LoadCompositionExample(mainCircle, new[] { additionalCircle });
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = message;
    }
    
    private void LoadCompositionExample(MagicCircle mainCircle, MagicCircle circle1, MagicCircle circle2, string message)
    {
        LoadCompositionExample(mainCircle, new[] { circle1, circle2 });
        
        var statusText = FindName("StatusText") as TextBlock;
        if (statusText != null)
            statusText.Text = message;
    }

    #endregion

    #region Canvas Panning

    private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            _isPanning = true;
            _lastPanPosition = e.GetPosition((Canvas)sender);
            
            var canvas = (Canvas)sender;
            canvas.CaptureMouse();
            canvas.Cursor = Cursors.SizeAll;
            
            e.Handled = true;
        }
    }

    private void MainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Released && _isPanning)
        {
            _isPanning = false;
            
            var canvas = (Canvas)sender;
            canvas.ReleaseMouseCapture();
            canvas.Cursor = Cursors.Arrow;
            
            e.Handled = true;
        }
    }

    private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isPanning && e.MiddleButton == MouseButtonState.Pressed)
        {
            var canvas = (Canvas)sender;
            var scrollViewer = FindName("CanvasScrollViewer") as ScrollViewer;
            
            if (scrollViewer != null)
            {
                var currentPosition = e.GetPosition(canvas);
                var deltaX = _lastPanPosition.X - currentPosition.X;
                var deltaY = _lastPanPosition.Y - currentPosition.Y;
                
                // Apply pan by adjusting scroll viewer offsets
                var newHorizontalOffset = scrollViewer.HorizontalOffset + deltaX;
                var newVerticalOffset = scrollViewer.VerticalOffset + deltaY;
                
                // Clamp to valid scroll ranges
                newHorizontalOffset = Math.Max(0, Math.Min(newHorizontalOffset, scrollViewer.ScrollableWidth));
                newVerticalOffset = Math.Max(0, Math.Min(newVerticalOffset, scrollViewer.ScrollableHeight));
                
                scrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
                scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
                
                _lastPanPosition = currentPosition;
            }
            
            e.Handled = true;
        }
    }

    private void MainCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_isPanning)
        {
            _isPanning = false;
            
            var canvas = (Canvas)sender;
            canvas.ReleaseMouseCapture();
            canvas.Cursor = Cursors.Arrow;
        }
    }

    #endregion
}