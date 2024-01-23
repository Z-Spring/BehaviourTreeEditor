using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedTransform : SharedVariable<Transform>
    {
        public static implicit operator SharedTransform(Transform value)
        {
            SharedTransform sharedTransform = CreateInstance<SharedTransform>();
            sharedTransform.sharedValue = value;
            return sharedTransform;
        }
    }
}