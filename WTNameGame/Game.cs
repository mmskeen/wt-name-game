using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

namespace WTNameGame
{
    public class Game
    {
        const int selectionSize = 5;
        private List<Profile> allProfilesList;
        private List<Profile> mattProfilesList = new List<Profile>();
        private List<Profile> mProfilesList = new List<Profile>();
        private List<Profile> teamProfilesList = new List<Profile>();
        private ProfileShot correctProfile;
        private ObservableCollection<ProfileShot> profileShots = new ObservableCollection<ProfileShot>();
        private static Random rnd = new Random();

        public int SelectionSize { get => selectionSize; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public bool HasWon { get; private set; }
        public event EventHandler<ProfilesEventArgs> ProfilesGenerated;

        /// <summary>
        /// Constructor -- immediately fetches json profile data and stores in profile lists
        /// </summary>
        public Game()
        {
            CreateAllProfilesList();
            CreateFilteredProfileLists();
        }

        /// <summary>
        /// Generate five new profileShots (and a correct profile) to play a new game
        /// </summary>
        public void Play(bool mattMode, bool mMode, bool teamMode)
        {
            if (allProfilesList != null)
            {
                List<Profile> list = mattMode ? mattProfilesList : mMode ? mProfilesList : teamMode ? teamProfilesList : allProfilesList;
                SelectUniqueProfileShotsFromList(selectionSize, list);
                SelectCorrectProfileShot();
                HasWon = false;
            }
            OnProfilesGenerated();
        }

        /// <summary>
        /// Checks whether a guessed profile matches the given (correct) profile
        /// </summary>
        /// <param name="correctShot">profileshot of the correct profile</param>
        /// <param name="guessedShot">profileshot of the guessed profile</param>
        /// <returns></returns>
        public bool IsCorrectMatch(ProfileShot correctShot, ProfileShot guessedShot)
        {
            bool correct = correctShot.FullName.Equals(guessedShot.FullName);
            Wins = correct && !HasWon ? Wins + 1 : Wins;
            Losses =  !correct && !HasWon ? Losses + 1 : Losses;
            HasWon = correct ? true : HasWon;
            return correct;
        }

        /// <summary>
        /// Resets the Score to 0 wins and 0 losses
        /// </summary>
        public void ResetScore()
        {
            Wins = Losses = 0;
        }

        private void SelectUniqueProfileShotsFromList(int desiredCount, List<Profile> list)
        {
            profileShots.Clear();
            while (profileShots.Count < desiredCount)
            {
                int r = rnd.Next(list.Count);
                if (!(from shot in profileShots
                      where shot.FullName.Equals(list[r].FullName)
                      select shot).Any())
                {
                    profileShots.Add(new ProfileShot(list[r].FullName, list[r].JobTitle, list[r].Headshot.Url));
                }
            }
        }

        private void SelectCorrectProfileShot()
        {
            int rand = rnd.Next(selectionSize);
            correctProfile = profileShots?[rand];
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
            if (allProfilesList == null)
            {
                return;
            }
            foreach (Profile profile in allProfilesList)
            {
                if (profile.FullName.StartsWith("M", StringComparison.CurrentCultureIgnoreCase))
                {
                    mProfilesList.Add(profile);
                    if (profile.FullName.StartsWith("Mat", StringComparison.CurrentCultureIgnoreCase))
                    {
                        mattProfilesList.Add(profile);
                    }
                }
                if (profile.JobTitle != null)
                {
                    teamProfilesList.Add(profile);
                }
            }
        }

        private void CreateAllProfilesList()
        {
            var client = new RestClient("https://willowtreeapps.com/api/");
            var request = new RestRequest("v1.0/profiles/", Method.GET)
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            var queryResult = client.Execute(request);
            allProfilesList = JsonConvert.DeserializeObject<List<Profile>>(queryResult.Content);
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

}
