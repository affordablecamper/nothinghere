using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Check to see if any audio sources are within hearing range of the current agent.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("a464405df8e82b24db602534724b5e6f", "941bd88188259374d885440560f1a29d")]
    public class CanHearAudioSource : Conditional
    {
        [Tooltip("The distance at which the agent can hear audio sources.")]
        public SharedFloat hearingDistance = 50f;
        [Tooltip("The tag of the audio source that the agent can hear.")]
        public SharedString targetTag;
        [Tooltip("The LayerMask of the audio sources that the agent can hear.")]
        public LayerMask targetLayerMask;
        [Tooltip("The position of the heard audio source.")]
        public SharedVector3 heardPosition;
        [Tooltip("The audibility threshold for footsteps.")]
        public SharedFloat footstepsAudibilityThreshold = 0.01f; // Adjust the audibility threshold for footsteps
        [Tooltip("The audibility threshold for gunshots.")]
        public SharedFloat gunshotAudibilityThreshold = 0.1f; // Adjust the audibility threshold for gunshots

        // Returns success if an audio source was heard, and sets the position of the heard audio source
        public override TaskStatus OnUpdate()
        {
            var audioSources = GameObject.FindGameObjectsWithTag(targetTag.Value);
            foreach (var audioSource in audioSources)
            {
                if (!audioSource.GetComponent<AudioSource>().isPlaying)
                    continue;

                var audibility = audioSource.GetComponent<AudioSource>().volume;
                var distance = Vector3.Distance(audioSource.transform.position, transform.position);

                // Adjust the condition based on audibility and distance from the agent
                if ((audibility >= footstepsAudibilityThreshold.Value && distance <= hearingDistance.Value) ||
                    (audibility >= gunshotAudibilityThreshold.Value && distance <= hearingDistance.Value))
                {
                    heardPosition.Value = audioSource.transform.position;
                    return TaskStatus.Success;
                }
            }

            return TaskStatus.Failure;
        }

        // Reset the public variables
        public override void OnReset()
        {
            hearingDistance = 50f;
            targetTag = "";
            targetLayerMask = LayerMask.NameToLayer("Default");
            heardPosition = Vector3.zero;
            footstepsAudibilityThreshold = 0.01f;
            gunshotAudibilityThreshold = 0.1f;
        }

        // Draw the hearing range
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || hearingDistance == null)
                return;

            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, hearingDistance.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}