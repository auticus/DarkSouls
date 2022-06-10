using DarkSouls.Locomotion.Player;
using UnityEngine;

namespace DarkSouls.AnimationBehaviours
{
    public class AnimationInteractionComplete : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //this assumes the animator component on the player is on the same object as the PlayerLocomotion script
            if (animator.transform.TryGetComponent(out PlayerLocomotion player))
            {
                player.FinishInteractiveAnimation();
                return;
            }

            Debug.LogError("RollingBehaviour :: Player component on the Animator was not found!");
        }
    }
}
