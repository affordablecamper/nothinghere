using FishNet;
using FishNet.Managing;
using UnityEngine;


public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private PlayerSpawnerModified playerSpawner;
    [SerializeField] private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = InstanceFinder.NetworkManager;
    }

    public void OnClick_Client()
    {

        
        if (_networkManager == null)
            return;

        if (_networkManager.ClientManager.Connection.IsValid)
        {
            _networkManager.ClientManager.StopConnection();
        }
        else
        {
            _networkManager.ClientManager.StartConnection();

            // Spawn the player when the client connection starts
            playerSpawner.SpawnPlayer();
        }

        DeselectButtons();
    }

    public void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        if (!_networkManager.ServerManager.enabled)
        {
            _networkManager.ServerManager.StopConnection(true);
        }
        else
        {
            _networkManager.ServerManager.StartConnection();

            // Spawn the player when the server connection starts
           
        }

        DeselectButtons();
    }

    private void DeselectButtons()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        eventSystem?.SetSelectedGameObject(null);
    }
}