using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
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