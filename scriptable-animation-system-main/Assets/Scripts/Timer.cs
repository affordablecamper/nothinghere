using UnityEngine;
using BehaviorDesigner.Runtime;

public class Timer : MonoBehaviour
{
    public float duration = 10f; // The duration of the initial timer in seconds
    public float additionalDuration = 10f; // The duration of the additional timer in seconds
    public BehaviorTree behaviorTree; // The Behavior Tree that contains the boolean variable to update
    public string boolVariableName = "TimerFinished"; // The name of the boolean variable to update

    private float currentTime; // The current time of the timer
    private bool isTimerFinished = false; // Indicates if the initial timer has reached zero
    private bool isAdditionalTimerFinished = false; // Indicates if the additional timer has reached zero

    private void Start()
    {
        currentTime = duration;
    }

    private void Update()
    {
        if (!isTimerFinished)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    isTimerFinished = true;
                    UpdateBehaviorTreeVariable();
                }
            }
        }
        else if (!isAdditionalTimerFinished)
        {
            // Wait for the additional timer to finish
            if (currentTime < additionalDuration)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= additionalDuration)
                {
                    currentTime = additionalDuration;
                    isAdditionalTimerFinished = true;
                    ResetBehaviorTreeVariable();
                }
            }
        }
    }

    private void UpdateBehaviorTreeVariable()
    {
        if (behaviorTree != null)
        {
            SharedBool boolVariable = behaviorTree.GetVariable(boolVariableName) as SharedBool;
            if (boolVariable != null)
            {
                boolVariable.Value = true;
            }
            else
            {
                Debug.LogError("Behavior Tree boolean variable not found: " + boolVariableName);
            }
        }
        else
        {
            Debug.LogError("Behavior Tree reference not set.");
        }
    }

    private void ResetBehaviorTreeVariable()
    {
        if (behaviorTree != null)
        {
            SharedBool boolVariable = behaviorTree.GetVariable(boolVariableName) as SharedBool;
            if (boolVariable != null)
            {
                boolVariable.Value = false;
            }
            else
            {
                Debug.LogError("Behavior Tree boolean variable not found: " + boolVariableName);
            }
        }
        else
        {
            Debug.LogError("Behavior Tree reference not set.");
        }
    }
}