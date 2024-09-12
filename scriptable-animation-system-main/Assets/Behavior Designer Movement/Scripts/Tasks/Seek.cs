using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("3278c95539f686f47a519013713b31ac", "9f01c6fc9429bae4bacb3d426405ffe4")]
    public class Seek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        [UnityEngine.Serialization.FormerlySerializedAs("target")]
        public SharedGameObject m_Target;
        [Tooltip("If target is null then use the target position")]
        [UnityEngine.Serialization.FormerlySerializedAs("targetPosition")]
        public SharedVector3 m_TargetPosition;

        public float additionalTime = 2f; // Additional time buffer in seconds
        public SendMovementToServer server;
        private Vector3 originalDestination;
        private bool findingAlternateDestination;
        private const float alternateDestinationRange = 5f;
        private float estimatedTime;
        private float startTime;
        private NavMeshAgent agent;

        public override void OnStart()
        {
            base.OnStart();

            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("NavMeshAgent component is missing.");
                return;
            }

            originalDestination = Target();
            server.SetDestinationToServer(originalDestination);
            estimatedTime = CalculateEstimatedTime();
            startTime = Time.time;
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {

            


            if (HasArrived())
            {
                return TaskStatus.Success;
            }

            // Check if the agent fails to reach the destination within the estimated time plus additional time buffer
            if (Time.time - startTime >= estimatedTime + additionalTime)
            {
                return TaskStatus.Failure;
            }

            Vector3 targetPosition = Target();

            if (!findingAlternateDestination && !IsDestinationReachable(targetPosition))
            {
                // The original destination is unreachable, try to find an alternate reachable destination
                findingAlternateDestination = true;
                targetPosition = FindAlternateDestination(targetPosition);
            }



            return TaskStatus.Running;
        }

        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (m_Target.Value != null)
            {
                return m_Target.Value.transform.position;
            }
            return m_TargetPosition.Value;
        }

        public bool IsDestinationReachable(Vector3 destination)
        {
            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
            if (UnityEngine.AI.NavMesh.CalculatePath(transform.position, destination, UnityEngine.AI.NavMesh.AllAreas, path))
            {
                // Check if the path is valid and has at least one corner
                if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete && path.corners.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private Vector3 FindAlternateDestination(Vector3 originalDestination)
        {
            Vector3 alternateDestination = originalDestination;
            Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * alternateDestinationRange;
            alternateDestination.x += randomCirclePoint.x;
            alternateDestination.z += randomCirclePoint.y;

            // Check if the alternate destination is reachable
            if (IsDestinationReachable(alternateDestination))
            {
                return alternateDestination;
            }

            return originalDestination;
        }

        private float CalculateEstimatedTime()
        {
            float distance = Vector3.Distance(transform.position, originalDestination);
            float speed = agent.speed;
            return distance / speed;
        }

        public override void OnReset()
        {
            base.OnReset();
            m_Target = null;
            m_TargetPosition = Vector3.zero;
            originalDestination = Vector3.zero;
            findingAlternateDestination = false;
            estimatedTime = 0f;
            startTime = 0f;
        }
    }
}