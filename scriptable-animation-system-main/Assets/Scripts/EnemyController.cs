using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
public class EnemyController : MonoBehaviour
{

    public enum StateEnum
    {
        Firing,
        Idle,
        Run,
        Walk,
        TossGrenade,
      
    }

    
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField]private StateEnum currentState = StateEnum.Idle;
    private BehaviorTree behaviorTree;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    private void Update()
    {
        

        // Perform state-specific behavior
        switch (currentState)
        {
            case StateEnum.Firing:
                // Perform firing logic
                break;
            case StateEnum.Idle:
                // Perform idle logic
                break;
            case StateEnum.Run:
                // Perform run logic
                break;
            case StateEnum.Walk:
                // Perform walk logic
                break;
            case StateEnum.TossGrenade:
                // Perform toss grenade logic
                break;
           
            
        }
    }

    public void MeleeCanHit()
    {
        SharedBool canHitPlayer = (SharedBool)behaviorTree.GetVariable("canHitPlayer");
        canHitPlayer.Value = true;
    }

    public void MeleeCantHit()
    {
        SharedBool canHitPlayer = (SharedBool)behaviorTree.GetVariable("canHitPlayer");
        canHitPlayer.Value = false;
    }

    // Method to change the enemy's state
    public void ChangeState(StateEnum newState)
    {
        currentState = newState;
        UpdateAnimator();
    }

    // Method to update the animator based on the current state
    private void UpdateAnimator()
    {
        // Update animator parameters based on the current state
        animator.SetBool("Firing", currentState == StateEnum.Firing);
        animator.SetBool("Idle", currentState == StateEnum.Idle);
        animator.SetBool("Run", currentState == StateEnum.Run);
        animator.SetBool("Walk", currentState == StateEnum.Walk);
        animator.SetBool("TossGrenade", currentState == StateEnum.TossGrenade);
       
        
    }
}