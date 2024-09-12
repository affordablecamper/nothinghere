using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.AI;
public class SendMovementToServer : NetworkBehaviour
{
    private NavMeshAgent agent;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestinationToServer(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public bool HasArrived()
    {
        // The path hasn't been computed yet if the path is pending.
        float remainingDistance;
        if (agent.pathPending)
        {
            remainingDistance = float.PositiveInfinity;
        }
        else
        {
            remainingDistance = agent.remainingDistance;
        }

        float arrivalDistance = 0.5f; // Replace 0f with the actual value of the acceptable arrival distance

        return remainingDistance <= arrivalDistance;
    }

}
