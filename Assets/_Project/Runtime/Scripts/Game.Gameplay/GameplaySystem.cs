using Game.Gameplay.Playing;
using Game.Services;
using Game.Events;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameplaySystem : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        
        private IEventService _eventService;

        private void Awake()
        {
            _eventService = ServiceLocator.GetService<IEventService>();
            
            _playerManager.Initialize(_eventService);
        }

        private void OnDestroy()
        {
            _playerManager.Dispose();
        }

        private void Update()
        {
            _playerManager.Tick(Time.deltaTime);
        }
    }
}
