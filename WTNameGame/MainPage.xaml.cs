using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WTNameGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Game game;
        public ObservableCollection<ProfileShot> ProfileShots { get; set; }
        public ProfileShot CorrectProfile { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = ProfileShots;
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
            this.game.Play();
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
            txtWhois.Text = "Who is " + eventArgs.CorrectProfile.FullName + "?";

            // Bindings.Update often gives an error message before compiling, but should be OK
            Bindings.Update();
        }

        /// <summary>
        /// Tapping an Image event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            ProfileShot shot = panel.DataContext as ProfileShot;
            panel.Background = new SolidColorBrush(shot.FullName.Equals(CorrectProfile.FullName) ?
                Colors.LightGreen : Colors.LightCoral);
            shot.TextVisible = Visibility.Visible;
        }
    }
}
