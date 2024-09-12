using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BotAnimationEngine : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<NavMeshAgent>().velocity);

        // Update the animator parameters
        animator.SetFloat("ForwardSpeed", localVelocity.z);
        animator.SetFloat("SideSpeed", localVelocity.x);
    }
}
