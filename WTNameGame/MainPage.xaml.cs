using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WTNameGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string youWonMessage = "You got it! Check any remaining names or Play again.";
        private Game game;
        public ObservableCollection<ProfileShot> ProfileShots { get; set; }
        public ProfileShot CorrectProfile { get; set; }
        public bool IsRegularModeChecked { get; set; }
        public bool IsMattModeChecked { get; set; }
        public bool IsMModeChecked { get; set; }
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
            BeginFadeOut.Completed += (s, ea) =>
            {
                this.game.Play(IsMattModeChecked, IsMModeChecked, teamMode: false, reverse: IsReverseModeChecked);
            };
            BeginFadeOut.Begin();
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
            if (CorrectProfile.ImageOpacity == 0)
            {
                imageWhois.Source = new BitmapImage(new Uri(CorrectProfile.Url, UriKind.RelativeOrAbsolute));
                imageWhois.Visibility = Visibility.Visible;
                txtWhois.Text = "Who is this?";
            }
            else
            {
                imageWhois.Visibility = Visibility.Collapsed;
                txtWhois.Text = "Who is " + eventArgs.CorrectProfile.FullName + "?";
            }
            // Bindings.Update often gives an error message before compiling, but should be OK
            Bindings.Update();
            ResetProfileBackgrounds();
            BeginFadeIn.Begin();
        }

        /// <summary>
        /// Tapping an Image event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            ProfileShot shot = grid.DataContext as ProfileShot;
            shot.TextOpacity = shot.ImageOpacity = 1.0;
            bool won = game.IsCorrectMatch(CorrectProfile, shot);
            grid.Background = new SolidColorBrush( won ? Colors.LightGreen : Colors.LightCoral);
            txtWhois.Text = won ? youWonMessage : txtWhois.Text;
        }

        private void ResetProfileBackgrounds()
        {
            ProfileGridView.UpdateLayout();
            foreach (var item in ProfileGridView.Items)
            {
                var container = (GridViewItem)ProfileGridView.ContainerFromItem(item);
                if (container != null)
                {
                    var grid = (container.ContentTemplateRoot as FrameworkElement)?.FindName("DataGrid") as Grid;
                    if (grid != null)
                    {
                        grid.Background = new SolidColorBrush(Colors.White);
                    }
                }
            }
        }
    }
}
