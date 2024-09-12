using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] public float currentHealth;

    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    
    [SerializeField] private Behaviour[] behavioursToDisable;
    
    [SerializeField]private bool isDead = false;


    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public void PlayRandomDeathClip()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned.");
            return;
        }

        // Generate a random index within the range of the array
        int randomIndex = Random.Range(0, audioClips.Length);

        // Play the random audio clip
        audioSource.PlayOneShot(audioClips[randomIndex]);
    }


    private void Start()
    {
        currentHealth = maxHealth;
        
        // Disable ragdoll components initially
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        SetRagdollState(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void TakeDamage(int damage, Rigidbody rb)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(rb);
        }
    }

   
    private void Die()
    {
        isDead = true;
        PlayRandomDeathClip();
        // Enable ragdoll physics
        

        // Disable other components (e.g., AI, attack scripts, etc.)
        foreach (var behaviour in behavioursToDisable)
        {
            behaviour.enabled = false;
        }
        // Perform any other death-related actions
        
        // Destroy the game object after a delay (or implement respawn logic)
        Destroy(gameObject);
    }




    private void Die(Rigidbody rb)
    {
        isDead = true;
        PlayRandomDeathClip();
        // Enable ragdoll physics
        SetRagdollState(true);
        
        // Disable other components (e.g., AI, attack scripts, etc.)
        foreach (var behaviour in behavioursToDisable)
        {
            behaviour.enabled = false;
        }
        // Perform any other death-related actions
        rb.AddForce(-transform.forward * 2000); // change this to have its own force from the gun type and add all the rbs that were hit and then apply the force

        rb.transform.localScale = Vector3.zero;
        // Destroy the game object after a delay (or implement respawn logic)
        Destroy(gameObject, 400f);
    }

    private void SetRagdollState(bool isActive)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
        }

        foreach (var col in ragdollColliders)
        {
            //col.enabled = isActive;
        }
    }
}