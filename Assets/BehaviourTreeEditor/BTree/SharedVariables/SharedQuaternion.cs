using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedQuaternion : SharedVariable<Quaternion>
    {
        public static implicit operator SharedQuaternion(Quaternion value)
        {
            SharedQuaternion sharedQuaternion = CreateInstance<SharedQuaternion>();
            sharedQuaternion.sharedValue = value;
            return sharedQuaternion;
        }
    }
}