using UnityEngine;
using System.Collections;

public class SoundtrackManager : MonoBehaviour
{
    public AudioSource firstPartAudioSource;
    public AudioSource secondPartAudioSource;
    public AudioClip firstPartClip;
    public AudioClip secondPartClip;

    [SerializeField]
    private bool isInCombat = false;
    private bool isFading = false;
    private float fadeSpeed = 0.5f;
    private float targetVolume = 1f;
    private bool wasInCombat = false;
    public bool isEnemyVisible;
    private Coroutine combatTimerCoroutine;
    private float combatTimerDuration = 5f; // Duration of combat timer in seconds

    private void Start()
    {
        // Play the first part of the music initially
        PlayFirstPart();
    }

    private void Update()
    {
        // Simulated condition for enemy visibility
        isEnemyVisible = CheckEnemyVisibility();

        // Update combat status based on enemy visibility
        if (isEnemyVisible)
        {
            isInCombat = true;

            // Reset the combat timer
            ResetCombatTimer();
        }
        else if (combatTimerCoroutine == null)
        {
            isInCombat = false;
        }

        // If combat status changed, play the appropriate part of the music
        if (isInCombat != wasInCombat)
        {
            if (isInCombat)
            {
                // Play the second part with synchronization to the first part
                PlaySecondPartWithSynchronization();
            }
            else
            {
                // Fade in the first part and fade out the second part
                PlayFirstPartWithFade();
                FadeAudioSource(secondPartAudioSource, 0f);
            }
        }

        // Update the volume if fading
        if (isFading)
        {
            firstPartAudioSource.volume = Mathf.MoveTowards(firstPartAudioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            secondPartAudioSource.volume = Mathf.MoveTowards(secondPartAudioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

            // Stop fading if target volume reached
            if (Mathf.Approximately(firstPartAudioSource.volume, targetVolume) && Mathf.Approximately(secondPartAudioSource.volume, targetVolume))
            {
                isFading = false;

                // Stop the audio sources if volume is zero
                if (targetVolume == 0f)
                {
                    firstPartAudioSource.Stop();
                    secondPartAudioSource.Stop();
                }
            }
        }
    }

    private bool CheckEnemyVisibility()
    {
        // Cast a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        // Cast a ray from the cursor position
        // Uncomment the line below if you want to cast a ray from the cursor position instead
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limbs"))
            {
                // Hit an enemy with the layer "Limbs"
                return true;
            }
        }

        // Didn't hit an enemy with the layer "Limbs"
        return false;
    }

    private void PlayFirstPart()
    {
        firstPartAudioSource.clip = firstPartClip;
        firstPartAudioSource.Play();
        firstPartAudioSource.volume = 1f; // Ensure the volume is set to maximum
    }

    private void PlayFirstPartWithFade()
    {
        // Start fading in the first part
        FadeAudioSource(firstPartAudioSource, 1f);

        // Stop the second part and fade it out
        StopAudioSource(secondPartAudioSource);
        FadeAudioSource(secondPartAudioSource, 0f);
    }

    private void PlaySecondPartWithSynchronization()
    {
        // Fade out the first part
        FadeAudioSource(firstPartAudioSource, 0f);

        // Play the second part and fade it in
        PlayAudioSource(secondPartAudioSource, secondPartClip);
        FadeAudioSource(secondPartAudioSource, 1f);
        SyncAudioSources(firstPartAudioSource, secondPartAudioSource);

        // Start fading in the audio sources
        FadeAudioSources(1f);
    }

    private void SyncAudioSources(AudioSource source1, AudioSource source2)
    {
        source2.timeSamples = source1.timeSamples;
    }

    private void FadeAudioSource(AudioSource audioSource, float targetVolume)
    {
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

        // Check if the audio source reached the target volume
        if (Mathf.Approximately(audioSource.volume, targetVolume))
        {
            // Stop the audio source if volume is zero
            if (targetVolume == 0f)
            {
                audioSource.Stop();
            }
        }
    }

    private void StopAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.volume = 0f;
    }

    private void PlayAudioSource(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void FadeAudioSources(float targetVolume)
    {
        this.targetVolume = targetVolume;
        isFading = true;
    }

    private void ResetCombatTimer()
    {
        if (combatTimerCoroutine != null)
        {
            StopCoroutine(combatTimerCoroutine);
        }

        combatTimerCoroutine = StartCoroutine(CombatTimerCoroutine());
    }

    private IEnumerator CombatTimerCoroutine()
    {
        yield return new WaitForSeconds(combatTimerDuration);

        isInCombat = false;
        combatTimerCoroutine = null;
    }
}