using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WTNameGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string youWonMessage = "You got it! Check any remaining names or Play again.";
        private const string reverseWhoIsText = "Who is this?";
        private const string scoreWinLossSeparator = " / ";
        private Uri playSoundUri = new Uri("ms-appx:///Assets/Windows Message Nudge.wav");
        private Uri correctSoundUri = new Uri("ms-appx:///Assets/Windows Exclamation.wav");
        private Uri incorrectSoundUri = new Uri("ms-appx:///Assets/Windows Error.wav");

        public string ShortFadeDuration { get => "0:0:0.4"; }
        private Game game;
        private SolidColorBrush correctColor = new SolidColorBrush(Colors.LightGreen);
        private SolidColorBrush incorrectColor = new SolidColorBrush(Colors.LightCoral);
        public ObservableCollection<ProfileShot> ProfileShots { get; set; }
        public ProfileShot CorrectProfile { get; set; }
        public bool IsRegularModeChecked { get; set; }
        public bool IsMattModeChecked { get; set; }
        public bool IsMModeChecked { get; set; }
        public bool IsTeamModeChecked { get; set; }
        public bool IsReverseModeChecked { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = ProfileShots;
            IsRegularModeChecked = true;
            game = new Game();
            game.ProfilesGenerated += OnProfilesGenerated;
        }

        /// <summary>
        /// Play Button Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            GridViewFadeOut.Completed += (s, ea) =>
            {
                this.game.Play(IsMattModeChecked, IsMModeChecked, IsTeamModeChecked);
            };
            GridViewFadeOut.Begin();
            FadeOutWhoIsImageRect.Begin();
            PlayPlaySound();
        }

        /// <summary>
        /// Event Handler for new profile shots being generated
        /// </summary>
        /// <param name="source"></param>
        /// <param name="eventArgs"></param>
        public void OnProfilesGenerated(Object source, ProfilesEventArgs eventArgs)
        {
            ProfileShots = eventArgs.ProfileShots;
            CorrectProfile = eventArgs.CorrectProfile;

            // Bindings.Update often gives an error message before compiling, but should be OK
            Bindings.Update();
            if (ProfileShots == null || CorrectProfile == null)
            {
                txtWhois.Text = "Cannot access data. Check Internet connection and restart game.";
                return;
            }
            UpdateWhoIsImageAndText(eventArgs);
            ResetProfileBackgrounds();
            GridViewFadeIn.Begin();
        }

        private void UpdateWhoIsImageAndText(ProfilesEventArgs eventArgs)
        {
            if (IsReverseModeChecked)
            {
                imageWhois.ImageSource = new BitmapImage(new Uri(CorrectProfile.Url, UriKind.RelativeOrAbsolute));
                rectWhois.Visibility = Visibility.Visible;
                txtWhois.Text = reverseWhoIsText;
            }
            else
            {
                rectWhois.Visibility = Visibility.Collapsed;
                txtWhois.Text = "Who is " + eventArgs.CorrectProfile.FullName + "?";
            }
            FadeInWhoIsImageRect.Begin();
        }

        /// <summary>
        /// Tapping an Image event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (HasAlreadyBeenTapped(grid))
            {
                return;
            }
            FadeInTextAndImage(grid);
            ProfileShot shot = grid.DataContext as ProfileShot;
            bool isCorrect = game.IsCorrectMatch(CorrectProfile, shot);
            grid.Background = isCorrect ? correctColor : incorrectColor;
            PlayTappedSound(isCorrect);
            txtWhois.Text = isCorrect ? youWonMessage : txtWhois.Text;
            scoreText.Text = game.Wins + scoreWinLossSeparator + game.Losses;
        }

        private static void FadeInTextAndImage(Grid grid)
        {
            if (grid.FindName("FadeInClickedText") is Storyboard fadeInText)
            {
                fadeInText.Begin();
            }
            if (grid.FindName("FadeInClickedImage") is Storyboard fadeInImage)
            {
                fadeInImage.Begin();

            }
            if (grid.FindName("NameTxt") is TextBlock textBlock)
            {
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                textBlock.FontWeight = FontWeights.ExtraBold;
            }
        }

        private bool HasAlreadyBeenTapped(Grid grid)
        {
            bool hasNotBeenTappedYet = grid.IsTapEnabled;
            grid.IsTapEnabled = false;
            return !hasNotBeenTappedYet;
        }

        private void PlayPlaySound()
        {
            myMediaElement.Source = playSoundUri;
            myMediaElement.Play();
        }

        private void PlayTappedSound(bool isCorrect)
        {
            if (!game.HasWon || isCorrect)
            {
                myMediaElement.Source = isCorrect ? correctSoundUri : incorrectSoundUri;
                myMediaElement.Play();
            }
        }

        private void ResetProfileBackgrounds()
        {
            ProfileGridView.UpdateLayout();
            foreach (var item in ProfileGridView.Items)
            {
                var container = (GridViewItem)ProfileGridView.ContainerFromItem(item);
                if (container != null)
                {
                    if ((container?.ContentTemplateRoot as FrameworkElement)?.FindName("DataGrid") is Grid grid)
                    {
                        grid.Background = new SolidColorBrush(Colors.Transparent);
                        ResetTappedAlready(grid);
                    }
                    ResetProfileImageOpacity(container);
                    ResetProfileNameTxt(container);
                }
            }
        }

        private void ResetProfileImageOpacity(GridViewItem container)
        {
            if ((container.ContentTemplateRoot as FrameworkElement)?.FindName("imageRectangle") is Rectangle rect)
            {
                rect.Opacity = IsReverseModeChecked ? 0 : 1;
            }
        }

        private void ResetProfileNameTxt(GridViewItem container)
        {
            if ((container.ContentTemplateRoot as FrameworkElement)?.FindName("NameTxt") is TextBlock textBlock)
            {
                textBlock.VerticalAlignment = IsReverseModeChecked ? VerticalAlignment.Center : VerticalAlignment.Bottom;
                textBlock.Foreground = new SolidColorBrush(IsReverseModeChecked ? Colors.Black : Colors.White);
                textBlock.FontWeight = IsReverseModeChecked ? FontWeights.Light : FontWeights.ExtraBold;
                textBlock.Opacity = IsReverseModeChecked ? 1.0 : 0.0;
            }
        }

        private void ResetTappedAlready(Grid grid)
        {
            grid.IsTapEnabled = true;
        }

        private void ScoreText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.ResetScore();
            scoreText.Text = game.Wins + scoreWinLossSeparator + game.Losses;
        }
    }
}
