using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedAnimationCurve : SharedVariable<AnimationCurve>
    {
        public static implicit operator SharedAnimationCurve(AnimationCurve value)
        {
            SharedAnimationCurve sharedAnimationCurve = CreateInstance<SharedAnimationCurve>();
            sharedAnimationCurve.sharedValue = value;
            return sharedAnimationCurve;
        }
    }
}