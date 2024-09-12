using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.AI;
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] disableBehaviours;
   
    public GameObject PlayerModel;
    //public GameObject ViewModel;
    public bool isBot = false;
    public int playerID;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isBot)
        {
            if (base.IsServer)
            {
                NetworkObject networkObject = GetComponent<NetworkObject>();
                playerID = networkObject.OwnerId;
                ShootOverNetwork.instance.players.Add(playerID, new ShootOverNetwork.Player() { health = 100, playerObject = this.gameObject, connection = GetComponent<NetworkObject>().Owner, isBot = false});
                Debug.Log("Initialized Player under the ownerID Category as " + playerID);
            }

            if (!base.IsOwner)
            {
                DisableBehaviours();

            }

            if (base.IsOwner)
            {
                // Set the local player model and its children to the "LocalPlayer" layer
                SetLayerRecursively(PlayerModel, LayerMask.NameToLayer("LocalPlayer"));
                //SetLayerRecursively(ViewModel, LayerMask.NameToLayer("RemotePlayer")); // this doesnt make sense but it does do to the camera being able to see remoteplayers and not local players
            }
            else
            {
                // Set other players' models to the "RemotePlayer" layer
                SetLayerRecursively(PlayerModel, LayerMask.NameToLayer("RemotePlayer"));
                //SetLayerRecursively(ViewModel, LayerMask.NameToLayer("LocalPlayer"));
            }

        }
        else
        {
            playerID = gameObject.GetInstanceID();
            ShootOverNetwork.instance.players.Add(playerID, new ShootOverNetwork.Player() { health = 100, playerObject = this.gameObject, connection = GetComponent<NetworkObject>().Owner,isBot = true});

            Debug.Log("Initialized bot under the gameobjectID " + playerID);
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            agent.enabled = true; // little fix when ai is spawning on terrain
        }
        

    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }


    private void DisableBehaviours()
    {
        for (int i = 0; i < disableBehaviours.Length; i++)
        {
            if (disableBehaviours[i] != null)
            {
                disableBehaviours[i].enabled = false;
            }
            else
            {
                Debug.LogError("A behavior component is not assigned in the PlayerSetup script.");
            }
        }
    }
}