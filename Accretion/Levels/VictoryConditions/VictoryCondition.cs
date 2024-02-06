using Accretion.AudioHelpers;
using Accretion.GameplayObjects;
using Microsoft.Xna.Framework.Media;

namespace Accretion.Levels.VictoryConditions
{
    public enum GameStatus
    {
        LevelSelectMenu,
        Starting,
        OpeningText,
        InProgress,
        Defeat,
        Victory,
        Credits
    }

    internal abstract class VictoryCondition
    {
        public static readonly Song victoryMusic = AccretionGame.staticContent.Load<Song>("BrokeForFree-SomethingElated");

        public virtual GameStatus gameStatus(Field field)
        {
            if (field.getPlayer() == null || field.getPlayer().getMass() <= 0)
            {
                this.onDefeat();
                return GameStatus.Defeat;
            }
            else
            {
                return GameStatus.InProgress;
            }
        }

        public virtual void onVictory()
        {
            SimplifiedMusicPlayer.PlaySong(victoryMusic);
        }

        public virtual void onDefeat()
        {
        }
    }
}