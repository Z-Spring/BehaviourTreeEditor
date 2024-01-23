using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedColor : SharedVariable<Color>
    {
        public static implicit operator SharedColor(Color value)
        {
            var sharedColor = CreateInstance<SharedColor>();
            sharedColor.sharedValue = value;
            return sharedColor;
        }
    }
}