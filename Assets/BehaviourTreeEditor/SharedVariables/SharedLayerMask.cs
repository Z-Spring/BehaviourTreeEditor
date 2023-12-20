using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedLayerMask : SharedVariable<LayerMask>
    {
        public static implicit operator SharedLayerMask(LayerMask value)
        {
            var sharedVariable = CreateInstance<SharedLayerMask>();
            sharedVariable.SetValue(value);
            return sharedVariable;
        }
    }
}