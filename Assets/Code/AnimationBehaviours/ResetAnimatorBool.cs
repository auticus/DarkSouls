using DarkSouls.Locomotion.Player;
using UnityEngine;

namespace DarkSouls.AnimationBehaviours
{
    public class ResetAnimatorBool : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition begins
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //this assumes the animator component on the player is on the same object as the PlayerLocomotion script
            if (animator.transform.TryGetComponent(out PlayerController player))
            {
                player.FinishInteractiveAnimation();
                return;
            }

            Debug.LogError("Player component on the Animator was not found!");
        }
    }
}
