using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedGameObject : SharedVariable<GameObject>
    {
        public static implicit operator SharedGameObject(GameObject value)
        {
            SharedGameObject sharedGameObject = CreateInstance<SharedGameObject>();
            sharedGameObject.sharedValue = value;
            return sharedGameObject;
        }
    }
}