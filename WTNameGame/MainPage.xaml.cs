using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
        private MainViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = this.DataContext as MainViewModel;
            vm.ViewModelUpdated += OnViewModelUpdated;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.game.Play();
        }

        /// <summary>
        /// Tapping an Image event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button btn = sender as Button;
            ProfileShot shot = btn.DataContext as ProfileShot;
            shot.Background = new SolidColorBrush(shot.FullName.Equals("Tony") ?
                Colors.LightGreen : Colors.LightCoral);
            shot.TextVisible = Visibility.Visible;
        }

        private void OnViewModelUpdated(object sender, EventArgs e)
        {
            this.Bindings.Update();
        }
    }

}
