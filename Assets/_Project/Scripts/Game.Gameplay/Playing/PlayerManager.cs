using System.Collections.Generic;
using Game.Events;
using UnityEngine;

namespace Game.Gameplay.Playing
{
    public sealed class PlayerManager : MonoBehaviour
    {
        private List<Player> _spawnedPlayers;
        private IEventService _eventService;

        public void Initialize(IEventService eventService)
        {
            _eventService = eventService;

            _spawnedPlayers = new List<Player>();
            
            _eventService.AddEventListener<PlayerHasSpawnEvent>(HandlePlayerHasSpawn);
        }

        public void Dispose()
        {
            foreach (Player spawnedPlayer in _spawnedPlayers)
            {
                spawnedPlayer.Stop();
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (Player spawnedPlayer in _spawnedPlayers)
            {
                if (!spawnedPlayer.IsOwner)
                {
                    return;
                }
                
                spawnedPlayer.Tick(deltaTime);
            }
        }
        
        private void HandlePlayerHasSpawn(ServiceEvent serviceEvent)
        {
            if (serviceEvent is PlayerHasSpawnEvent playerHasSpawnEvent)
            {
                Player player = playerHasSpawnEvent.Player;
                
                _spawnedPlayers.Add(player);
                
                player.Begin();
            }
        }
    }
}
