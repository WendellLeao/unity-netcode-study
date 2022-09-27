using Game.Events;

namespace Game.Gameplay.Playing
{
    public class PlayerHasSpawnEvent : ServiceEvent
    {
        public PlayerHasSpawnEvent(Player player)
        {
            Player = player;
        }
        
        public Player Player { get; }
    }
}