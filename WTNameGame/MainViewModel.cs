using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WTNameGame
{
    class MainViewModel : MainViewModelBase
    {
        private Game game = null;
        private string whoIsText = "";
        private ObservableCollection<ProfileShot> profileShots;

        public string WhoIsText
        {
            get
            {
                return whoIsText;
            }
            set
            {
                whoIsText = value;
                OnPropertyChanged(nameof(WhoIsText));
            }
        }

        public ObservableCollection<ProfileShot> ProfileShots
        {
            get
            {
                return profileShots;
            }
            set
            {
                profileShots = value;
                OnPropertyChanged(nameof(ProfileShots));
            }
        }

        public MainViewModel()
        {
            game = new Game();
            game.ProfilesGenerated += OnProfilesGenerated;
        }

        public ICommand PlayButtonClicked
        {
            get
            {
                return new DelegateCommand(PlayClickResults);
            }
        }
        public void PlayClickResults()
        {
            game.Play();
        }

        public void OnProfilesGenerated(Object source, ProfilesEventArgs eventArgs)
        {
            ProfileShots = eventArgs.ProfileShots;
            WhoIsText = eventArgs.CorrectProfile.FullName;
        }



    }
}
