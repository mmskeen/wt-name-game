using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
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

        public Game() { }

        public void Play()
        {
            var client = new RestClient("https://willowtreeapps.com/api/");

            var request = new RestRequest("v1.0/profiles/", Method.GET)
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };

            var queryResult = client.Execute(request);

            profileList = JsonConvert.DeserializeObject<List<Profile>>(queryResult.Content);

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

    public class ProfilesEventArgs : EventArgs
    {
        public ObservableCollection<ProfileShot> ProfileShots { get; set; }
        public ProfileShot CorrectProfile { get; set; }
    }

    public class ProfileShot
    {
        private string url_;
        public Visibility TextVisible { get; set; }
        public string FullName { get; set; }
        public string Url
        {
            get
            {
                return "http:" + url_;
            }
            set
            {
                url_ = value;
            }
        }
        public ProfileShot(string fullname, string url)
        {
            this.FullName = fullname;
            this.Url = url;
            this.TextVisible = Visibility.Collapsed;
        }
    }
}
