using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
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