using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedCollider : SharedVariable<Collider>
    {
        public static implicit operator SharedCollider(Collider value)
        {
            SharedCollider sharedCollider = CreateInstance<SharedCollider>();
            sharedCollider.sharedValue = value;
            return sharedCollider;
        }
    }
}