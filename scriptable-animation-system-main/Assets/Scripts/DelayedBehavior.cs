using UnityEngine;

public class DelayedBehavior : MonoBehaviour
{
    public float delayTime = 2f; // Time to wait before enabling the behavior
    public MonoBehaviour behaviorToEnable; // The behavior to enable

    private void Start()
    {
        // Start the delayed behavior after the specified delay time
        Invoke("EnableBehavior", delayTime);
    }

    private void EnableBehavior()
    {
        // Enable the desired behavior component
        behaviorToEnable.enabled = true;
    }
}