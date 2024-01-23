using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public abstract class SharedVariable : ScriptableObject
    {
        public abstract void SetValue(object value);
        public abstract object GetValue();
    }
}