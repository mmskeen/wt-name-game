using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace WTNameGame
{
    public class Game
    {
        const int selectionSize = 5;
        private List<Profile> profileList;
        private ProfileShot correctProfile;
        private ObservableCollection<ProfileShot> profileShots = new ObservableCollection<ProfileShot>();
        private static System.Random rnd = new System.Random();

        public event EventHandler<ProfilesEventArgs> ProfilesGenerated;

        /// <summary>
        /// Constructor -- immediately fetches json profile data and stores in ProfileList
        /// </summary>
        public Game()
        {
            var client = new RestClient("https://willowtreeapps.com/api/");
            var request = new RestRequest("v1.0/profiles/", Method.GET)
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            var queryResult = client.Execute(request);
            profileList = JsonConvert.DeserializeObject<List<Profile>>(queryResult.Content);
        }

        /// <summary>
        /// Generate five new profileShots (and a correct profile) to play a new game
        /// </summary>
        public void Play()
        {
            profileShots.Clear();
            while (profileShots.Count < selectionSize)
            {
                int r = rnd.Next(profileList.Count);
                profileShots.Add(new ProfileShot(profileList[r].FullName, profileList[r].Headshot.Url));
            }
            int rand = rnd.Next(selectionSize);
            correctProfile = profileShots[rand];

            OnProfilesGenerated();
        }

        /// <summary>
        /// Publish (raise) an event that new profile shots have been generated
        /// </summary>
        protected virtual void OnProfilesGenerated()
        {
            if (ProfilesGenerated != null)
            {
                ProfilesGenerated(this, new ProfilesEventArgs()
                {
                    ProfileShots = profileShots,
                    CorrectProfile = correctProfile
                });
            }
        }
    }

    /// <summary>
    /// EventArgs class to hold profile data in ProfilesGenerated event parameters
    /// </summary>
    public class ProfilesEventArgs : EventArgs
    {
        public ObservableCollection<ProfileShot> ProfileShots { get; set; }
        public ProfileShot CorrectProfile { get; set; }
    }

    /// <summary>
    /// ProfileShot class to hold employee faces, names, etc. for the game
    /// </summary>
    public class ProfileShot : ModelBase
    {
        private const string URL_PREFIX = "http:";
        private string url_;
        private Visibility textVisible = Visibility.Collapsed;
        public Visibility TextVisible
        {
            get
            {
                return textVisible;
            }
            set
            {
                textVisible = value;
                OnPropertyChanged(nameof(TextVisible));
            }
        }
        public string FullName { get; set; }
        public string Url
        {
            get
            {
                return URL_PREFIX + url_;
            }
            set
            {
                // Using conditional to prevent runtime errors for null url's from json data (WillowTree logo)
                url_ = value != null ? value : "//images.ctfassets.net/3cttzl4i3k1h/1PoufpRNis4mmAmiqkA0ge/ef1fc7606584d54b5892010a65a5a262/WT_Logo-Hye-tTeI0Z.png";
            }
        }
        public ProfileShot(string fullname, string url)
        {
            this.FullName = fullname;
            this.Url = url;
        }
    }
}
