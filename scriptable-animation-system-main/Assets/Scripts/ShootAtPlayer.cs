using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Shoot at the player and rotate towards the player.")]
    [TaskCategory("Movement")]
    public class ShootAtPlayer : Action
    {
        public float detectionRadius = 50f;
        public GameObject projectilePrefab;
        public SharedGameObject target;
        public float projectileSpeed = 10f;
        public float rateOfFire = 1f;
        public float damageAmount = 10f;
        public Transform muzzle;
        public float yOffset = 0.5f;
        public float reactionTime = 1f;
        public float dodgeSpeed = 5f;
        public float strafeRadius = 10f;
        public LayerMask obstacleMask; // Layer mask to detect obstacles
        public PlayerSetup setup;
        private Transform playerTransform;
        private bool isShooting;
        private bool isReadyToShoot;
        private float nextFireTime;
        private AudioSource source;
        private NavMeshAgent agent;
        private float shootingDuration;
        private float accuracyMultiplier;
        public AudioClip gunShot;
        public SendMovementToServer server;

        public override void OnStart()
        {
            playerTransform = target.Value.GetComponent<Transform>();
            isShooting = false;
            isReadyToShoot = false;
            nextFireTime = 0f;
            source = GetComponent<AudioSource>();
            agent = GetComponent<NavMeshAgent>();
            shootingDuration = 0f;
            accuracyMultiplier = 2f;
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
            {
                return TaskStatus.Failure;
            }

            Vector3 strafeDirection = CalculateStrafeDirection();
            Vector3 newDestination = playerTransform.position + strafeDirection * strafeRadius;
            server.SetDestinationToServer(newDestination);

            // Check if target is within detection radius and there is a clear line of sight
            if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRadius && CanShootTarget())
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + (1f / rateOfFire); // Incorporate deltaTime
                }
            }

            RotateTowardsPlayer();

            return TaskStatus.Running;
        }

        private bool CanShootTarget()
        {
            RaycastHit hit;
            if (Physics.Linecast(muzzle.position, playerTransform.position, out hit, obstacleMask))
            {
                // Obstacle between the NPC and target, cannot shoot
                return false;
            }

            return true;
        }

        private void Shoot()
        {
            source.PlayOneShot(gunShot);

            if (muzzle == null)
            {
                Debug.LogError("Muzzle not found. Make sure the shooter has a child object named 'Muzzle' at the shooting position.");
                return;
            }

            muzzle.LookAt(playerTransform.position + Vector3.up * yOffset);
            Vector3 bulletDirection = muzzle.forward.normalized;
            //Vector3 bulletDirection = muzzle.forward.normalized + Random.insideUnitSphere * (1f - accuracyMultiplier);
            GameObject newProjectile = GameObject.Instantiate(projectilePrefab, muzzle.transform.position, Quaternion.LookRotation(bulletDirection));
            newProjectile.GetComponent<Bullet>().ID = setup.playerID;
            newProjectile.GetComponent<Rigidbody>().velocity = bulletDirection * projectileSpeed;

            //shootingDuration += Time.deltaTime;
            //accuracyMultiplier = Mathf.Clamp01(1f - shootingDuration);
        }

        private void RotateTowardsPlayer()
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 50f);
            }
        }

        private bool IsFacingPlayer()
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0f;

            Vector3 forward = transform.forward;
            forward.y = 0f;

            return Vector3.Angle(forward, directionToPlayer) < 90f;
        }

        private void DodgeIncomingProjectiles()
        {
            Vector3 toTarget = (playerTransform.position - transform.position).normalized;
            if (Vector3.Dot(toTarget, playerTransform.forward) < 0)
            {
                return;
            }

            Vector3 dodgeDirection = Vector3.Cross(toTarget, Vector3.up);
            if (Vector3.Dot(dodgeDirection, playerTransform.right) < 0)
            {
                dodgeDirection = transform.right;
            }
            else
            {
                dodgeDirection = -transform.right;
            }

            Vector3 newDestination = transform.position + dodgeDirection * dodgeSpeed;
            server.SetDestinationToServer(newDestination);
        }

        private Vector3 CalculateStrafeDirection()
        {
            Vector3 toTarget = (playerTransform.position - transform.position).normalized;
            Vector3 dodgeDirection = Vector3.Cross(toTarget, Vector3.up);
            if (Vector3.Dot(toTarget, playerTransform.forward) >= 0)
            {
                if (Vector3.Dot(dodgeDirection, playerTransform.right) < 0)
                {
                    dodgeDirection = transform.right;
                }
                else
                {
                    dodgeDirection = -transform.right;
                }
            }

            return dodgeDirection;
        }

        public override void OnReset()
        {
            projectilePrefab = null;
            projectileSpeed = 10f;
            rateOfFire = 1f;
            damageAmount = 10f;
        }
    }
}