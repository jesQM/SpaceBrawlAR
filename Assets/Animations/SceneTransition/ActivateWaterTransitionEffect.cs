using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWaterTransitionEffect : StateMachineBehaviour
{
    public bool activateEffect = true;

    private Material mat;
    private float amount = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CustomImageEffect effect = FindObjectOfType<CustomImageEffect>();
        if (effect == null) return;

        mat = effect.EffectMaterial;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (mat == null) return;

        if (activateEffect)
        {
            float totalTime = 0.5f;
            amount += Time.deltaTime * 1 / totalTime;
            mat.SetFloat("_Amount", amount);
        } else
        {
            amount = 0;
            mat.SetFloat("_Amount", amount);
            mat = null;
        }
    }
}
