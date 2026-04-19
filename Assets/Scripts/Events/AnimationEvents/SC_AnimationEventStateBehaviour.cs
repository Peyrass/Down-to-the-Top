using UnityEngine;
using UnityEngine.Events;

public class SC_AnimationEventStateBehaviour : StateMachineBehaviour {
    public string eventName;
    [Range(0f, 1f)] public float triggerTime;
    
    bool hasTriggered;
    SC_AnimationEventReceiver receiver;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        hasTriggered = false;
        receiver = animator.GetComponent<SC_AnimationEventReceiver>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float currentTime = stateInfo.normalizedTime % 1f;

        if (!hasTriggered && currentTime >= triggerTime) {
            NotifyReceiver(animator);
            hasTriggered = true;
        }
    }

    void NotifyReceiver(Animator animator) {
        if (receiver != null) {
            receiver.OnAnimationEventTriggered(eventName);
        }
    }
}
