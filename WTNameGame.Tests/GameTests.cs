
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
            PlayTemplate(mMode: false, mattMode: false, teamMode: false, reverse: false, passTestMethod: alwaysTrue);
        }

        [TestMethod]
        public void Play_MattMode_StartsWithMatt()
        {
            bool func(ProfileShot shot)
            {
                return shot.FullName.StartsWith("Mat", StringComparison.CurrentCultureIgnoreCase);
            }
            PlayTemplate(mMode: false, mattMode: true, teamMode: false, reverse: false, passTestMethod: func);

        }

        [TestMethod]
        public void Play_MMode_StartsWithM()
        {
            bool startsWithM(ProfileShot shot)
            {
                return shot.FullName.StartsWith("M", StringComparison.CurrentCultureIgnoreCase);
            }
            PlayTemplate(mMode: true, mattMode: false, teamMode: false, reverse: false, passTestMethod: startsWithM);
        }

        [TestMethod]
        public void Play_TeamMode_JobTitleNotNull()
        {
            bool jobTitleNotNull(ProfileShot shot)
            {
                return shot.JobTitle != null;
            }
            PlayTemplate(mMode: false, mattMode: false, teamMode: true, reverse: false, passTestMethod: jobTitleNotNull);
        }

        [TestMethod]
        public void Play_ReverseMode_OpacitiesReversed()
        {
            bool obacitiesReversed(ProfileShot shot)
            {
                return shot.TextOpacity == 1 && shot.ImageOpacity == 0;
            }
            PlayTemplate(mMode: false, mattMode: false, teamMode: false, reverse: true, passTestMethod: obacitiesReversed);
        }

        [TestMethod]
        public void Play_MattAndReverseMode_StartsWithMattAndOpacitiesReversed()
        {
            bool startsWithMattAndOpacitiesReversed(ProfileShot shot)
            {
                return shot.TextOpacity == 1 && shot.ImageOpacity == 0
                    && shot.FullName.StartsWith("Mat", StringComparison.CurrentCultureIgnoreCase);
            }
            PlayTemplate(mMode: false, mattMode: true, teamMode: false, reverse: true, passTestMethod: startsWithMattAndOpacitiesReversed);
        }

        [TestMethod]
        public void IsCorrectMatch_TestShot_ReturnsTrueOrFalseCorrectly()
        {
            ProfileShot correctShot;
            ObservableCollection<ProfileShot> myProfileShots;
            game.ProfilesGenerated += delegate (Object source, ProfilesEventArgs eventArgs)
            {
                myProfileShots = eventArgs.ProfileShots;
                correctShot = eventArgs.CorrectProfile;
                Assert.IsTrue(game.IsCorrectMatch(correctShot, correctShot));
                foreach (ProfileShot testShot in myProfileShots)
                {
                    if (correctShot.FullName.Equals(testShot.FullName))
                    {
                        Assert.IsTrue(game.IsCorrectMatch(correctShot, testShot));
                    }
                    else
                    {
                        Assert.IsFalse(game.IsCorrectMatch(correctShot, testShot));
                    }
                }
            };
            for (int i = 0; i < numberOfIterationsToTest; ++i)
            {
                game.Play(mattMode: false, mMode: false, teamMode: false, reverse: false);
            }
        }

        private void PlayTemplate(bool mMode, bool mattMode, bool teamMode, bool reverse, Func<ProfileShot,bool> passTestMethod)
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
                game.Play(mattMode: mattMode, mMode: mMode, teamMode: teamMode, reverse: reverse);
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
