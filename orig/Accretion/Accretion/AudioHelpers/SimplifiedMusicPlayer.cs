using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Accretion.Levels.VictoryConditions;

namespace Accretion.AudioHelpers
{
    class SimplifiedMusicPlayer
    {
        public static List<String> songs = new List<String>() 
        {
            "BrokeForFree-NightOwl",
            "BrokeForFree-DayBird",
            "BrokeForFree-TheGreat",
            "BrokeForFree-MyLuck",
            "BrokeForFree-HighSchoolSnaps",
            "BrokeForFree-OnlyInstrumental",
            "BrokeForFree-OurEgo",
            "ChrisZabriskie-DividerMod",
            "RevolutionVoid-ScatteredKnowledge",
            "ChrisZabriskie-WonderCycleMod",
            "ChrisZabriskie-CandlepowerMod"
        };

        private static List<Song> songList;

        private static int trackNumber = 0;

        private static DateTime lastManualSongStarted = DateTime.MinValue;

        static SimplifiedMusicPlayer()
        {            
            MediaPlayer.MediaStateChanged += delegate(object sender, EventArgs e)
            {
                ContinuePlaying();
            };

            songList = new List<Song>();
            foreach (String songName in songs)
            {
                songList.Add(AccretionGame.staticContent.Load<Song>(songName));
            }

            //randomize the starting track
            trackNumber = new Random().Next(songList.Count);

            MediaPlayer.IsRepeating = false;
        }

        public static void PlaySong(Song song)
        {
            lastManualSongStarted = DateTime.Now;

            if (MediaPlayer.Queue.ActiveSong != song)
            {
                MediaPlayer.Play(song);
            }
        }

        //plays the next song if no song is currently playing
        static void ContinuePlaying()
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                PlayNextPlaylistSong();
            }
        }

        public static void ContinuePlayingNonVictoryMusic()
        {
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong.Name == VictoryCondition.victoryMusic.Name)
            {
                PlayNextPlaylistSong();
            }
        }

        static void PlayNextPlaylistSong()
        {
            if (DateTime.Now - lastManualSongStarted > TimeSpan.FromSeconds(0.5))
            {
                MediaPlayer.Play(songList[trackNumber++ % songList.Count]);
            }
        }
    }
}
