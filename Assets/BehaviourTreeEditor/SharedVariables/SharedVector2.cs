using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedVector2 : SharedVariable<Vector2>
    {
        public static implicit operator SharedVector2(Vector2 value)
        {
            SharedVector2 sharedVector2 = CreateInstance<SharedVector2>();
            sharedVector2.sharedValue = value;
            return sharedVector2;
        }
    }
}