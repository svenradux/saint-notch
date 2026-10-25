using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SaintNotch
{
    public enum NotchState
    {
        S0, // Continuous idle pill (140x38)
        S1, // Compact media pill (380x56)
        S2  // Expanded downward panel (620x290)
    }

    public partial class MainWindow : Window
    {
        private NotchState _currentState = NotchState.S0;
        private Storyboard? _breathingStoryboard;
        private bool _isPlaying = true;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);

        private const uint SPI_GETCLIENTAREAANIMATION = 0x1042;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Position top-center of primary screen
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = 0;

            InitBreathingAnimation();
            ApplyState(_currentState, animate: false);
        }

        private bool IsReducedMotionEnabled()
        {
            try
            {
                bool animationEnabled = true;
                if (SystemParametersInfo(SPI_GETCLIENTAREAANIMATION, 0, ref animationEnabled, 0))
                {
                    return !animationEnabled;
                }
            }
            catch
            {
                // Fallback to normal animation
            }
            return false;
        }

        private void InitBreathingAnimation()
        {
            if (IsReducedMotionEnabled())
            {
                BreathingShadow.Opacity = 0.35;
                BreathingShadow.BlurRadius = 16;
                return;
            }

            var opacityAnim = new DoubleAnimation
            {
                From = 0.22,
                To = 0.68,
                Duration = TimeSpan.FromSeconds(1.8),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var blurAnim = new DoubleAnimation
            {
                From = 12.0,
                To = 24.0,
                Duration = TimeSpan.FromSeconds(1.8),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            _breathingStoryboard = new Storyboard();
            _breathingStoryboard.Children.Add(opacityAnim);
            _breathingStoryboard.Children.Add(blurAnim);

            Storyboard.SetTarget(opacityAnim, BreathingShadow);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(nameof(BreathingShadow.Opacity)));

            Storyboard.SetTarget(blurAnim, BreathingShadow);
            Storyboard.SetTargetProperty(blurAnim, new PropertyPath(nameof(BreathingShadow.BlurRadius)));

            _breathingStoryboard.Begin(this, isControllable: true);
        }

        public void TransitionTo(NotchState targetState, bool animate = true)
        {
            if (_currentState == targetState && animate) return;
            _currentState = targetState;
            ApplyState(targetState, animate);
        }

        private void ApplyState(NotchState state, bool animate)
        {
            double targetWidth = 140;
            double targetHeight = 38;
            CornerRadius targetRadius = new CornerRadius(19);

            switch (state)
            {
                case NotchState.S0:
                    targetWidth = 140;
                    targetHeight = 38;
                    targetRadius = new CornerRadius(19);
                    S0Content.Visibility = Visibility.Visible;
                    S1Content.Visibility = Visibility.Collapsed;
                    S2Content.Visibility = Visibility.Collapsed;
                    _breathingStoryboard?.Resume(this);
                    break;

                case NotchState.S1:
                    targetWidth = 380;
                    targetHeight = 56;
                    targetRadius = new CornerRadius(24);
                    S0Content.Visibility = Visibility.Collapsed;
                    S1Content.Visibility = Visibility.Visible;
                    S2Content.Visibility = Visibility.Collapsed;
                    _breathingStoryboard?.Pause(this);
                    BreathingShadow.Opacity = 0.40;
                    BreathingShadow.BlurRadius = 18;
                    break;

                case NotchState.S2:
                    targetWidth = 620;
                    targetHeight = 290;
                    targetRadius = new CornerRadius(18);
                    S0Content.Visibility = Visibility.Collapsed;
                    S1Content.Visibility = Visibility.Collapsed;
                    S2Content.Visibility = Visibility.Visible;
                    _breathingStoryboard?.Pause(this);
                    BreathingShadow.Opacity = 0.48;
                    BreathingShadow.BlurRadius = 22;
                    break;
            }

            NotchContainer.CornerRadius = targetRadius;

            if (animate && !IsReducedMotionEnabled())
            {
                var duration = TimeSpan.FromMilliseconds(260);
                var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

                var widthAnim = new DoubleAnimation(targetWidth, duration) { EasingFunction = ease };
                var heightAnim = new DoubleAnimation(targetHeight, duration) { EasingFunction = ease };

                NotchContainer.BeginAnimation(WidthProperty, widthAnim);
                NotchContainer.BeginAnimation(HeightProperty, heightAnim);
            }
            else
            {
                NotchContainer.BeginAnimation(WidthProperty, null);
                NotchContainer.BeginAnimation(HeightProperty, null);
                NotchContainer.Width = targetWidth;
                NotchContainer.Height = targetHeight;
            }
        }

        private void NotchContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentState == NotchState.S0)
            {
                TransitionTo(NotchState.S1);
            }
        }

        private void NotchContainer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }

        private void ExpandToS2_Click(object sender, RoutedEventArgs e)
        {
            TransitionTo(NotchState.S2);
        }

        private void CollapseToS1_Click(object sender, RoutedEventArgs e)
        {
            TransitionTo(NotchState.S1);
        }

        private void SwitchToS0_Click(object sender, RoutedEventArgs e)
        {
            TransitionTo(NotchState.S0);
        }

        private void SwitchToS1_Click(object sender, RoutedEventArgs e)
        {
            TransitionTo(NotchState.S1);
        }

        private void SwitchToS2_Click(object sender, RoutedEventArgs e)
        {
            TransitionTo(NotchState.S2);
        }

        private void MenuS0_Click(object sender, RoutedEventArgs e) => TransitionTo(NotchState.S0);
        private void MenuS1_Click(object sender, RoutedEventArgs e) => TransitionTo(NotchState.S1);
        private void MenuS2_Click(object sender, RoutedEventArgs e) => TransitionTo(NotchState.S2);

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private void HeroPlayPause_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TogglePlayPause();
        }

        private void TogglePlayPause()
        {
            _isPlaying = !_isPlaying;
            string icon = _isPlaying ? "⏸" : "▶";
            S1PlayPauseText.Text = icon;
            S2PlayPauseText.Text = icon;
        }

        private void PrevTrack_Click(object sender, RoutedEventArgs e)
        {
            // Visual feedback indicator
        }

        private void NextTrack_Click(object sender, RoutedEventArgs e)
        {
            // Visual feedback indicator
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    TransitionTo(NotchState.S0);
                    break;

                case Key.D1:
                case Key.NumPad1:
                    TransitionTo(NotchState.S1);
                    break;

                case Key.D2:
                case Key.NumPad2:
                    TransitionTo(NotchState.S2);
                    break;

                case Key.Space:
                    TogglePlayPause();
                    break;

                case Key.Tab:
                    int next = ((int)_currentState + 1) % 3;
                    TransitionTo((NotchState)next);
                    break;

                case Key.Escape:
                    if (_currentState == NotchState.S2)
                        TransitionTo(NotchState.S1);
                    else if (_currentState == NotchState.S1)
                        TransitionTo(NotchState.S0);
                    else
                        Application.Current.Shutdown();
                    break;

                case Key.Q:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Application.Current.Shutdown();
                    }
                    break;
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
