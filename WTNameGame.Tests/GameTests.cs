
using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WTNameGame.Tests
{
    [TestClass]
    public class GameTests
    {
        Game game;
        const int numberOfIterationsToTest = 100;

        [TestInitialize]
        public void Initialize()
        {
            game = new Game();
        }

        [TestMethod]
        public void Play_NormalMode_ProfilesAreUnique()
        {
            bool alwaysTrue(ProfileShot shot) { return true; }
            PlayTemplate(mMode: false, mattMode: false, teamMode: false, passTestMethod: alwaysTrue);
        }

        [TestMethod]
        public void Play_MattMode_StartsWithMatt()
        {
            bool func(ProfileShot shot)
            {
                return shot.FullName.StartsWith("Mat", StringComparison.CurrentCultureIgnoreCase);
            }
            PlayTemplate(mMode: false, mattMode: true, teamMode: false, passTestMethod: func);

        }

        [TestMethod]
        public void Play_MMode_StartsWithM()
        {
            bool startsWithM(ProfileShot shot)
            {
                return shot.FullName.StartsWith("M", StringComparison.CurrentCultureIgnoreCase);
            }
            PlayTemplate(mMode: true, mattMode: false, teamMode: false, passTestMethod: startsWithM);
        }

        [TestMethod]
        public void Play_TeamMode_JobTitleNotNull()
        {
            bool jobTitleNotNull(ProfileShot shot)
            {
                return shot.JobTitle != null;
            }
            PlayTemplate(mMode: false, mattMode: false, teamMode: true, passTestMethod: jobTitleNotNull);
        }

        [TestMethod]
        public void IsCorrectMatch_TestShot_UpdatesScoreCorrectly()
        {
            ProfileShot correctShot;
            ObservableCollection<ProfileShot> myProfileShots;
            game.ProfilesGenerated += delegate (Object source, ProfilesEventArgs eventArgs)
            {
                myProfileShots = eventArgs.ProfileShots;
                correctShot = eventArgs.CorrectProfile;
                int wins, losses;
                foreach (ProfileShot guessedShot in myProfileShots)
                {
                    wins = game.Wins;
                    losses = game.Losses;
                    bool gameAlreadyWon = game.HasWon;
                    if (correctShot.FullName.Equals(guessedShot.FullName))
                    {
                        Assert.IsTrue(game.IsCorrectMatch(correctShot, guessedShot));
                        Assert.IsTrue(game.HasWon);
                        Assert.IsTrue(game.Wins == wins + 1);
                        Assert.IsTrue(game.Losses == losses);
                    }
                    else
                    {
                        Assert.IsFalse(game.IsCorrectMatch(correctShot, guessedShot));
                        if (gameAlreadyWon)
                        {
                            Assert.IsTrue(game.HasWon);
                            Assert.IsTrue(game.Wins == wins);
                            Assert.IsTrue(game.Losses == losses);
                        }
                        else
                        {
                            Assert.IsFalse(game.HasWon);
                            Assert.IsTrue(game.Wins == wins);
                            Assert.IsTrue(game.Losses == losses + 1);
                        }
                    }
                }
                Assert.IsTrue(game.IsCorrectMatch(correctShot, correctShot));
                game.ResetScore();
                Assert.IsTrue(game.Losses == 0 && game.Wins == 0);
                foreach (ProfileShot guessedShot in myProfileShots)
                {
                    game.IsCorrectMatch(correctShot, guessedShot);
                }
                Assert.IsTrue(game.Losses == 0 && game.Wins == 0);
            };
            for (int i = 0; i < numberOfIterationsToTest; ++i)
            {
                game.Play(mattMode: false, mMode: false, teamMode: false);
            }
        }

        private void PlayTemplate(bool mMode, bool mattMode, bool teamMode, Func<ProfileShot,bool> passTestMethod)
        {
            ObservableCollection<ProfileShot> myProfileShots;
            game.ProfilesGenerated += delegate (Object source, ProfilesEventArgs eventArgs)
            {
                myProfileShots = eventArgs.ProfileShots;
                Assert.IsTrue(myProfileShots.Count == game.SelectionSize);
                Assert.IsTrue(AreProfileShotsUnique(myProfileShots));
                foreach (ProfileShot shot in myProfileShots)
                {
                    if (!passTestMethod(shot))
                    {
                        Assert.Fail();
                    }
                }
            };
            for (int i = 0; i<numberOfIterationsToTest; ++i)
            {
                game.Play(mattMode: mattMode, mMode: mMode, teamMode: teamMode);
            }
        }

        private bool AreProfileShotsUnique(ObservableCollection<ProfileShot> profileShots)
        {
            for (int i = 0; i < profileShots.Count - 1; ++i)
            {
                for (int j = i + 1; j < profileShots.Count; ++j)
                {
                    if (profileShots[j].FullName.Equals(profileShots[i].FullName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
