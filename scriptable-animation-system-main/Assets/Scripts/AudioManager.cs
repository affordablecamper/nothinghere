using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float pitchVariation = 0.1f;
    public AudioSource audioSource;

    // Call this method to play a footstep sound
    public void PlayFootstep()
    {
        // Check if there are footstep clips available
        if (footstepClips.Length == 0)
        {
            Debug.LogWarning("No footstep clips assigned!");
            return;
        }

        // Select a random footstep clip
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        // Set the pitch variation
        float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Play the footstep sound with the modified pitch
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }
}