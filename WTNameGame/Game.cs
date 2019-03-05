using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using System.Linq;

namespace WTNameGame
{
    public class Game
    {
        const int selectionSize = 5;
        private List<Profile> profileList;
        private List<Profile> mattProfileList = new List<Profile>();
        private List<Profile> mProfileList = new List<Profile>();
        private List<Profile> teamProfileList = new List<Profile>();
        private ProfileShot correctProfile;
        private ObservableCollection<ProfileShot> profileShots = new ObservableCollection<ProfileShot>();
        private static Random rnd = new Random();

        public int SelectionSize { get => selectionSize; }
        public event EventHandler<ProfilesEventArgs> ProfilesGenerated;

        /// <summary>
        /// Constructor -- immediately fetches json profile data and stores in ProfileList
        /// </summary>
        public Game()
        {
            CreateFullProfileList();
            CreateFilteredProfileLists();
        }

        /// <summary>
        /// Generate five new profileShots (and a correct profile) to play a new game
        /// </summary>
        public void Play(bool mattMode, bool mMode, bool teamMode, bool reverse)
        {
            if (profileList == null)
            {
                return;
            }
            List<Profile> list = mattMode ? mattProfileList : mMode ? mProfileList : teamMode ? teamProfileList : profileList;
            SelectUniqueProfileShots(selectionSize, list, reverse);
            int rand = rnd.Next(selectionSize);
            correctProfile = profileShots[rand];
            OnProfilesGenerated();
        }

        public bool IsCorrectMatch(ProfileShot correctShot, ProfileShot testShot)
        {
            return correctShot.FullName.Equals(testShot.FullName);
        }

        /// <summary>
        /// Selects and creates a unique collection of profile shots (full name & URL)
        /// </summary>
        /// <param name="desiredCount"></param>
        private void SelectUniqueProfileShots(int desiredCount, List<Profile> list, bool reverse)
        {
            profileShots.Clear();
            while (profileShots.Count < desiredCount)
            {
                int r = rnd.Next(list.Count);
                if (!(from shot in profileShots
                      where shot.FullName.Equals(list[r].FullName)
                      select shot).Any())
                {
                    profileShots.Add(new ProfileShot(list[r].FullName, list[r].JobTitle, list[r].Headshot.Url, reverse));
                }
            }
        }

        /// <summary>
        /// Publish (raise) an event that new profile shots have been generated
        /// </summary>
        protected virtual void OnProfilesGenerated()
        {
            ProfilesGenerated?.Invoke(this, new ProfilesEventArgs()
            {
                ProfileShots = profileShots,
                CorrectProfile = correctProfile
            });
        }

        private void CreateFilteredProfileLists()
        {
            foreach (Profile profile in profileList)
            {
                if (profile.FullName.StartsWith("M", StringComparison.CurrentCultureIgnoreCase))
                {
                    mProfileList.Add(profile);
                    if (profile.FullName.StartsWith("Mat", StringComparison.CurrentCultureIgnoreCase))
                    {
                        mattProfileList.Add(profile);
                    }
                }
                if (profile.JobTitle != null)
                {
                    teamProfileList.Add(profile);
                }
            }
        }

        private void CreateFullProfileList()
        {
            var client = new RestClient("https://willowtreeapps.com/api/");
            var request = new RestRequest("v1.0/profiles/", Method.GET)
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            var queryResult = client.Execute(request);
            profileList = JsonConvert.DeserializeObject<List<Profile>>(queryResult.Content);
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
        private double textOpacity_;
        private double imageOpacity_;
        public double TextOpacity
        {
            get
            {
                return textOpacity_;
            }
            set
            {
                textOpacity_ = value;
                OnPropertyChanged(nameof(TextOpacity));
            }
        }
        public double ImageOpacity
        {
            get
            {
                return imageOpacity_;
            }
            set
            {
                imageOpacity_ = value;
                OnPropertyChanged(nameof(ImageOpacity));
            }
        }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
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
        public ProfileShot(string fullname, string jobTitle, string url, bool reverse)
        {
            this.FullName = fullname;
            this.JobTitle = jobTitle;
            this.Url = url;
            TextOpacity = reverse ? 1 : 0;
            ImageOpacity = reverse ? 0 : 1;
        }
    }
}
