using Unity.Netcode;
using UnityEngine;

namespace Game.Networking
{
    public class NetworkManagerWrapper : MonoBehaviour
    { 
        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;

            _networkManager.OnServerStarted += HandleServerStarted;

            _networkManager.OnClientConnectedCallback += HandleClientConnected;
            _networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        private void OnDestroy()
        {
            _networkManager.OnServerStarted -= HandleServerStarted;
            
            _networkManager.OnClientConnectedCallback -= HandleClientConnected;
            _networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        private void HandleServerStarted()
        {
            Debug.Log("The server has started.");
        }
        
        private void HandleClientConnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} has connected.");
        }
        
        private void HandleClientDisconnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} has disconnected.");
        }
    }
}
