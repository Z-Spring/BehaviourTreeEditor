using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public static implicit operator SharedVector3(Vector3 value)
        {
            SharedVector3 sharedVector3 = CreateInstance<SharedVector3>();
            sharedVector3.sharedValue = value;
            return sharedVector3;
        }
    }
}