using Game.Services;
using Game.Events;
using UnityEngine;

namespace Game.Gameplay.Playing
{
    public sealed class Player : NetworkEntity
    {
        [SerializeField] private PlayerMovement _playerMovement;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            IEventService eventService = ServiceLocator.GetService<IEventService>();
            
            eventService.DispatchEvent(new PlayerHasSpawnEvent(this));
        }

        public void Begin()
        {
            _playerMovement.Begin();
        }

        public void Stop()
        {
            _playerMovement.Stop();
        }

        public void Tick(float deltaTime)
        {
            _playerMovement.Tick(deltaTime);
        }
    }
}
